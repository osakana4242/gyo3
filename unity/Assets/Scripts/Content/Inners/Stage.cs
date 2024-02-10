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

			AssetInfos.STORM_R_01_01_ASSET,
		};

		int loading_ = 0;

		public List<Storm> stormList_ = new List<Storm>();

		public CharaBank charaBank = new CharaBank();
		public StageTime time = new StageTime();

		public static Stage Current => InnerMain.Instance.stage;
		CancellationTokenSource cancellationTokenSource = new();
		List<object> assets_ = new List<object>();
		public bool isEndIfDestroyedBoss_;

		public Stage() {
		}
		public void Init() {
			Clear();
			for (var i = 0; i < 2; ++i) {
				var storm = new Storm(i);
				storm.Init();
				stormList_.Add(storm);
			}
			LoadAssetAsync(cancellationTokenSource.Token).Forget();
		}

		public void Dispose() {
			Clear();
		}

		public void Clear() {
			Debug.Log("CANCEL by Clear");
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
			cancellationTokenSource = new();
			isEndIfDestroyedBoss_ = false;
			time.Clear();
			charaBank.ForEach(this, (_chara, _) => _chara.data.removeRequested = true);
			charaBank.RemoveAll(this, (_item, _) => _item.data.removeRequested);
			stormList_.Clear();
		}

		public bool IsLoading() => 0 < loading_;

		public async UniTask LoadAssetAsync(CancellationToken cancellationToken) {
			++loading_;
			Debug.Log($"Stage Load Start, loading_: {loading_}");
			while (2 <= loading_) {
				Debug.Log($"wait loading_: {loading_}");
				await UniTask.NextFrame(cancellationToken: cancellationToken);
			}
			try {
				var tasks = preLoadAssetInfoList_g_.
					Select(info => AssetService.Instance.GetAsync<UnityEngine.Object>(info, cancellationToken)).
					ForEach_Ext();
				await tasks;
				stormList_[0].data = AssetService.Instance.Get<StormData>(AssetInfos.STORM_R_01_01_ASSET);

				var waveTask = stormList_[0].LoadAssetAsync(cancellationToken);

				foreach (var task in tasks) {
					cancellationToken.ThrowIfCancellationRequested();
					assets_.Add(await task);
				}

				await waveTask;
				--loading_;
				Debug.Log($"Stage Load Completed, loading: {loading_}");
			} catch (OperationCanceledException ex) {
				await Resources.UnloadUnusedAssets();
				GC.Collect();
				GC.Collect();
				--loading_;
				Debug.Log($"Stage Load Canceled, loading: {loading_}, ex: {ex}");
			} catch (Exception ex) {
				Debug.LogError($"Stage Load Failed, loading: {loading_}, ex: {ex}");
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
			if (IsLoading()) return;
			time.Update(Msec.FromSeconds(Time.deltaTime));
			charaBank.Update();
			stormList_.ForEach(_storm => _storm.Update());

			if (charaBank.TryGetPlayer(out var chara)) {
				chara.Player.Report();
			}
		}
	}

	[System.Serializable]
	public class StageTime {
		public Msec time;
		public Msec dt;

		public Msec preTime => time - dt;

		public void Update(Msec dt) {
			this.dt = dt;
			this.time += dt;
		}

		public void Clear() {
			this.time = Msec.Zero;
			this.dt = Msec.Zero;
		}
	}
}
