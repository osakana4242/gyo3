

using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Osakana4242.Lib.AssetServices {
	public class AssetHolder {
		static readonly TimeSpan DebugLoadingDuration = TimeSpan.FromMilliseconds(1);

		public readonly AssetInfo info;
		readonly WeakReference assetRef_ = new(null);
		UnityEngine.Object asset_;
		bool loading_;
		int watcherCount_;
		Exception ex_;
		int loadedCount_;

		public AssetHolder(AssetInfo info) {
			assetRef_.Target = null;
			this.info = info;
		}

		bool IsNeedLoadStart() =>
			!assetRef_.IsAlive && !loading_;

		public T GetAsset<T>() {
			info.ThrowIfCantCast<T>();
			if (!TryGetAsset<T>(out var asset)) {
				if (null != ex_) {
					throw new Exception("アセットのロードに失敗している", ex_);
				}
				throw new Exception($"アセットがキャッシュされていない. path: {info.pathFromAssets}");
			}
			return asset;
		}
		bool TryGetAsset<T>(out T asset) {
			if (IsNeedLoadStart()) {
				asset = default;
				return false;
			}
			asset = (T)assetRef_.Target;
			return true;
		}
		void SetAsset(UnityEngine.Object asset) {
			asset_ = asset;
			assetRef_.Target = asset;
			++loadedCount_;
			// if (2 <=loadedCount_)
			// 	Debug.LogWarning($"SetAssets, name: {info.pathFromAssets}, loadedCount_: {loadedCount_}");
		}
		void AddWatcher() {
			++watcherCount_;
		}

		void RemoveWatcher() {
			if (watcherCount_ <= 0) throw new Exception($"watcherCount: {watcherCount_}");
			--watcherCount_;
			if (0 < watcherCount_) return;
			asset_ = null;
		}

		static async UniTask<T> GetAsyncCore<T>(AssetInfo info, CancellationToken cancellationToken) where T : UnityEngine.Object {
			Debug.Log($"{nameof(GetAsyncCore)} - start: {info.pathFromAssets}");
			info.ThrowIfCantCast<T>();
			await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			var asset = await Resources.LoadAsync<T>(info.resourceName.value);
			if (null == asset) {
				throw new Exception($"リソースが見つからない. name: '{info}'");
			}
			var castedAsset = (T)asset;
			Debug.Log($"{nameof(GetAsyncCore)} - completed: {info.pathFromAssets}");
			return castedAsset;
		}

		public async UniTask<T> GetAsync<T>(CancellationToken cancellationToken) where T : UnityEngine.Object {
			AddWatcher();
			try {
				await WaitLoadingAsync(cancellationToken);

				if (!IsNeedLoadStart())
					return await GetFromCachedAsync<T>(cancellationToken);

				return await GetFromFileAsync<T>(cancellationToken);
			} finally {
				RemoveWatcher();
			}
		}

		async UniTask<T> GetFromFileAsync<T>(CancellationToken cancellationToken) where T : UnityEngine.Object {
			loading_ = true;
			try {
				var asset = await GetAsyncCore<T>(info, cancellationToken);
				SetAsset(asset);
				return asset;
			} catch (OperationCanceledException) {
				throw;
			} catch (Exception ex) {
				ex_ = ex;
				throw ex;
			} finally {
				loading_ = false;
			}
		}

		async UniTask<T> GetFromCachedAsync<T>(CancellationToken cancellationToken) {
			while (loading_) {
				cancellationToken.ThrowIfCancellationRequested();
				await UniTask.Delay(DebugLoadingDuration);
			}
			cancellationToken.ThrowIfCancellationRequested();
			var asset1 = GetAsset<T>();
			return asset1;
		}

		async UniTask WaitLoadingAsync(CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();
			while (loading_) {
				await UniTask.Delay(DebugLoadingDuration);
				cancellationToken.ThrowIfCancellationRequested();
			}
		}

	}
}
