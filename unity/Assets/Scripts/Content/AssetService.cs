using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Threading;

namespace Osakana4242.Content {
	public class AssetService {
		static AssetService instance_;
		public static AssetService Instance => instance_;
		readonly Dictionary<string, (AssetInfo info, object asset)> holderDict_ = new();

		int loadingCount_;

		public static void Init() {
			instance_ = new AssetService();
		}

		public bool IsBusy() => 0 < loadingCount_;

		public async Task<T> GetAsync<T>(AssetInfo info, CancellationToken cancellationToken) where T : Object {
			++loadingCount_;
			try {
				var asset = await Main.Instance.resourceData.GetAsync<T>(info, cancellationToken);
				Debug.Log( $"cached {info.pathFromAssets}" );
				holderDict_.Add(info.fileName, (info, asset));
				return asset;
			} finally {
				--loadingCount_;
			}
		}

		public T Get<T>(AssetInfo info) where T : Object {
			if (!holderDict_.TryGetValue(info.fileName, out var holder))
				throw new System.Exception($"アセットがキャッシャされていない. name: {info}");
			return (T)holder.asset;
		}
	}
}
