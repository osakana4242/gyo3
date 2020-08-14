using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class CharaFactory {
		public static Chara CreatePlayer() {
			var go = new GameObject("player");
			GameObject.Instantiate(ResourceService.Instance.ply01Prefab, go.transform);
			var chara = go.AddComponent<Chara>();
			chara.data.id = Main.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.Player;
			chara.data.hp = chara.data.hpMax = 3;
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = false;
			go.name = "player_" + chara.data.id;

			var player = go.AddComponent<Player>();
			go.transform.position = new Vector3(0f, 0f, 0f);
			//					player.transform.rotation = Quaternion.LookRotation(Vector3.right);
			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);

			return chara;
		}

		public static Chara CreateBullet() {
			var go = new GameObject("blt");
			var chara = go.AddComponent<Chara>();
			chara.data.id = Main.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.PlayerBullet;
			chara.data.hp = chara.data.hpMax = 1;
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.velocity = new Vector3(SpeedByScreen(2f), 0f, 0f);
			go.name = "blt_" + chara.data.id;


			GameObject.Instantiate(ResourceService.Instance.blt01Prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
			return chara;
		}

		public static Chara CreateBullet2() {
			var go = new GameObject("blt");
			var chara = go.AddComponent<Chara>();
			chara.data.id = Main.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.EnemyBullet;
			chara.data.hp = chara.data.hpMax = 1;
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.velocity = new Vector3(SpeedByScreen(1f), 0f, 0f);
			go.name = "blt_" + chara.data.id;


			GameObject.Instantiate(ResourceService.Instance.blt02Prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
			return chara;
		}

		public static Chara CreateEnemy() {
			var go = new GameObject("enemy");
			var chara = go.AddComponent<Chara>();
			chara.data.id = Main.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.Enemy;
			chara.data.hp = chara.data.hpMax = 1;
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.aiName = "enemy_1";
			go.name = "enemy_" + chara.data.id;
			chara.data.rotation = Quaternion.LookRotation(Vector3.left);
			chara.data.velocity = chara.data.rotation * Vector3.forward * SpeedByScreen(0.25f);

			GameObject.Instantiate(ResourceService.Instance.enm01Prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);


			go.transform.position = new Vector3(
				Main.Instance.bulletAliveArea.bounds.max.x,
				Random.Range(Main.Instance.screenArea.bounds.min.x, Main.Instance.screenArea.bounds.max.x),
				0f);
			//					enemy.transform.rotation = Quaternion.LookRotation(Vector3.left);
			go.transform.localScale = new Vector3(-1f, 1f, 1f);
			return chara;
		}

		public static float SpeedByScreen(float s) {
			return s * Config.instance.screen.main.x;
		}
	}

}