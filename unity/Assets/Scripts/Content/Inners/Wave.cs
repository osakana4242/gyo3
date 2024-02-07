using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using System.Threading;
using Osakana4242.Lib.AssetServices;
using Cysharp.Threading.Tasks;
namespace Osakana4242.Content.Inners {


	[System.Serializable]
	public class Wave {
		public float startTime;
		public WaveData data;

		public float debugStartTime;
		public float debugLoopDuration;
		public Config.TestEnemyWave testEnemy => Config.instance.testEnemy;
		public int testEnemyIndex;
		int eventIndex_;
		HashSet<int> enemyIds = new HashSet<int>();
		bool isEnd_ = false;

		public void Init() {
			Clear();
			startTime = Stage.Current.time.time;
		}

		public void Clear() {
			data = WaveData.Empty;
			startTime = 0;
			eventIndex_ = 0;
			isEnd_ = false;
			enemyIds.Clear();
		}

		public bool IsPlaying() => !isEnd_;
		bool IsEndEvents() => data.eventList.Length <= eventIndex_;

		public void Update() {
			if (isEnd_)
				return;

			if (testEnemy.enabled) {
				UpdateTestEnemy();
				return;
			}

			UpdateEvents();

			if (!IsEndEvents())
				return;

			isEnd_ = CountActive() <= 0;
			if (isEnd_)
				Debug.Log($"wave is end: {isEnd_}");
		}

		int CountActive() {
			var activeCount = 0;
			foreach (var enemyId in enemyIds) {
				if (!Stage.Current.charaBank.dict.TryGetValue(enemyId, out var _)) continue;
				++activeCount;
			}
			return activeCount;
		}

		void UpdateEvents() {
			var preTime = Stage.Current.time.preTime;
			var time = Stage.Current.time.time;
			for (var iCount = data.eventList.Length; eventIndex_ < iCount; ++eventIndex_) {
				var row = data.eventList[eventIndex_];

				if (row.type == WaveEventType.None)
					continue;

				var eventStartTime = startTime + (row.startTime / 1000f);
				var isEnter = TimeEvent.IsEnter(eventStartTime, preTime, time);
				Debug.Log($"index: {eventIndex_}, eventStartTime: {eventStartTime}, isEnter: {isEnter}");

				if (!isEnter)
					return;

				switch (row.type) {
					case WaveEventType.Spawn:
						Spawn(row);
						break;
				}
			}
		}

		void UpdateEndIfDestroyedBoss() {
			if (!Stage.Current.isEndIfDestroyedBoss_) return;
			if (Stage.Current.charaBank.TryGetEnemy(out var _)) return;
			InnerMain.Instance.hasStageClearRequest = true;
		}

		void Spawn(WaveEventData row) {
			var pos = row.Position * 16f;
			var charaInfo = row.GetEnemyCharaInfo();
			var enemy = CharaFactory.CreateEnemy(charaInfo);
			enemy.data.position = pos;
			var vec = Vector2Util.FromDeg(row.angle);
			enemy.data.rotation = Quaternion.LookRotation((Vector3)vec);

			enemy.data.velocity = Vector3.zero;
			enemyIds.Add(enemy.data.id);

			InnerMain.Instance.stage.charaBank.Add(enemy);
		}

		void EndIfDestroyedEnemy(WaveEventData row) {
			Stage.Current.isEndIfDestroyedBoss_ = true;
		}

		public void UpdateTestEnemy() {
			if (!Stage.Current.charaBank.TryGetEnemy(out var enemy)) {
				var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.Get($"{testEnemy.enemyName}.asset"));
				enemy = CharaFactory.CreateEnemy(info);
				enemy.data.position = testEnemy.positionList[testEnemyIndex];
				enemy.data.rotation = Quaternion.LookRotation(new Vector2(-1f, 0f));
				InnerMain.Instance.stage.charaBank.Add(enemy);
				testEnemyIndex = (testEnemyIndex + 1) % testEnemy.positionList.Length; ;
			}
		}
	}
}
