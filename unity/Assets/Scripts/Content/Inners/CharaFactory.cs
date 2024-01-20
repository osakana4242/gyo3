﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	public class CharaFactory {
		public static Chara CreatePlayer() {
			var go = new GameObject("player");
			GameObject.Instantiate(ResourceService.Instance.Get<GameObject>(ResourceNames.PLY_01_PREFAB), go.transform);
			var chara = go.AddComponent<Chara>();
			chara.data.id = InnerMain.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.Player;
			chara.data.hpMax = new HitPointMax( 3 );
			chara.data.hp = new HitPoint( chara.data.hpMax.value );
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = false;
			chara.data.rotation = Quaternion.LookRotation(Vector3.right);
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
			chara.data.id = InnerMain.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.PlayerBullet;
			chara.data.hpMax = new HitPointMax( 1 );
			chara.data.hp = new HitPoint( chara.data.hpMax.value );
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.velocity = new Vector3(SpeedByScreen(2f), 0f, 0f);
			go.name = "blt_" + chara.data.id;


			GameObject.Instantiate(ResourceService.Instance.Get<GameObject>(ResourceNames.BLT_01_PREFAB), go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
			return chara;
		}

		public static Chara CreateBullet2() {
			var go = new GameObject("blt");
			var chara = go.AddComponent<Chara>();
			chara.data.id = InnerMain.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.EnemyBullet;
			chara.data.hpMax = new HitPointMax( 1 );
			chara.data.hp = new HitPoint( chara.data.hpMax.value );
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.velocity = new Vector3(SpeedByScreen(1f), 0f, 0f);
			go.name = "blt_" + chara.data.id;


			GameObject.Instantiate(ResourceService.Instance.Get<GameObject>(ResourceNames.BLT_02_PREFAB), go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
			return chara;
		}

		public static Chara CreateEnemy() {
			return CreateEnemy("enemy_1");
		}

		public static Chara CreateEnemy(string aiName) {
			var go = new GameObject("enemy");
			var chara = go.AddComponent<Chara>();
			chara.data.id = InnerMain.Instance.stage.charaBank.CreateId();
			chara.data.layer = Layer.Enemy;
			HitPointMax hpMax;
			GameObject prefab;
			switch (aiName) {
				default:
				case "enemy_1":
				prefab = ResourceService.Instance.Get<GameObject>(ResourceNames.ENM_01_PREFAB);
				hpMax = new HitPointMax( 1 );
				break;

				case "enemy_2":
				prefab = ResourceService.Instance.Get<GameObject>(ResourceNames.PLY_01_PREFAB);
				hpMax = new HitPointMax( 20 );
				break;

				case "enemy_3":
				prefab = ResourceService.Instance.Get<GameObject>(ResourceNames.ENM_01_PREFAB);
				hpMax = new HitPointMax( 1 );
				break;
			}
			chara.data.hpMax = hpMax;
			chara.data.hp = new HitPoint( chara.data.hpMax.value );
			chara.data.hasBlast = true;
			chara.data.hasDeadArea = true;
			chara.data.aiName = aiName;
			go.name = "enemy_" + chara.data.id;
			chara.data.rotation = Quaternion.LookRotation(Vector3.left);
			chara.data.velocity = chara.data.rotation * Vector3.forward * SpeedByScreen(0.25f);

			GameObject.Instantiate(prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			go.transform.ForEachChildWithSelf_Ext(chara.data.layer, (_tr, _layer) => _tr.gameObject.layer = _layer);


			go.transform.position = new Vector3(
				InnerMain.Instance.bulletAliveArea.bounds.max.x,
				Random.Range(InnerMain.Instance.screenArea.bounds.min.x, InnerMain.Instance.screenArea.bounds.max.x),
				0f);
			//					enemy.transform.rotation = Quaternion.LookRotation(Vector3.left);
			go.transform.localScale = new Vector3(-1f, 1f, 1f);
			return chara;
		}

		public static float SpeedByScreen(float s) {
			return s * ScreenAService.Instance.Size.x;
		}
	}

}