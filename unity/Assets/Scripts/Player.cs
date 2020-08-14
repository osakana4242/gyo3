using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Player : MonoBehaviour {
		void Start() {

		}

		Vector2 pressPos_;
		Vector3 anchorPos_;
		Vector3 cpos0_;
		bool pressed_;
		public bool hasShotInput;
		public float shotInterval = 0.05f;
		public float shotTime;

		void UpdateShot(Chara chara) {
			if (!hasShotInput) {
				shotTime = 0f;
				return;
			}

			if (shotTime < shotInterval) {
				shotTime += Time.deltaTime;
			} else {
				shotTime = 0f;
				var blt = CharaFactory.CreateBullet();
				Main.Instance.stage.charaBank.Add(blt);
				blt.transform.position = chara.data.position + new Vector3(0f, Random.Range(-2f, 2f), 0f);
			}
		}

		struct InputItem {
			public float time;
			public Vector2 dir;
		}

		public enum Dir8 {
			None,
			Up,
			UpRight,
			Right,
			DownRight,
			Down,
			DownLeft,
			Left,
			UpLeft,
		}
		List<InputItem> inputHistory_ = new List<InputItem>();
		static bool CheckShotInput(List<InputItem> history) {
			var time = Time.time - 0.4f;
			var b = true;
			// left.
			b &= 0 <= history.FindIndex_Ext(time, (_item, _time) => {
				return (_time <= _item.time) && (_item.dir.x < -0.7f);
			});
			// right.
			b &= 0 <= history.FindIndex_Ext(time, (_item, _time) => {
				return (_time <= _item.time) && (0.7f < _item.dir.x);
			});
			// down.
			b &= 0 <= history.FindIndex_Ext(time, (_item, _time) => {
				return (_time <= _item.time) && (_item.dir.y < -0.7f);
			});
			// up.
			b &= 0 <= history.FindIndex_Ext(time, (_item, _time) => {
				return (_time <= _item.time) && (0.7f < _item.dir.y);
			});
			return b;
		}

		void UpdateMove(Chara chara) {
			inputHistory_.RemoveAll_Ext(Time.time - 2f, (_item, _time) => {
				return _item.time < _time;
			});
			inputHistory_.Add(new InputItem() {
				time = Time.time,
				dir = Mouse.current.leftButton.isPressed ?
					Mouse.current.delta.ReadValue().normalized :
					Vector2.zero
			});
			hasShotInput = CheckShotInput(inputHistory_);



			var btn = Mouse.current.leftButton;
			if (btn.isPressed) {
				if (!pressed_) {
					pressPos_ = Mouse.current.position.ReadValue();
					anchorPos_ = chara.data.position;
				}

				var pos0 = Mouse.current.position.ReadValue();
				var posStart = ScreenService.Instance.Convert(pressPos_, Main.Instance.camera);
				var posCur = ScreenService.Instance.Convert(pos0, Main.Instance.camera);
				var mouseDeltaFromStart = posCur - posStart;

				var pos1 = anchorPos_ + (Vector3)(mouseDeltaFromStart);
				chara.data.position = pos1;
			}

			{
				var otherB = Main.Instance.screenArea.bounds;
				var ownC = GetComponentInChildren<Collider>();
				var ownB = ownC.bounds;
				ownB.center += chara.data.position - cpos0_;

				if (ownB.min.x < otherB.min.x) {
					chara.data.position.x += otherB.min.x - ownB.min.x;
				} else if (otherB.max.x < ownB.max.x) {
					chara.data.position.x += otherB.max.x - ownB.max.x;
				}

				if (ownB.min.y < otherB.min.y) {
					chara.data.position.y += otherB.min.y - ownB.min.y;
				} else if (otherB.max.y < ownB.max.y) {
					chara.data.position.y += otherB.max.y - ownB.max.y;
				}
			}

			pressed_ = btn.isPressed;
		}

		public void ManualUpdate(Chara chara) {
			cpos0_ = chara.data.position;

			UpdateShot(chara);
			UpdateMove(chara);
			transform.position = chara.data.position;
		}
	}
}