using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	[System.Serializable]
	public class Config : ScriptableObject {
		public static Config instance;
		public Screen screen = new Screen();

		[System.Serializable]
		public class Screen {
			public Vector2 size = new Vector2( 320f, 480f );
			public Vector2 main = new Vector2( 320f, 160f );
			public Vector2 sub = new Vector2( 320f, 160f );
		}
	}
}
