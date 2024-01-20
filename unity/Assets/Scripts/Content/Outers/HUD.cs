using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Outers {
	[System.Serializable]
	public class HUD {
		public TMPro.TextMeshProUGUI ui;
		public void Update() {
			var playerInfo = Main.Instance.inner.playerInfo;
			var score = playerInfo.score;
			ui.text = FormatForContentFont(string.Format("SC {0} POW {1}", score, playerInfo.weaponChargeProgress.ToPercentString() ));
		}

		public static string FormatForContentFont(string str) {
			// 美咲フォント用.
			return str.ToZenkaku_Ext();
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
