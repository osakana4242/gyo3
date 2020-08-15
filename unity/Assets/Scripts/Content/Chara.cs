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

		public void ManualUpdate() {
			data.position = transform.position;
			if (data.spawnedTime <= 0f) {
				data.spawnedTime = Stage.Current.time.time;
			}

			switch (data.aiName) {
				case "enemy_1":
				CharaAI.UpdateEnemy1(this);
				break;
				case "enemy_2":
				CharaAI.UpdateEnemy2(this);
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

		public void ApplyTransform() {
			transform.position = data.position;
			var euler = data.rotation.eulerAngles;
			// 画像は右向きを基準にしてる.
			// 0: 奥
			// 90: 右
			// 180: 手前
			// 270: 左
			var flipX = 180f <= euler.y && euler.y < 360f;

			var scale = transform.localScale;
			scale.x = Mathf.Abs(scale.x) * (flipX ? -1f : 1f);
			transform.localScale = scale;

			var rot = Quaternion.Euler(0f, 0f, euler.x);

			transform.localRotation = rot;
		}


	}
}