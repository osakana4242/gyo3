using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Main : MonoBehaviour {
		static Main instance_;
		[SerializeField] public new Camera camera;
		[SerializeField] public BoxCollider bulletAliveArea;
		[SerializeField] public BoxCollider screenArea;
		public Wave wave;

		void Awake() {
			instance_ = this;
			wave = new Wave();
		}

		public static Main Instance => instance_;

		void Start() {
			Physics.reuseCollisionCallbacks = true;
			ScreenService.Init();
			ResourceService.Init();

			{
					var player = new GameObject("player");
					GameObject.Instantiate(ResourceService.Instance.ply01Prefab, player.transform);
					var bullet = player.AddComponent<Player>();
					player.transform.position = new Vector3(0f, 0f, 0f);
//					player.transform.rotation = Quaternion.LookRotation(Vector3.right);
					var rb = player.AddComponent<Rigidbody>();
					rb.isKinematic = false;
					rb.constraints = RigidbodyConstraints.FreezeAll;
					rb.useGravity = false;
					player.transform.ForEachChildWithSelf_Ext(Layer.Player, (_tr, _layer) => _tr.gameObject.layer = _layer);
			}

		}

		void FixedUpdate() {
			wave.Update();
		}

		public class Layer {
			public static int Player = UnityEngine.LayerMask.NameToLayer("Player");
			public static int PlayerBullet = UnityEngine.LayerMask.NameToLayer("PlayerBullet");
			public static int Enemy = UnityEngine.LayerMask.NameToLayer("Enemy");
			public static int EnemyBullet = UnityEngine.LayerMask.NameToLayer("EnemyBullet");

		}

		public class Wave {
			public float time;
			public float interval = 0.5f;
			public void Update() {
				if (time < interval) {
					time += Time.deltaTime;
				} else {
					time = 0f;
					var enemy = new GameObject("enemy");
					GameObject.Instantiate(ResourceService.Instance.enm01Prefab, enemy.transform);
					var bullet = enemy.AddComponent<Bullet>();
					bullet.velocity.x = 60f * -2;
					enemy.transform.position = new Vector3(
						Main.Instance.bulletAliveArea.bounds.max.x,
						Random.Range(Main.Instance.screenArea.bounds.min.x, Main.Instance.screenArea.bounds.max.x),
						0f);
//					enemy.transform.rotation = Quaternion.LookRotation(Vector3.left);
					enemy.transform.localScale = new Vector3(-1f, 1f, 1f);
					var rb = enemy.AddComponent<Rigidbody>();
					rb.isKinematic = false;
					rb.constraints = RigidbodyConstraints.FreezeAll;
					rb.useGravity = false;
					enemy.transform.ForEachChildWithSelf_Ext(Layer.Enemy, (_tr, _layer) => _tr.gameObject.layer = _layer);
				}
			}
		}
	}
}
