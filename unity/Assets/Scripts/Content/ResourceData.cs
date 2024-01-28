using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
namespace Osakana4242.Content {
	public class ResourceData : ScriptableObject {
		// public List<Holder> assetList = new List<Holder>();

		// public T Get<T>(AssetInfo name) where T : Object {
		// 	var res = Resources.Load<T>(name.resourcesName.value);
		// 	if (null == res)
		// 		throw new System.Exception($"リソースが見つからない. name: '{name}'");
		// 	return res;
		// }

		public async Task<T> GetAsync<T>(AssetInfo name, CancellationToken cancellationToken) where T : Object {
			await Task.Delay(1000);
			cancellationToken.ThrowIfCancellationRequested();
			var req = Resources.LoadAsync<T>(name.resourcesName.value);
			while (req.isDone) {
				await Task.Delay(1);
				cancellationToken.ThrowIfCancellationRequested();
			}
			var asset = req.asset;
			if (null == asset)
				throw new System.Exception($"リソースが見つからない. name: '{name}'");
			var castedAsset = (T)asset;
			return castedAsset;
		}

#if UNITY_EDITOR
		public UnityEditor.DefaultAsset targetDirectory;

		[UnityEngine.ContextMenu("RefleshAssetList")]
		void RefleshAssetList() {
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

			WriteResouceList(resourceInfoList);
		}

		public static void WriteResouceList(List<AssetInfo> infoList) {
			var tmpl = @$"
	namespace Osakana4242.Content {{
		public static class {nameof(AssetInfos)} {{
{0}
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
			var src = string.Format(tmpl, sb);
			src = src.Replace("\r\n", "\n");
			var srcPath = $"Assets/Scripts/Content/{nameof(AssetInfos)}.cs";
			System.IO.File.WriteAllText(srcPath, src, System.Text.Encoding.UTF8);
			UnityEditor.AssetDatabase.Refresh();
		}
#endif

	}
	public readonly struct AssetInfo {
		public readonly string fileName;
		public readonly string pathFromAssets;
		public readonly System.Type type;
		public readonly ResourcesName resourcesName;
		public readonly string fileNameWithoutExtention;

		public AssetInfo(string path, System.Type type) {
			this.pathFromAssets = path;
			this.fileName = System.IO.Path.GetFileName(path);
			this.fileNameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(this.fileName);
			this.resourcesName = new ResourcesName(path);
			this.type = type;
		}

		public bool IsDirectory() => type == typeof(UnityEditor.DefaultAsset);
		public override string ToString() => $"{pathFromAssets}";
		public string AnimationClipName { get {
			if ( type != typeof( AnimationClip ) )
				throw new System.Exception( $"{nameof(AnimationClip)} ではない. pathFromAssets: {pathFromAssets}");
			return fileNameWithoutExtention;
		} }

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

	public readonly struct ResourcesName {
		public readonly string value;
		public ResourcesName(string path) {
			string pathFromResources;
			{
				var RESOURCES_PATH = "/Resources/";
				var index = path.LastIndexOf(RESOURCES_PATH, System.StringComparison.Ordinal);
				if (0 <= index) {
					pathFromResources = path[(index + RESOURCES_PATH.Length)..];
				} else {
					pathFromResources = path;
				}
			}

			{
				var index = pathFromResources.IndexOf(".", System.StringComparison.Ordinal);
				if (0 <= index) {
					value = pathFromResources[..index];
				} else {
					value = pathFromResources;
				}
			}
		}

		public override string ToString() => value;
	}
}
