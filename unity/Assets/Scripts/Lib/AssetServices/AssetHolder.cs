

using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Osakana4242.Lib.AssetServices {
	public class AssetHolder {
		public readonly AssetInfo info;
		readonly System.WeakReference assetRef_ = new(null);
		UnityEngine.Object asset_;
		bool loading_;
		int watcherCount_;
		System.Exception ex_;

		public AssetHolder(AssetInfo info) {
			assetRef_.Target = null;
			this.info = info;
		}

		bool IsNeedLoadStart() => !assetRef_.IsAlive && !loading_;
		public T GetAsset<T>() {
			info.ThrowIfCantCast<T>();
			if (!TryGetAsset<T>(out var asset)) {
				if (null != ex_) {
					throw new System.Exception("アセットのロードに失敗している", ex_);
				}
				throw new System.Exception($"アセットがキャッシュされていない. path: {info.pathFromAssets}");
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
			this.asset_ = asset;
			this.assetRef_.Target = asset;
		}
		void AddWatcher() {
			++watcherCount_;
		}

		void RemoveWatcher() {
			if (watcherCount_ <= 0) throw new System.Exception($"watcherCount: {watcherCount_}");
			--watcherCount_;
			if (0 < watcherCount_) return;
			asset_ = null;
		}

		static async UniTask<T> GetAsyncCore<T>(AssetInfo info, CancellationToken cancellationToken) where T : UnityEngine.Object {
			Debug.Log($"{nameof(GetAsyncCore)} - start: {info.pathFromAssets}");
			info.ThrowIfCantCast<T>();
			await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			var asset = await Resources.LoadAsync<T>(info.resourceName.value);
			if (null == asset) {
				throw new System.Exception($"リソースが見つからない. name: '{info}'");
			}
			var castedAsset = (T)asset;
			Debug.Log($"{nameof(GetAsyncCore)} - completed: {info.pathFromAssets}");
			return castedAsset;
		}

		public async UniTask<T> GetAsync<T>(CancellationToken cancellationToken) where T : UnityEngine.Object {
			if (!IsNeedLoadStart())
				return await GetFromCachedAsync<T>(cancellationToken);
			return await GetFromFileAsync<T>(cancellationToken);
		}

		async UniTask<T> GetFromFileAsync<T>(CancellationToken cancellationToken) where T : UnityEngine.Object {
			loading_ = true;
			AddWatcher();
			try {
				var asset = await GetAsyncCore<T>(info, cancellationToken);
				SetAsset(asset);
				return asset;
			} catch (OperationCanceledException) {
				throw;
			} catch (System.Exception ex) {
				ex_ = ex;
				throw ex;
			} finally {
				RemoveWatcher();
				loading_ = false;
			}
		}

		async UniTask<T> GetFromCachedAsync<T>(CancellationToken cancellationToken) {
			AddWatcher();
			try {
				while (loading_) {
					cancellationToken.ThrowIfCancellationRequested();
					await UniTask.Delay(1);
				}
				var asset1 = GetAsset<T>();
				return asset1;
			} finally {
				RemoveWatcher();
			}
		}
	}
}
