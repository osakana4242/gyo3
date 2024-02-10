using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {
	public interface ICharaComponent {
		void Update(Chara chara);
	}

	public class Chara : MonoBehaviour {
		public CharaParam data = new CharaParam();
		readonly List<ICharaComponent> actionList_ = new List<ICharaComponent>();
		Player player_;
		public Player Player => player_;
		public CharaAIs.Bullet Bullet => (CharaAIs.Bullet)actionList_[ 0 ];

		void OnDestroy() {
			data.observerList.Clear();
		}

		public void AddPlayer(Player player) {
			player_ = player;
			AddComponent(player);
		}

		public void AddComponent(ICharaComponent comp) {
			actionList_.Add(comp);
		}


		void OnTriggerEnter(Collider col) {
			Debug.Log("self: " + name + ", col: " + col);
		}
		void OnCollisionEnter(Collision collision) {
			CollisionService.Instance.AddCollision(this, collision);
		}

		public void AddDamage(Damage damage, Chara from) {
			data.hp = data.hp.AddDamge(damage);
			if (from.data.charaType == CharaType.PlayerBullet) {
				InnerMain.Instance.playerInfo.score += new AddScore(10);
			}
			if (!data.hp.isEmpty()) return;
			data.removeRequested = true;
			if (data.hasBlast) {
				if (from.data.charaType == CharaType.PlayerBullet) {
					InnerMain.Instance.playerInfo.score += new AddScore(100);
				}
				var eft = CharaFactory.CreateEffect();
				eft.data.position = data.position;
				InnerMain.Instance.stage.charaBank.Add(eft);
			}
		}

		public void ManualUpdate() {
			data.position = transform.position;
			if (data.spawnedTime <= Msec.Zero) {
				data.spawnedTime = Stage.Current.time.time - Stage.Current.time.dt;
				data.spawnedPosition = data.position;
			}

			foreach (var item in actionList_) {
				item.Update(this);
			}

			// 移動.
			var delta = data.velocity * Stage.Current.time.dt.PerSecToPerThis;
			data.position += delta;

			if (data.hasDeadArea) {
				// エリア外.
				var area = InnerMain.Instance.bulletAliveArea;
				var a = area.bounds;
				var b = GetComponentInChildren<Collider>().bounds;

				if (!a.Intersects(b)) {
					data.removeRequested = true;
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

		public void AttachModelTo(GameObject prefab) {
			var go = gameObject;
			var model = new GameObject("model");
			model.transform.SetParent(go.transform, false);

			GameObject.Instantiate(prefab, model.transform);

			data.SetLayerTo(go);
			data.SetSortingOrderTo(go);
		}

	}
}
