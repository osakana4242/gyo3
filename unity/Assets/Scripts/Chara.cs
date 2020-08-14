using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Chara : MonoBehaviour {
		public CharaParam data = new CharaParam();

		void OnTriggerEnter(Collider col) {
			Debug.Log("self: " + name + ", col: " + col);
		}
		void OnCollisionEnter(Collision collision) {
			CollisionService.Instance.AddCollision(this, collision);
		}

		public void AddDamage(float damage) {
			data.hp -= damage;
			if (0 < data.hp) return;
			GameObject.Destroy(gameObject);
			if (data.hasBlast) {
				var eft1 = GameObject.Instantiate(ResourceService.Instance.eftBlast01Prefab, gameObject.transform.position, Quaternion.identity);
				GameObject.Destroy(eft1, 1f);
			}
		}

		static void UpdateEnemy1(Chara self) {
			const float shot1 = 1f;
			const float shot2 = 1.5f;
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			if (IsEnterTime(shot1, preTime, self.data.stateTime)) {
				ShotToPlayer(self);
			}

			if (IsEnterTime(shot2, preTime, self.data.stateTime)) {
				ShotToPlayer(self);
			}
		}

		static void ShotToPlayer(Chara self) {
			var blt = CharaFactory.CreateBullet2();
			Stage.Current.charaBank.Add(blt);

			blt.transform.position = self.transform.position;
			Vector3 target;
			if (Stage.Current.charaBank.TryGetPlayer(out var player)) {
				target = player.data.position;
			} else {
				target = self.data.position + self.data.rotation * Vector3.forward;
			}
			Vector3 toTarget = (target - blt.transform.position).ToXY0_Ext();
			blt.data.velocity = toTarget.normalized * 60f;
			blt.data.rotation = Quaternion.LookRotation(toTarget);
			blt.transform.rotation = Quaternion.Euler(0f, 0f, 180f + blt.data.rotation.eulerAngles.x);
		}

		static bool IsEnterTime(float target, float pre, float current) {
			return pre < target && target <= current;
		}

		public void ManualUpdate() {
			data.position = transform.position;
			if (data.spawnedTime <= 0f) {
				data.spawnedTime = Stage.Current.time.time;
			}

			switch (data.aiName) {
				case "enemy_1":
				UpdateEnemy1(this);
				break;
			}

			// 移動.
			var delta = data.velocity * Stage.Current.time.dt;
			data.position += delta;

			if (data.hasDeadArea) {
				// エリア外.
				var area = Main.Instance.bulletAliveArea;
				var a = area.bounds;
				var b = GetComponentInChildren<Collider>().bounds;

				if (!a.Intersects(b)) {
					GameObject.Destroy(gameObject);
				}
			}

			ApplyTransform();
		}

		void ApplyTransform() {
			transform.position = data.position;
		}


	}
}