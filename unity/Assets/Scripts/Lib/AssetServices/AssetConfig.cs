
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Osakana4242.Lib.AssetServices {
	public class AssetConfig : ScriptableObject {
#if UNITY_EDITOR
		static string tmpl_g_ = @"
/// genereated from <see cref=""{0}"" />.

using System.Collections.Generic;
using {1};

namespace {2} {{
	public static class {3} {{
		static readonly Dictionary<string, AssetInfo> dict_g = new Dictionary<string, AssetInfo>();

		static AssetInfo Add(AssetInfo info) {{
			dict_g.Add(info.fileName, info);
			return info;
		}}

		public static AssetInfo Get(string name) => dict_g[name];

{4}

	}}
}}
	";

		[SerializeField] UnityEditor.DefaultAsset targetDirectory_;
		[SerializeField] UnityEditor.DefaultAsset outDirectory_;
		[SerializeField] string className_ = "Osakana4242.Content.AssetInfos";

		[UnityEngine.ContextMenu("RefleshAssetList")]
		void Editor_RefleshAssetList() {
			var resourceInfoList = new List<AssetInfo>();
			var targetPath = UnityEditor.AssetDatabase.GetAssetPath(targetDirectory_);
			var guidList = UnityEditor.AssetDatabase.FindAssets("", new string[] { targetPath });
			for (int i = 0, iCount = guidList.Length; i < iCount; ++i) {
				var guid = guidList[i];
				var info = AssetInfo.Editor_CreateByGUID(guid);
				if (info.IsDirectory())
					continue; // ディレクトリはスキップ.
				resourceInfoList.Add(info);
			}

			resourceInfoList.Sort((_a, _b) => _a.fileName.CompareTo(_b.fileName));

			WriteAssetInfosCSFile(
				new ClassInfo(outDirectory_, className_),
				resourceInfoList
			);
		}

		static void WriteAssetInfosCSFile(in ClassInfo classInfo, List<AssetInfo> infoList) {
			var sb = new System.Text.StringBuilder();
			for (int i = 0, iCount = infoList.Count; i < iCount; ++i) {
				var info = infoList[i];
				var varName = info.fileName;
				varName = varName.ToUpper();
				varName = varName.Replace(" ", "_");
				varName = varName.Replace(".", "_");
				sb.Append($"		public static readonly {nameof(AssetInfo)} {varName} = Add(new (\"{info.pathFromAssets}\", typeof({info.type.FullName})));\n");
			}
			var src = string.Format(tmpl_g_,
				typeof(AssetConfig).FullName,
				typeof(AssetInfo).Namespace,
				classInfo.nameSpaceName,
				classInfo.className,
				sb
			);
			src = src.Replace("\r\n", "\n");
			System.IO.File.WriteAllText(classInfo.srcPath, src, System.Text.Encoding.UTF8);
			UnityEditor.AssetDatabase.Refresh();
		}

		readonly struct ClassInfo {
			public readonly string srcPath;
			public readonly string nameSpaceName;
			public readonly string className;

			public ClassInfo(
				DefaultAsset directory,
				string fullClassName
			) {
				if (null == directory) throw new System.ArgumentNullException("directory");
				if (string.IsNullOrEmpty(fullClassName)) throw new System.ArgumentNullException("fullClassName");

				{
					var separatorIndex = fullClassName.LastIndexOf(".");
					this.className = fullClassName[(separatorIndex + 1)..];
					this.nameSpaceName = fullClassName[0..separatorIndex];
				}
				{
					var directoryPath = AssetDatabase.GetAssetPath(directory);
					this.srcPath = $"{directoryPath}/{this.className}.cs";
				}
			}
		}
#endif

	}
}
