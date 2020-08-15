using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	[System.Serializable]
	public class CharaParam {
		public int id;
		// player, bulletA, bulletB, enemy1
		//public int type;
		public int layer;
		public float hp;
		public float hpMax;
		public bool hasDeadArea;
		public bool hasBlast;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 velocity;
		public string modelName;
		public int ownerCharaId;
		public string aiName;
		public float spawnedTime;
		public float stateTime = -1f;
		public int state;
	}



	public struct Rotation2D {
		public float value;
		public static Rotation2D From(Vector2 vector) {
			return new Rotation2D() {
				value = Vector2.Angle(new Vector2(1f, 0f), vector)
			};
		}

		public Vector2 ToVector2() {
			var rad = value * Mathf.Rad2Deg;
			var vec = new Vector2(
				Mathf.Cos(rad),
				Mathf.Sin(rad)
			);
			return vec;
		}
	}
}