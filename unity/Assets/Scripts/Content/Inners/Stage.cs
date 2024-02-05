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
	public class Stage {
		static readonly AssetInfo[] preLoadAssetInfoList_g_ = {
			AssetInfos.PLAYER_ASSET,
			AssetInfos.PLY_01_PREFAB,

			AssetInfos.BLT_1_ASSET,
			AssetInfos.BLT_01_PREFAB,

			AssetInfos.BLT_2_ASSET,
			AssetInfos.BLT_02_PREFAB,

			AssetInfos.EFT_BLAST_01_PREFAB,
		};

		bool loading_;
		public Wave wave;

		public CharaBank charaBank = new CharaBank();
		public StageTime time = new StageTime();

		public static Stage Current => InnerMain.Instance.stage;
		readonly CancellationTokenSource cancellationTokenSource = new();
		List<object> assets_ = new List<object>();

		public Stage() {
			wave = new Wave();

		}
		public void Init() {
			Clear();
			wave.Init();
			if (!loading_) {
				LoadAssetAsync(cancellationTokenSource.Token).Forget();
			}
		}

		public void Dispose() {
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
			Clear();
		}

		public void Clear() {
			charaBank.ForEach(this, (_chara, _) => _chara.data.removeRequested = true);
			charaBank.RemoveAll(this, (_item, _) => _item.data.removeRequested);
		}

		public bool IsLoading() => loading_;

		public async UniTask LoadAssetAsync(CancellationToken cancellationToken) {
			while (loading_) {
				await UniTask.NextFrame(cancellationToken: cancellationToken);
			}
			loading_ = true;
			try {
				var tasks = preLoadAssetInfoList_g_.
					Select(info => AssetService.Instance.GetAsync<Object>(info, cancellationToken)).
					ForEach_Ext();

				var waveTask = wave.LoadAssetAsync(cancellationToken);

				foreach (var task in tasks) {
					cancellationToken.ThrowIfCancellationRequested();
					assets_.Add(await task);
				}

				await waveTask;
				loading_ = false;
			} catch (TaskCanceledException ex) {
				Debug.Log($"task canceled: {ex}");
				throw ex;
			} catch (System.Exception ex) {
				Debug.LogError($"ex: {ex}");
				throw ex;
			}
		}

		public void AddPlayerIfNeeded() {
			if (InnerMain.Instance.playerInfo.stock.IsEmpty()) return;
			if (charaBank.TryGetPlayer(out var _)) return;
			InnerMain.Instance.stage.charaBank.Add(CharaFactory.CreatePlayer());
		}

		public bool ExistsPlayer() {
			return charaBank.TryGetPlayer(out var _);
		}

		public void Update() {
			if (loading_) return;
			time.Update(UnityEngine.Time.deltaTime);
			charaBank.Update();
			wave.Update();

			if (charaBank.TryGetPlayer(out var chara)) {
				chara.Player.Report();
			}
		}

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

					if (debugStartTime + debugLoopDuration < Stage.Current.time.time) {
						Stage.Current.time.time = debugStartTime;
					}
				}

				var preTime = Stage.Current.time.preTime;
				var time = Stage.Current.time.time;

				for (int i = 0, iCount = data.eventList.Length; i < iCount; ++i) {
					var row = data.eventList[i];
					if (row.type == WaveEventType.None) continue;
					if (!TimeEvent.TryGetEvent(startTime + (row.startTime / 1000f), 0f, preTime, time, out var eventData)) continue;
					var pos = row.Position * 16f;
					var charaInfo = row.GetEnemyCharaInfo();
					var enemy = CharaFactory.CreateEnemy(charaInfo);
					enemy.data.position = pos;
					var vec = Vector2Util.FromDeg(row.angle);
					enemy.data.rotation = Quaternion.LookRotation((Vector3)vec);
					enemy.data.velocity = Vector3.zero;

					InnerMain.Instance.stage.charaBank.Add(enemy);
				}
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

	public enum WaveEventType {
		None = 0,
		Spawn = 1,
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

	[System.Serializable]
	public class StageTime {
		public float time;
		public float dt;

		public float preTime => time - dt;

		public void Update(float dt) {
			this.dt = dt;
			this.time += dt;
		}
	}
}
