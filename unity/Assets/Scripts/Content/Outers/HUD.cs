using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Cysharp.Text;

namespace Osakana4242.Content.Outers {
	[System.Serializable]
	public class HUD {
		public TMPro.TextMeshProUGUI ui;
		public UnityEngine.UI.Button exitBtn;

		public void Init() {
			exitBtn.onClick.AddListener(() => {
				if (!Main.Instance.inner.CanExit()) return;
				Main.Instance.inner.hasExitRequest = true;
			});
		}
		public void Update() {
			exitBtn.gameObject.SetActive(Main.Instance.inner.CanExit());
			var playerInfo = Main.Instance.inner.playerInfo;
			var score = playerInfo.score;
			UnityEngine.Profiling.Profiler.BeginSample("HOGE");
			var sb = ZString.CreateStringBuilder();
			try {
				sb.AppendFormat("SC {0} POW ", score.value);
				playerInfo.weaponChargeProgress.WritePercentStringTo(ref sb);
				FormatForContentFont(ref sb);
				ui.SetText(sb);
			} finally {
				sb.Dispose();
			}
			UnityEngine.Profiling.Profiler.EndSample();

		}

		public static void FormatForContentFont(ref Utf16ValueStringBuilder sb) {
			// 美咲フォント用.
			sb.ToZenkaku_Ext();
		}

		public static string FormatForContentFont(string str) {
			// 美咲フォント用.
			return str.ToZenkaku_Ext();
		}
	}

	public static class Utf16ValueStringBuilderExt {
		public static void ToZenkaku_Ext(this ref Utf16ValueStringBuilder sb) {
			var span = sb.AsSpan();
			for (int i = 0, iCount = span.Length; i < iCount; ++i) {
				char c = span[i];
				char c2;
				if ('0' <= c && c <= '9') {
					c2 = (char)('０' + c - '0');
				} else if ('A' <= c && c <= 'Z') {
					c2 = (char)('Ａ' + c - 'A');
				} else if (c == ' ') {
					c2 = '　';
				} else {
					c2 = c;
				}
				sb.ReplaceAt(c2, i);
			}
		}
	}

	public static class StringExt {
		public static string ToZenkaku_Ext(this string str) {
			var sb = new System.Text.StringBuilder(str.Length);
			for (int i = 0, iCount = str.Length; i < iCount; ++i) {
				char c = str[i];
				char c2;
				if ('0' <= c && c <= '9') {
					c2 = (char)('０' + c - '0');
				} else if ('A' <= c && c <= 'Z') {
					c2 = (char)('Ａ' + c - 'A');
				} else if (c == ' ') {
					c2 = '　';
				} else {
					c2 = c;
				}
				sb.Append(c2);
			}
			return sb.ToString();
		}
	}

}
