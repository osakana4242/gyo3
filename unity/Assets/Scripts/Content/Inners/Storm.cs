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
using System;
namespace Osakana4242.Content.Inners {


	[System.Serializable]
	public class Storm {
		public float startTime;
		public StormData data;

		public float debugStartTime;
		public float debugLoopDuration;
		public Config.TestEnemyWave testEnemy => Config.instance.testEnemy;
		public int testEnemyIndex;
		List<CharaInfo> charaInfos_ = new List<CharaInfo>();
		List<GameObject> models_ = new List<GameObject>();

		Storm child_;
		int readedIndex_;
		float rowTime_;
		Wave wave_ = new Wave();

		public void Init() {
			startTime = Stage.Current.time.time;
			wave_.Init();
		}

		public async UniTask LoadAssetAsync(CancellationToken cancellationToken) {

			List<StormEventData> eventList = new();

			data.ForEachEvent(new HashSet<StormData>(), eventList, (_evt, _eventList) => {
				_eventList.Add(_evt);
			});

			var charaInfoTasks = eventList.
				Where(_evt => _evt.type == StormEventType.SpawnWave).
				SelectMany(_evt => _evt.GetSpawnWave().wave.eventList).
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
			child_?.Update();

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
			rowTime_ += Stage.Current.time.dt;
			Hoge();
			wave_.Update();
			UpdateEndIfDestroyedBoss();
		}

		void Hoge() {
			for (int i = readedIndex_, iCount = data.eventList.Length; i < iCount; ++i) {
				var row = data.eventList[i];

				switch (row.type) {
					case StormEventType.None:
						break;
					case StormEventType.Delay:
						if (rowTime_ < row.GetDelay().delay) {
							return;
						}
						break;
					case StormEventType.SpawnWave:
						var w = row.GetSpawnWave();
						TimeEvent.TryGetEvent(0, 999, rowTime_ - Stage.Current.time.dt, rowTime_);
						wave_.Init();
						wave_.data = w.wave;
						break;
					case StormEventType.SetStorm:
						var s = row.GetSetStorm();
						if ( s.railIndex <= 0 ) {
							this.data = s.storm;
							// 打ち切り.
							return;
						} else {
							if ( null ==  child_) {
								child_ = new Storm();
							}
							child_.Init();
							child_.data = s.storm;
						}
						break;
				}

				rowTime_ = 0f;
				readedIndex_ = i;
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
