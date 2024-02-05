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
			AssetInfos.EFT_BLAST_01_PREFAB,

			AssetInfos.WAVE_01_ASSET,
		};

		bool loading_;
		public Wave wave;

		public CharaBank charaBank = new CharaBank();
		public StageTime time = new StageTime();

		public static Stage Current => InnerMain.Instance.stage;
		readonly CancellationTokenSource cancellationTokenSource = new();
		List<object> assets_ = new List<object>();
		public bool isEndIfDestroyedBoss_;

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
			isEndIfDestroyedBoss_ = false;
			time.Clear();
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
				await tasks;
				wave.data = AssetService.Instance.Get<WaveData>(AssetInfos.WAVE_01_ASSET);

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

		public void Clear() {
			this.time = 0;
			this.dt = 0;
		}
	}
}
