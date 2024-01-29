using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
namespace Osakana4242.Content {
	public class AssetConfig : ScriptableObject {
#if UNITY_EDITOR
		public UnityEditor.DefaultAsset targetDirectory;

		[UnityEngine.ContextMenu("RefleshAssetList")]
		void Editor_RefleshAssetList() {
			var resourceInfoList = new List<AssetInfo>();
			var targetPath = UnityEditor.AssetDatabase.GetAssetPath(targetDirectory);
			var guidList = UnityEditor.AssetDatabase.FindAssets("", new string[] { targetPath });
			for (int i = 0, iCount = guidList.Length; i < iCount; ++i) {
				var guid = guidList[i];
				var info = AssetInfo.Editor_CreateByGUID(guid);
				if (info.IsDirectory())
					continue; // ディレクトリはスキップ.
				resourceInfoList.Add(info);
			}

			resourceInfoList.Sort((_a, _b) => _a.fileName.CompareTo(_b.fileName));

			WriteAssetInfosCSFile(resourceInfoList);
		}

		public static void WriteAssetInfosCSFile(List<AssetInfo> infoList) {
			var tmpl = @"
	namespace Osakana4242.Content {{
		public static class {0} {{
{1}
		}}
	}}
	";
			var sb = new System.Text.StringBuilder();
			for (int i = 0, iCount = infoList.Count; i < iCount; ++i) {
				var info = infoList[i];
				var varName = info.fileName;
				varName = varName.ToUpper();
				varName = varName.Replace(" ", "_");
				varName = varName.Replace(".", "_");
				sb.Append($"			public static readonly {nameof(AssetInfo)} {varName} = new (\"{info.pathFromAssets}\", typeof({info.type.FullName}));\n");
			}
			var src = string.Format(tmpl, nameof(AssetInfos), sb);
			src = src.Replace("\r\n", "\n");
			var srcPath = $"Assets/Scripts/Content/{nameof(AssetInfos)}.cs";
			System.IO.File.WriteAllText(srcPath, src, System.Text.Encoding.UTF8);
			UnityEditor.AssetDatabase.Refresh();
		}
#endif

	}
}
