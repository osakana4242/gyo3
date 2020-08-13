using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class ResourceService {
		static ResourceService instance_;
		public static ResourceService Instance => instance_;
		public static void Init() {
			instance_ = new ResourceService();
			instance_.Load();
		}

		public void Load() {
			ply01Prefab = Load<GameObject>("mdl/ply_01.prefab");
			blt01Prefab = Load<GameObject>("mdl/blt_01.prefab");
			enm01Prefab = Load<GameObject>("mdl/enm_01.prefab");
		}

		public T Load<T>(string path) where T : Object {
			var path2 = System.IO.Path.GetDirectoryName( path ) + "/" +
			System.IO.Path.GetFileNameWithoutExtension( path );
			var t = Resources.Load<T>(path2);
			if (t == null) throw new System.Exception(string.Format("ロード失敗. path: '{0}'", path2));
			return t;
		}

		public GameObject blt01Prefab;
		public GameObject ply01Prefab;
		public GameObject enm01Prefab;
	}
}
