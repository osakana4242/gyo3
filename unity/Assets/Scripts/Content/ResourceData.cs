using UnityEngine;
using System.Collections.Generic;

namespace Osakana4242.Content {
	public class ResourceData : ScriptableObject {
		public List<Holder> assetList = new List<Holder>();
		public T Get<T>(string name) where T : Object {
			// TODO: 高速化.
			var holder = assetList.Find(_holder => _holder.name == name);
			return (T)holder.asset;
		}

		[System.Serializable]
		public class Holder {
			public string name;
			public Object asset;
		}

#if UNITY_EDITOR
		public UnityEditor.DefaultAsset targetDirectory;

		[UnityEngine.ContextMenu("RefleshAssetList")]
		void RefleshAssetList() {
			assetList.Clear();
			var targetPath = UnityEditor.AssetDatabase.GetAssetPath(targetDirectory);
			var guidList = UnityEditor.AssetDatabase.FindAssets("", new string[] { targetPath });
			for (int i = 0, iCount = guidList.Length; i < iCount; ++i) {
				var guid = guidList[i];
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				var name = System.IO.Path.GetFileName(path);
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
				Debug.Log("path: " + path + " type: " + asset.GetType().Name);
				if (asset is UnityEditor.DefaultAsset) {
					// ディレクトリはスキップ.
					continue;
				}
				var holder = new Holder() {
					name = name,
					asset = asset,
				};
				assetList.Add(holder);
			}

			assetList.Sort((_a, _b) => _a.name.CompareTo(_b.name));

			writeResouceList(assetList);
		}

		public static void writeResouceList(List<Holder> assetList) {
			var tmpl = @"
	namespace Osakana4242.Content {{
		public static class ResourceNames {{
{0}
		}}
	}}
	";
			var sb = new System.Text.StringBuilder();
			for (int i = 0, iCount = assetList.Count; i < iCount; ++i) {
				var holder = assetList[i];
				var varName = holder.name;
				varName = varName.ToUpper();
				varName = varName.Replace(" ", "_");
				varName = varName.Replace(".", "_");
				sb.Append($"			public const string {varName} = \"{holder.name}\";\n");
			}
			var src = string.Format(tmpl, sb);
			src = src.Replace("\r\n", "\n");
			var srcPath = "Assets/Scripts/Content/ResourceNames.cs";
			System.IO.File.WriteAllText(srcPath, src, System.Text.Encoding.UTF8);
			UnityEditor.AssetDatabase.Refresh();
		}
#endif
	}
}
