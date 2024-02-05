
using System;
using UnityEngine;

namespace Osakana4242.Content.Inners {
	public class CharaInfo : ScriptableObject {
		public CharaType charaType;
		public string comment;
		public int hp;
		public bool hasDeadArea;
		public bool hasBlast;
		public string modelName;
		public string aiName;
		public Bullet[] bullet;

		[Serializable]
		public class Bullet {
			public float speed;
		}
	}
}
