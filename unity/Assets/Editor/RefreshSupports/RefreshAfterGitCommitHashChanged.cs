
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RefreshSupports {
	/// <summary>
	/// フォーカスの切り替え時にコミットハッシュの変更を検知したら Refresh を促す。
	/// Auto Refresh がオフのとき限定。
	/// </summary>
	[InitializeOnLoad]
	sealed class RefreshAfterGitCommitHashChanged {
		static bool active_;

		static RefreshAfterGitCommitHashChanged() {
			// Debug.Log($"{nameof(RefreshAfterGitCommitHashChanged)}, isPlayingOrWillChangePlaymode: {EditorApplication.isPlayingOrWillChangePlaymode}, AutoRefreshDisabled: {AutoRefreshDisabled()}");
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			EditorApplication.update += Update;
		}

		static bool Enabled => AutoRefreshDisabled();

		static bool AutoRefreshDisabled() {
#if UNITY_2022_2_OR_NEWER
			// https://unity.com/ja/releases/editor/whats-new/2022.2.0#notes
			// > Asset Pipeline: Removed the option to Recompile after finished playing and added a new option to only auto refresh outside playmode. (1325047)
			const string keyAutoRefreshMode = "kAutoRefreshMode_h2874646975";
			if (EditorPrefs.HasKey(keyAutoRefreshMode)) {
				// 0: Disabled
				// 1: Enabled
				// 2: Enabled Outside Playmode
				var autoRefreshMode = EditorPrefs.GetInt(keyAutoRefreshMode);
				return autoRefreshMode == 0;
			}
#else
			const string keyAutoRefresh = "kAutoRefresh_h2404942332";
			if (EditorPrefs.HasKey(keyAutoRefresh)) {
				var autoRefresh = EditorPrefs.GetBool(keyAutoRefresh);
				return !autoRefresh;
			}
#endif
			return false;
		}

		static (GitCommitInfo prev, GitCommitInfo current) WriteCommitHashIfNeeded() {
			if (!Enabled) {
				return (default, default);
			}
			var path = $"Temp/{nameof(RefreshAfterGitCommitHashChanged)}_commit_hash.txt";
			var prev = default(GitCommitInfo);
			if (System.IO.File.Exists(path)) {
				var lines = System.IO.File.ReadAllLines(path);
				var commitHash = (lines.Length <= 0) ? default : new GitCommitHash(lines[0]);
				var branch     = (lines.Length <= 1) ? default : lines[1];
				prev = new GitCommitInfo(commitHash, branch);
			}
			var current = prev;
			{
				if (GitCommand.TryGetCommitHash(out var commitHash)) {
					GitCommand.TryGetBranch(out var branch);
					current = new GitCommitInfo(commitHash, branch);
				}
			}
			if (prev.commitHash.value != current.commitHash.value) {
				var text =
					$"{current.commitHash.value}\n" +
					$"{current.branch}\n";
				System.IO.File.WriteAllText(path, text);
			}
			return (prev, current);
		}

		static void ConfirmRefreshIfNeeded() {
			if (!Enabled) {
				return;
			}
			var r = WriteCommitHashIfNeeded();
			if (r.prev.commitHash.IsEmpty()) {
				return;
			}
			if (r.prev.commitHash.value == r.current.commitHash.value) {
				return;
			}
			var isOk = EditorUtility.DisplayDialog(
				title: $"{nameof(RefreshAfterGitCommitHashChanged)}",
				message: $"git コミットハッシュの変更を検知しました。\n" +
				$"\n" +
				$"前回: {r.prev.toOneline()}\n" +
				$"今回: {r.current.toOneline()}\n" +
				$"\n" +
				$"Refresh します。",
				ok: "OK",
				cancel: "Cancel");
			if (!isOk) {
				return;
			}
			Debug.Log($"Refresh By {nameof(RefreshAfterGitCommitHashChanged)}");
			AssetDatabase.Refresh();
		}

		static void Update() {
			var a = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
			if (active_ == a) {
				return;
			}
			active_ = a;
			if (active_) {
				OnFocus();
			} else {
				OnUnfocus();
			}
		}

		static void OnFocus() {
			if (EditorApplication.isPlaying) {
				return;
			}
			ConfirmRefreshIfNeeded();
		}

		static void OnUnfocus() {
			if (EditorApplication.isPlaying) {
				return;
			}
			WriteCommitHashIfNeeded();
		}

		static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange) {
			switch (playModeStateChange) {
			case PlayModeStateChange.ExitingEditMode:
				WriteCommitHashIfNeeded();
				break;
			case PlayModeStateChange.EnteredEditMode:
				ConfirmRefreshIfNeeded();
				break;
			}
		}

		static class GitCommand {
			public static bool TryGetCommitHash(out GitCommitHash commitHash) {
				var gitCommand = "show --format='%H' --no-patch";
				if (!TryGetStandardOutputFromProcess(gitCommand, out var result)) {
					commitHash = default;
					return false;
				}
				commitHash = new GitCommitHash(result);
				return true;
			}

			public static bool TryGetBranch(out string branch) {
				// "branch --show-current" は git version 2.22.0以降から対応.
				// "rev-parse ..." の方が確実.
				var gitCommand = "rev-parse --abbrev-ref HEAD";
				if ( !TryGetStandardOutputFromProcess(gitCommand, out var str) ) {
					branch = default;
					return false;
				}
				branch = str.Trim().Trim('\'', '\n');
				return true;
			}

			static string GetGitPath() {
				var exePaths = System.Array.Empty<string>();
				switch (Application.platform) {
				case RuntimePlatform.OSXEditor:
					exePaths = new string[] {
						"/usr/local/bin/git",
						"/usr/bin/git"
					};
					break;
				case RuntimePlatform.WindowsEditor:
					exePaths = new string[] {
						@$"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}\AppData\Local\Atlassian\SourceTree\git_local\bin\git.exe",
						"git",
					};
					break;
				}
				return exePaths.FirstOrDefault(exePath => System.IO.File.Exists(exePath));
			}

			static bool TryGetStandardOutputFromProcess(string arguments, out string output) {
				var exePath = GetGitPath();
				if (string.IsNullOrEmpty(exePath)) {
					output = default;
					return false;
				}
				var startInfo = new System.Diagnostics.ProcessStartInfo() {
					FileName = exePath,
					Arguments = arguments,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
				};

				using (var process = System.Diagnostics.Process.Start(startInfo)) {
					output = process.StandardOutput.ReadToEnd();
					const int TimeoutPeriod = 3000;
					if (!process.WaitForExit(TimeoutPeriod)) {
						return false;
					}
					return true;
				}
			}
		}

		readonly struct GitCommitHash {
			public readonly string value;

			public GitCommitHash(string value) {
				var sanytized = value.Trim().Trim('\'', '\n');
				if (sanytized.Length != 40) {
					Debug.LogException(new System.ArgumentException($"value: {value}"));
					this.value = default;
					return;
				}
				this.value = sanytized;
			}

			public bool IsEmpty() => string.IsNullOrEmpty(value);

			public string ShortHash => string.IsNullOrEmpty(value) ?
				"" :
				value.Substring(0, 8);

			public override string ToString() => value;
		}

		readonly struct GitCommitInfo {
			public readonly GitCommitHash commitHash;
			public readonly string branch;

			public GitCommitInfo(GitCommitHash commitHash, string branch) {
				this.commitHash = commitHash;
				this.branch = branch;
			}

			public string toOneline() => string.IsNullOrEmpty( branch ) ?
				$"{commitHash.ShortHash}" :
				$"{commitHash.ShortHash} [{branch}]";
		}
	}
}
