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


	[Serializable]
	public class Storm {
		public int railIndex;
		public Msec startTime;
		public StormData data;
		int readedIndex_;
		Msec rowTime_;

		public Msec debugStartTime;
		public Msec debugLoopDuration;
		public Config.TestEnemyWave TestEnemy => Config.instance.testEnemy;
		public int testEnemyIndex;
		List<CharaInfo> charaInfos_ = new List<CharaInfo>();
		List<GameObject> models_ = new List<GameObject>();

		[SerializeField] Wave wave_ = new Wave();

		public Storm(int id) {
			this.railIndex = id;
			data = StormData.Empty;
		}

		public void Init() {
			Clear();
			startTime = Stage.Current.time.time;
			wave_.Init();
			Debug.Log( $"Storm, railIndex: {railIndex} Init" );
		}

		public void Clear() {
			wave_.Clear();
			startTime = Msec.Zero;
			rowTime_ = Msec.Zero;
			readedIndex_ = 0;
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
					AssetInfos.Get($"{TestEnemy.enemyName}.asset")
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
			if (TestEnemy.enabled) {
				UpdateTestEnemy();
				return;
			}
			if (Msec.Zero < debugStartTime) {
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
			UpdateEvents();
			UpdateEndIfDestroyedBoss();
		}

		void UpdateEvents() {
			var dt = Stage.Current.time.dt;
			for (var iCount = data.eventList.Length; readedIndex_ < iCount; ++readedIndex_) {
				var row = data.eventList[readedIndex_];

				switch (row.type) {
					case StormEventType.None:
						break;
					case StormEventType.Delay:
						if (rowTime_ < row.GetDelay().delay)
							return;
						break;
					case StormEventType.SpawnWave:
						if (!TimeEvent.TryGetEvent(Msec.Zero, rowTime_ - dt, rowTime_, out var timeEvent))
							return;
						
						switch (timeEvent.type) {
							case TimeEventType.Enter:
								var w = row.GetSpawnWave();
								wave_.Init();
								wave_.data = w.wave;
								break;
							case TimeEventType.Loop:
								break;
						}
						wave_.Update();
						if (wave_.IsPlaying())
							return;
						break;
					case StormEventType.SetStorm:
						var s = row.GetSetStorm();
						if ( s.railIndex == this.railIndex ) {
							Init();
							this.data = s.storm;
							// 打ち切り.
							return;
						} else {
							var otherStorm = Stage.Current.stormList_[ s.railIndex ];
							otherStorm.Init();
							otherStorm.data = s.storm;
						}
						break;
				}
				Debug.Log( $"Storm {railIndex}, event: {readedIndex_}, type: {row.type}" );
				rowTime_ = Msec.Zero;
			}
		}

		void UpdateEndIfDestroyedBoss() {
			if (!Stage.Current.isEndIfDestroyedBoss_) return;
			if (Stage.Current.charaBank.TryGetEnemy(out var _)) return;
			InnerMain.Instance.hasStageClearRequest = true;
		}

		void EndIfDestroyedEnemy(WaveEventData row) {
			Stage.Current.isEndIfDestroyedBoss_ = true;
		}

		public void UpdateTestEnemy() {
			if (!Stage.Current.charaBank.TryGetEnemy(out var enemy)) {
				var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.Get($"{TestEnemy.enemyName}.asset"));
				enemy = CharaFactory.CreateEnemy(info);
				enemy.data.position = TestEnemy.positionList[testEnemyIndex];
				enemy.data.rotation = Quaternion.LookRotation(new Vector2(-1f, 0f));
				InnerMain.Instance.stage.charaBank.Add(enemy);
				testEnemyIndex = (testEnemyIndex + 1) % TestEnemy.positionList.Length; ;
			}
		}
	}
}
