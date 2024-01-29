using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.InputSystem.Utilities;
using System.Collections.ObjectModel;

namespace Osakana4242.Content {
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

		public async Task<T> GetAsync<T>(AssetInfo info, CancellationToken cancellationToken) where T : Object {
			++loadingCount_;
			try {
				if (!holderDict_.TryGetValue(info.fileName, out var holder)) {
					holder = new(info);
					holderDict_.Add(info.fileName, holder);
				}
				return await holder.GetAsync<T>(cancellationToken);
			} finally {
				--loadingCount_;
			}
		}
		public T Get<T>(AssetInfo info) where T : Object {
			if (!holderDict_.TryGetValue(info.fileName, out var holder))
				throw new System.Exception($"アセットがキャッシャされていない. name: {info.fileName}");
			return holder.GetAsset<T>();
		}
	}
}
