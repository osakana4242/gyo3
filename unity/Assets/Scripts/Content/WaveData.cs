using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEnginUtil;

namespace Osakana4242.Content {
	[System.Serializable]
	public class WaveData : ScriptableObject {
		public WaveEventData[] eventList;
		// 160, 120
		// 16 * 10 = 160
		// 16 * 8 = 128
		// 16 * 9 = 144
		// center = 0, 0
		// spawn right = 5, 0
		// spawn left = -5, 0
		// spawn top = 0, 5
		// spawn bottom = 0, -5
		// 
		public static WaveEventData[] hoge() {
			object[] data = {
				 3000, "enemy1", 5,  3, 180,
				 3500, "enemy1", 5,  3, 180,
				 4000, "enemy1", 5,  3, 180,
				 4500, "enemy1", 5,  3, 180,

				 6000, "enemy1", 5, -3, 180,
				 6500, "enemy1", 5, -3, 180,
				 7000, "enemy1", 5, -3, 180,
				 7500, "enemy1", 5, -3, 180,

				 9000, "enemy1", 5,  2, 180,
				 9500, "enemy1", 5,  1, 180,
				10000, "enemy1", 5, -1, 180,
				10500, "enemy1", 5, -2, 180,
			};
			List<WaveEventData> list = new List<WaveEventData>();
			for (int i = 0, iCount = data.Length; i < iCount; i += 5) {
				var item = new WaveEventData() {
					startTime = (int)data[i + 0],
					enemyName = (string)data[i + 1],
					position = new Vector2((int)data[i + 2], (int)data[i + 3]),
					angle = (int)data[i + 4],
				};
				list.Add(item);
			}
			return list.ToArray();
		}
	}
}
