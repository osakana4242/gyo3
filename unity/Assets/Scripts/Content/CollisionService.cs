using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class CollisionService {
		static CollisionService instance_;
		public static CollisionService Instance => instance_;

		public static void Init() {
			instance_ = new CollisionService();
		}

		public void AddCollision(Chara chara, Collision collision) {
			var other = collision.gameObject.GetComponent<Chara>();
			if (other == null) return;
			chara.AddDamage(1f, other);
			// other.AddDamage(1f);
		}

		public void Update() {
		}
	}
}
