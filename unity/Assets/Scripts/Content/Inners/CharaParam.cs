using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	public sealed class AIName {
		Dictionary<HashKey, System.Action<Chara>> updateFuncs_g_ = new Dictionary<HashKey, System.Action<Chara>> {
			{ "enemy_test_rotation".ToHashKey_Ext(), CharaAIs.EnemyTestRoation.Update },
			{ "enemy_1".ToHashKey_Ext(), CharaAIs.Enemy1.Update },
			{ "enemy_2".ToHashKey_Ext(), CharaAIs.Enemy2.Update },
			{ "enemy_3".ToHashKey_Ext(), CharaAIs.Enemy3.Update },
			{ "enemy_4".ToHashKey_Ext(), CharaAIs.Enemy4.Update },
		};
		static readonly System.Action<Chara> emptyFunc_g_ = (_) => { };
		public static readonly AIName Empty = new AIName();

		public readonly string name = "";
		public readonly System.Action<Chara> updateFunc = emptyFunc_g_;

		AIName() {
		}
		public AIName(string name) {
			if ( string.IsNullOrEmpty( name ) )
				return;
			this.name = name;
			if (!updateFuncs_g_.TryGetValue(name.ToHashKey_Ext(), out var func)) throw new System.ArgumentException($"not found. {name}");
			this.updateFunc = func;
		}

		public bool IsEmpty() => string.IsNullOrEmpty(name);
	}

	[System.Serializable]
	public class CharaParam {


		public int id;
		public CharaType charaType;
		public HitPoint hp;
		public HitPointMax hpMax;
		public bool hasDeadArea;
		public bool hasBlast;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 velocity;
		public string modelName;
		public int ownerCharaId;
		public AIName aiName = AIName.Empty;
		public float spawnedTime;
		public Vector3 spawnedPosition;
		public float stateTime = -1f;
		public int state;
		public bool removeRequested;

		public int Layer => charaType.ToLayer();

		public void SetInfo(CharaInfo info) {
			this.charaType = info.charaType;
			this.hpMax = new HitPointMax( info.hp );
			this.hp = new HitPoint( this.hpMax.value );
			this.hasBlast = info.hasBlast;
			this.hasDeadArea = info.hasDeadArea;
			this.aiName = new AIName( info.aiName );
		}
	}

	[System.Serializable]
	public struct HitPoint {
		public readonly int value;
		public HitPoint(int value) {
			this.value = value;
		}

		public HitPoint AddDamge(Damage damage) {
			return new HitPoint(value - damage.value);
		}

		public bool isEmpty() => value <= 0;
	}

	[System.Serializable]
	public struct Damage {
		public readonly int value;
		public Damage(int value) {
			this.value = value;
		}
	}

	[System.Serializable]
	public struct HitPointMax {
		public readonly int value;
		public HitPointMax(int value) {
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
