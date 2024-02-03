using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {
	public class CharaFactory {
		public static Chara CreatePlayer() {
			var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.PLAYER_ASSET);
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.Get(info.modelName));
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();
			var go = new GameObject($"player_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.rotation = Quaternion.LookRotation(Vector3.right);
			chara.data.SetInfo(info);

			GameObject.Instantiate(prefab, go.transform);

			var player = go.AddComponent<Player>();
			go.transform.position = new Vector3(0f, 0f, 0f);
			//					player.transform.rotation = Quaternion.LookRotation(Vector3.right);
			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			go.transform.ForEachChildWithSelf_Ext(chara.data.Layer, (_tr, _layer) => _tr.gameObject.layer = _layer);

			return chara;
		}

		public static Chara CreateBullet(CharaInfo info) {
			var charaId = InnerMain.Instance.stage.charaBank.CreateId();
			var prefab = AssetService.Instance.Get<GameObject>(AssetInfos.Get(info.modelName));
			var go = new GameObject($"blt_{charaId}");
			var chara = go.AddComponent<Chara>();
			chara.data.id = charaId;
			chara.data.SetInfo(info);

			GameObject.Instantiate(prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;

			go.transform.ForEachChildWithSelf_Ext(chara.data.Layer, (_tr, _layer) => _tr.gameObject.layer = _layer);
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
			chara.data.velocity = chara.data.rotation * Vector3.forward * SpeedByScreen(0.25f);

			GameObject.Instantiate(prefab, go.transform);

			var rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			go.transform.ForEachChildWithSelf_Ext(chara.data.Layer, (_tr, _layer) => _tr.gameObject.layer = _layer);


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
