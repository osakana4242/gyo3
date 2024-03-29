﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {
	public class CharaFactory {
		public static Chara CreateEffect() {
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.EFT_BLAST_01_PREFAB);
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();
			var go = new GameObject($"eft_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.rotation = Quaternion.LookRotation(Vector3.right);
			chara.data.charaType = CharaType.Effect;
			chara.AddComponent(CharaAIs.Effect.instance_g);

			go.transform.position = new Vector3(0f, 0f, 0f);
			//					player.transform.rotation = Quaternion.LookRotation(Vector3.right);
			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;

			chara.AttachModelTo(prefab);

			return chara;
		}

		public static Chara CreatePlayer() {
			var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.PLAYER_ASSET);
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.Get(info.modelName));
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();
			var go = new GameObject($"player_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.rotation = Quaternion.LookRotation(Vector3.right);
			chara.data.SetInfo(info);


			var player = new Player(chara);
			chara.AddPlayer(player);

			go.transform.position = new Vector3(0f, 0f, 0f);
			//					player.transform.rotation = Quaternion.LookRotation(Vector3.right);
			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;

			chara.AttachModelTo(prefab);

			return chara;
		}

		public static Chara CreateBullet(CharaInfo info) {
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.Get(info.modelName));
			var go = new GameObject($"blt_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.SetInfo(info);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			chara.AttachModelTo(prefab);

			var bullet = new CharaAIs.Bullet();
			bullet.speed = info.bullet[0].speed;
			chara.AddComponent(bullet);

			return chara;
		}

		public static Chara CreateEnemy() {
			var charaInfo = AssetService.Instance.Get<CharaInfo>(AssetInfos.ENEMY_1_ASSET);
			return CreateEnemy(charaInfo);
		}

		public static Chara CreateEnemy(CharaInfo info) {
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.Get(info.modelName));
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();

			var go = new GameObject($"enemy_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.SetInfo(info);
			chara.data.rotation = Quaternion.LookRotation(Vector3.left);
			// chara.data.velocity = chara.data.rotation * Vector3.forward * SpeedByScreen(0.25f);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;

			chara.AttachModelTo(prefab);

			go.transform.position = new Vector3(
				InnerMain.Instance.bulletAliveArea.bounds.max.x,
				Random.Range(InnerMain.Instance.screenArea.bounds.min.x, InnerMain.Instance.screenArea.bounds.max.x),
				0f);
			//					enemy.transform.rotation = Quaternion.LookRotation(Vector3.left);
			go.transform.localScale = new Vector3(-1f, 1f, 1f);

			if (!chara.data.aiName.IsEmpty()) {
				var component = chara.data.aiName.createFunc(chara);
				chara.AddComponent(component);
			}

			return chara;
		}

		public static float SpeedByScreen(float s) {
			return s * ScreenAService.Instance.Size.x;
		}
	}

}
