﻿
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace Osakana4242.Lib.AssetServices {
	public class AssetHolder {
		public readonly AssetInfo info;
		readonly System.WeakReference assetRef_ = new(null);
		Object asset_;
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
		void SetAsset(Object asset) {
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

		static async Task<T> GetAsyncCore<T>(AssetInfo info, CancellationToken cancellationToken) where T : Object {
			info.ThrowIfCantCast<T>();
			await Task.Delay(1000);
			cancellationToken.ThrowIfCancellationRequested();
			var req = Resources.LoadAsync<T>(info.resourceName.value);
			Debug.Log($"GetAsyncStart {info.pathFromAssets}");
			while (req.isDone) {
				await Task.Delay(1);
				cancellationToken.ThrowIfCancellationRequested();
			}
			var asset = req.asset;
			Debug.Log($"GetAsyncEnd {info.pathFromAssets}, asset: {asset}");
			if (null == asset) {
				throw new System.Exception($"リソースが見つからない. name: '{info}'");
			}
			var castedAsset = (T)asset;
			return castedAsset;
		}

		public async Task<T> GetAsync<T>(CancellationToken cancellationToken) where T : Object {
			if (!IsNeedLoadStart())
				return await GetFromCachedAsync<T>(cancellationToken);
			return await GetFromFileAsync<T>(cancellationToken);
		}

		async Task<T> GetFromFileAsync<T>(CancellationToken cancellationToken) where T : Object {
			loading_ = true;
			AddWatcher();
			try {
				var asset = await GetAsyncCore<T>(info, cancellationToken);
				Debug.Log($"cached {info.pathFromAssets}");
				SetAsset(asset);
				return asset;
			} catch ( TaskCanceledException ) {
				throw;
			} catch ( System.Exception ex ) {
				ex_ = ex;
				throw ex;
			} finally {
				RemoveWatcher();
				loading_ = false;
			}
		}

		async Task<T> GetFromCachedAsync<T>(CancellationToken cancellationToken) {
			AddWatcher();
			try {
				while (loading_) {
					cancellationToken.ThrowIfCancellationRequested();
					await Task.Delay(1);
				}
				var asset1 = GetAsset<T>();
				return asset1;
			} finally {
				RemoveWatcher();
			}
		}
	}
}