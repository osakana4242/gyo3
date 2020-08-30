using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	[System.Serializable]
	public class Config : ScriptableObject {
		public static Config instance;

		// system
		public Screen screen = new Screen();

		// content
		public TestEnemyWave testEnemy = new TestEnemyWave();

		[System.Serializable]
		public class Screen {
			public Vector2 size = new Vector2(320f, 480f);
			public Vector2 main = new Vector2(320f, 160f);
			public Vector2 sub = new Vector2(320f, 160f);
		}

		[System.Serializable]
		public class TestEnemyWave {
			public bool enabled;
			public string enemyName;
			public Vector2[] positionList = new Vector2[] {
				new Vector2(80f,   0f),
				new Vector2(80f,  40f),
				new Vector2(80f, -40f),
			};
		}
	}
}
