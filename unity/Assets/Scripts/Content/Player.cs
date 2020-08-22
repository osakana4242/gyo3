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
		Vector2 pointerPrePos_;
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
				blt.data.position = chara.data.position + new Vector3(0f, Random.Range(-2f, 2f), 0f);
				Main.Instance.stage.charaBank.Add(blt);
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
				dir = Pointer.current.press.isPressed ?
					Pointer.current.delta.ReadValue().normalized :
					Vector2.zero
			});
			hasShotInput = CheckShotInput(inputHistory_);


			var btn = Pointer.current.press;
			if (btn.isPressed) {
				if (!pressed_) {
					pressPos_ = Pointer.current.position.ReadValue();
					anchorPos_ = chara.data.position;
					pointerPrePos_ = pressPos_;
				}
				var prePos = pointerPrePos_;
				pointerPrePos_ = Pointer.current.position.ReadValue();
				var pos0 = pointerPrePos_;;
				var posStart = ScreenService.Instance.Convert(pressPos_, Main.Instance.camera);
				var localPrePos = ScreenService.Instance.Convert(prePos, Main.Instance.camera);
				var posCur = ScreenService.Instance.Convert(pos0, Main.Instance.camera);
				var deltaFromPre = posCur - localPrePos;
				var mouseDeltaFromStart = posCur - posStart;

				var marginY = ScreenService.Instance.GameMainSize.y;
				// var marginX = 32f;
				var pressDelta = (Vector2)anchorPos_ - posStart;
				Debug.Log("pressDelta: " + pressDelta);
				if (pressDelta.y < marginY) {
					if (deltaFromPre.y < 0f) {
						anchorPos_.y -= Mathf.Clamp(deltaFromPre.y, -marginY, marginY);
					}
				}

				var pos1 = anchorPos_ + (Vector3)(mouseDeltaFromStart);



				chara.data.position = pos1;
			}

			{
				var otherB = Main.Instance.screenArea.bounds;
				var ownC = GetComponentInChildren<Collider>();
				var ownB = ownC.bounds;
				ownB.center += chara.data.position - cpos0_;

				if (ownB.min.x < otherB.min.x) {
					var delta = otherB.min.x - ownB.min.x;
					chara.data.position.x += delta;
					anchorPos_.x += delta;
				} else if (otherB.max.x < ownB.max.x) {
					var delta = otherB.max.x - ownB.max.x;
					chara.data.position.x += delta;
					anchorPos_.x += delta;
				}

				if (ownB.min.y < otherB.min.y) {
					var delta = otherB.min.y - ownB.min.y;
					chara.data.position.y += delta;
					anchorPos_.y += delta;
				} else if (otherB.max.y < ownB.max.y) {
					var delta = otherB.max.y - ownB.max.y;
					chara.data.position.y += delta;
					anchorPos_.y += delta;
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