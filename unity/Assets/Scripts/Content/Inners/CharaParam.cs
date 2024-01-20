using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	[System.Serializable]
	public class CharaParam {
		public int id;
		// player, bulletA, bulletB, enemy1
		//public int type;
		public int layer;
		public HitPoint hp;
		public HitPointMax hpMax;
		public bool hasDeadArea;
		public bool hasBlast;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 velocity;
		public string modelName;
		public int ownerCharaId;
		public string aiName;
		public float spawnedTime;
		public Vector3 spawnedPosition;
		public float stateTime = -1f;
		public int state;
	}

	[System.Serializable]
	public struct HitPoint {
		public readonly int value;
		public HitPoint( int value ) {
			this.value = value;
		}

		public HitPoint AddDamge( Damage damage ) {
			return new HitPoint( value - damage.value );
		}

		public bool isEmpty() => value <= 0;
	}

	[System.Serializable]
	public struct Damage {
		public readonly int value;
		public Damage( int value ) {
			this.value = value;
		}
	}

	[System.Serializable]
	public struct HitPointMax {
		public readonly int value;
		public HitPointMax( int value ) {
			this.value = value;
		}
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
