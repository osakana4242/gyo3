
using UnityEngine;

namespace Osakana4242.Lib.AssetServices {
	public readonly struct AssetInfo {
		public readonly string fileName;
		public readonly string pathFromAssets;
		public readonly System.Type type;
		public readonly ResourceName resourceName;
		public readonly string fileNameWithoutExtention;

		public AssetInfo(string path, System.Type type) {
			this.pathFromAssets = path;
			this.fileName = System.IO.Path.GetFileName(path);
			this.fileNameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(this.fileName);
			this.resourceName = new ResourceName(path);
			this.type = type;
		}

		public bool IsDirectory() => type == typeof(UnityEditor.DefaultAsset);
		public override string ToString() => $"{pathFromAssets}";
		public string AnimationClipName {
			get {
				if (type != typeof(AnimationClip))
					throw new System.Exception($"{nameof(AnimationClip)} ではない. pathFromAssets: {pathFromAssets}");
				return fileNameWithoutExtention;
			}
		}

#if UNITY_EDITOR
		public static AssetInfo Editor_CreateByGUID(string guid) {
			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
			Debug.Log("path: " + path + " type: " + asset.GetType().Name);
			var type = asset.GetType();
			return new AssetInfo(path, type);
		}
#endif
	}

}
