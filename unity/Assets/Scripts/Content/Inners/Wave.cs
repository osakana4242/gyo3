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
		List<CharaInfo> charaInfos_ = new List<CharaInfo>();
		List<GameObject> models_ = new List<GameObject>();

		public void Init() {
			startTime = Stage.Current.time.time;
		}

		public async UniTask LoadAssetAsync(CancellationToken cancellationToken) {
			var charaInfoTasks = data.eventList.
				Where(row => row.type == WaveEventType.Spawn).
				Select(row => row.GetEnemyCharaInfoAssetInfo()).
				Union(new AssetInfo[] {
					AssetInfos.Get($"{testEnemy.enemyName}.asset")
				}).
				Distinct().
				Select(row => AssetService.Instance.GetAsync<CharaInfo>(row, cancellationToken)).
				ForEach_Ext();

			foreach (var charaInfoTask in charaInfoTasks) {
				cancellationToken.ThrowIfCancellationRequested();
				charaInfos_.Add(await charaInfoTask);
			}

			var modelTasks = charaInfos_.
				Select(charaInfo => AssetService.Instance.GetAsync<GameObject>(AssetInfos.Get(charaInfo.modelName), cancellationToken)).
				ForEach_Ext();

			foreach (var modelTask in modelTasks) {
				models_.Add(await modelTask);
			}


		}

		public void Update() {
			if (testEnemy.enabled) {
				UpdateTestEnemy();
				return;
			}
			if (0 < debugStartTime) {
				if (Stage.Current.time.time < debugStartTime) {
					Stage.Current.time.time = debugStartTime;
				}
				var debugEndTime = debugStartTime + debugLoopDuration;

				if (debugEndTime < Stage.Current.time.time) {
					Stage.Current.time.time = debugStartTime;
				}
			}

			var preTime = Stage.Current.time.preTime;
			var time = Stage.Current.time.time;

			for (int i = 0, iCount = data.eventList.Length; i < iCount; ++i) {
				var row = data.eventList[i];
				if (row.type == WaveEventType.None)
					continue;
				var eventStartTime = startTime + (row.startTime / 1000f);
				if (!TimeEvent.IsEnter(eventStartTime, preTime, time))
					continue;
				switch (row.type) {
					case WaveEventType.Spawn:
						Spawn(row); break;
					case WaveEventType.EndIfDestroyedEnemy:
						EndIfDestroyedEnemy(row);
						break;
				}
			}
			UpdateEndIfDestroyedBoss();
		}

		void UpdateEndIfDestroyedBoss() {
			if (!Stage.Current.isEndIfDestroyedBoss_) return;
			if (Stage.Current.charaBank.TryGetEnemy( out var _ ) ) return;
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

	public enum WaveEventType {
		None = 0,
		Spawn = 1,
		EndIfDestroyedEnemy = 2,
	}

	[System.Serializable]
	public class WaveEventData {
		public WaveEventType type;
		public float startTime;
		public string comment;
		public string enemyName;
		public float positionX;
		public float positionY;
		// 0: 右, 90 下, 180: 左, 270: 上
		public float angle;

		public Vector2 Position => new Vector2(positionX, positionY);
		public AssetInfo GetEnemyCharaInfoAssetInfo() => AssetInfos.Get($"{enemyName}.asset");
		public CharaInfo GetEnemyCharaInfo() => AssetService.Instance.Get<CharaInfo>(GetEnemyCharaInfoAssetInfo());
	}
}
