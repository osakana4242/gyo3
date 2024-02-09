using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;
using System.ComponentModel;

namespace Osakana4242.Content.Inners {

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
		public float duration = float.MaxValue;
		public float stateTime = 0f;
		public int state;
		public bool removeRequested;
		public Vector3[] vector3s = new Vector3[4];

		public int Layer => charaType.GetLayer();
		public List<OnCharaEventDelegate> observerList = new List<OnCharaEventDelegate>();

		public void FireEvent(CharaEventType type) {
			for (var i = observerList.Count - 1; 0 <= i; --i) {
				var item = observerList[i];
				var isContinuing = item(type);
				if (isContinuing)
					continue;
				observerList.RemoveAt(i);
			}
		}

		public void SetInfo(CharaInfo info) {
			this.charaType = info.charaType;
			this.hpMax = new HitPointMax(info.hp);
			this.hp = new HitPoint(this.hpMax.value);
			this.hasBlast = info.hasBlast;
			this.hasDeadArea = info.hasDeadArea;
			this.aiName = new AIName(info.aiName);
		}

		public void SetSortingOrderTo(GameObject go) {
			go.transform.ForEachChildWithSelf_Ext(charaType.GetSortingOrder(), (_tr, _prm) => {
				if (!_tr.TryGetComponent<Renderer>(out var r)) return;
				r.sortingOrder = _prm;
			});
		}

		public void SetLayerTo(GameObject go) {
			go.transform.ForEachChildWithSelf_Ext(Layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
		}
	}

	public delegate bool OnCharaEventDelegate(CharaEventType type);

	public enum CharaEventType {
		None,
		Added,
		Removed,
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
