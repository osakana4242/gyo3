using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

namespace Osakana4242.Content {
	public class ResourceService {
		static ResourceService instance_;
		public static ResourceService Instance => instance_;

		public GameObject blt01Prefab;
		public GameObject blt02Prefab;
		public GameObject ply01Prefab;
		public GameObject enm01Prefab;
		public GameObject eftBlast01Prefab;

		public static void Init() {
			instance_ = new ResourceService();
			instance_.Load();
		}

		public static async Task<int> InitAsync() {
			instance_ = new ResourceService();
			return await instance_.LoadAsync();
		}

		public void Load() {
			ply01Prefab = Load<GameObject>("mdl/ply_01.prefab");
			blt01Prefab = Load<GameObject>("mdl/blt_01.prefab");
			blt02Prefab = Load<GameObject>("mdl/blt_02.prefab");
			enm01Prefab = Load<GameObject>("mdl/enm_01.prefab");
			eftBlast01Prefab = Load<GameObject>("eft/eft_blast_01.prefab");
		}

		public async Task<int> LoadAsync() {
			ply01Prefab = await LoadAsync<GameObject>("mdl/ply_01.prefab");
			blt01Prefab = await LoadAsync<GameObject>("mdl/blt_01.prefab");
			enm01Prefab = await LoadAsync<GameObject>("mdl/enm_01.prefab");
			eftBlast01Prefab = await LoadAsync<GameObject>("eft/eft_blast_01.prefab");
			return 123;
		}

		public T Load<T>(string path) where T : Object {
			var path2 = System.IO.Path.GetDirectoryName( path ) + "/" +
			System.IO.Path.GetFileNameWithoutExtension( path );
			var t = Resources.Load<T>(path2);
			if (t == null) throw new System.Exception(string.Format("ロード失敗. path: '{0}'", path2));
			return t;
		}

		public async Task<T> LoadAsync<T>(string path) where T : Object {
			var path2 = System.IO.Path.GetDirectoryName( path ) + "/" +
			System.IO.Path.GetFileNameWithoutExtension( path );
			ResourceRequest req = Resources.LoadAsync<T>(path2);
			var completed = false;
			T asset = null;
			req.completed += (_ope) => {
				completed = _ope.isDone;
				asset = (T)req.asset;
			};
			return await Task.Run<T>(() => {
				while ( !completed );
				if (asset == null) throw new System.Exception(string.Format("ロード失敗. path: '{0}'", path2));
				return asset;
			});
		}
	}
}
