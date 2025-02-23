﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content.Inners {
	public class CollisionService {
		static CollisionService instance_;
		public static CollisionService Instance => instance_;

		public static void Init() {
			instance_ = new CollisionService();
		}

		public void AddCollision(Chara chara, Collision collision) {
			if (!collision.gameObject.TryGetComponent<Chara>(out var other)) return;
			chara.AddDamage(new Damage(1), other);
			// other.AddDamage(1f);
		}

		public void Update() {
		}
	}
}
