﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Osakana4242.Lib.AssetServices {
	public class AssetService {
		static AssetService instance_;
		public static AssetService Instance => instance_;
		readonly Dictionary<string, AssetHolder> holderDict_ = new();

		int loadingCount_;

		public static void Init() {
			instance_ = new AssetService();
		}

		public void Dispose() {
			holderDict_.Clear();
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}

		public bool IsBusy() => 0 < loadingCount_;

		public async UniTask<T> GetAsync<T>(AssetInfo info, CancellationToken cancellationToken) where T : UnityEngine.Object {
			++loadingCount_;
			// Debug.Log($"increment loadingCount: {loadingCount_}");
			try {
				info.ThrowIfCantCast<T>();
				if (!holderDict_.TryGetValue(info.fileName, out var holder)) {
					holder = new(info);
					holderDict_.Add(info.fileName, holder);
				}
				return await holder.GetAsync<T>(cancellationToken);
			} catch ( OperationCanceledException ex ) {
				Debug.LogWarning( $"GetAsync canceled. fileName: '{info.fileName}', ex: {ex}" );
				throw ex;
			} catch ( Exception ex ) {
				Debug.LogError( $"ex: {ex}" );
				throw ex;
			} finally {
				// Debug.Log($"decrement loadingCount: {loadingCount_}");
				--loadingCount_;
			}
		}
		public T Get<T>(AssetInfo info) where T : UnityEngine.Object {
			info.ThrowIfCantCast<T>();
			if (!holderDict_.TryGetValue(info.fileName, out var holder))
				throw new System.Exception($"アセットがキャッシャされていない. name: {info.fileName}");
			return holder.GetAsset<T>();
		}
	}
}
