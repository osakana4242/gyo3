
using UnityEngine;
using UnityEditor;

namespace RefreshSupports {
	/// <summary>
	/// プレイモードに入る前に Refresh をする。
	/// Auto Refresh がオフのとき限定。
	/// </summary>
	[InitializeOnLoad]
	sealed class RefreshInExitingEditMode {
		static RefreshInExitingEditMode() {
			// Debug.Log($"{nameof(RefreshInExitingEditMode)}, isPlayingOrWillChangePlaymode: {EditorApplication.isPlayingOrWillChangePlaymode}, AutoRefreshDisabled: {AutoRefreshDisabled()}");
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
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
				return autoRefresh;
			}
#endif
			return false;
		}

		static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange) {
			if (!Enabled) {
				return;
			}
			switch (playModeStateChange) {
			case PlayModeStateChange.ExitingEditMode:
				Debug.Log($"Refresh By {nameof(RefreshInExitingEditMode)}");
				AssetDatabase.Refresh();
				break;
			}
		}
	}
}
