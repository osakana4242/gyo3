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

		public void AddDamage(float damage, Chara from) {
			data.hp -= damage;
			if (from.data.layer == Layer.PlayerBullet) {
				Main.Instance.playerInfo.score += 10;
			}
			if (0 < data.hp) return;
			GameObject.Destroy(gameObject);
			if (data.hasBlast) {
				if (from.data.layer == Layer.PlayerBullet) {
					Main.Instance.playerInfo.score += 100;
				}
				var eft1 = GameObject.Instantiate(ResourceService.Instance.Get<GameObject>("eft_blast_01.prefab"), gameObject.transform.position, Quaternion.identity);
				GameObject.Destroy(eft1, 1f);
			}
		}

		public void ManualUpdate() {
			data.position = transform.position;
			if (data.spawnedTime <= 0f) {
				data.spawnedTime = Stage.Current.time.time;
				data.spawnedPosition = data.position;
			}

			switch (data.aiName) {
				case "enemy_test_rotation":
				CharaAI.UpdateEnemyRotationTest(this);
				break;
				case "enemy_1":
				CharaAI.UpdateEnemy1(this);
				break;
				case "enemy_2":
				CharaAI.UpdateEnemy2(this);
				break;
				case "enemy_3":
				CharaAI.UpdateEnemy3(this);
				break;
				case "enemy_4":
				CharaAI.UpdateEnemy4(this);
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

			// 右 z
			// 90: 上
			// 0: 正面
			// -90: 下

			// 左 z
			// -90: 上
			// 0: 正面
			// 90: 下

			var rot = Quaternion.Euler(0f, 0f, euler.x * (flipX ? 1f : -1f));

			transform.localRotation = rot;
		}


	}
}