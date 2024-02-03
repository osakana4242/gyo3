using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {
	public class Player : MonoBehaviour {
		void Start() {

		}

		Vector2 pressPos_;
		Vector2 pointerPrePos_;
		Vector3 anchorPos_;
		Vector3 cpos0_;
		bool pressed_;
		public bool hasShotInput;
		bool hasCharge_ = false;
		Weapon weapon_ = new Weapon();
		ChargeWeapon chargeWeapon_ = new ChargeWeapon();

		List<InputItem> inputHistory_ = new List<InputItem>();

		/// <summary>レバガチャ判定</summary>
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
					pressPos_ = ScreenAService.Instance.ToLocalPosition(Pointer.current.position.ReadValue());
					anchorPos_ = chara.data.position;
					pointerPrePos_ = pressPos_;
				}
				var prePos = pointerPrePos_;
				pointerPrePos_ = ScreenAService.Instance.ToLocalPosition(Pointer.current.position.ReadValue());
				var pos0 = pointerPrePos_;
				var posStart = pressPos_;
				var localPrePos = prePos;
				var posCur = pos0;
				var deltaFromPre = posCur - localPrePos;
				var mouseDeltaFromStart = posCur - posStart;

				var marginY = ScreenAService.Instance.Size.y;
				// var marginX = 32f;
				var pressDelta = (Vector2)anchorPos_ - posStart;
				//Debug.Log("pressDelta: " + pressDelta);
				if (pressDelta.y < marginY) {
					if (deltaFromPre.y < 0f) {
						anchorPos_.y -= Mathf.Clamp(deltaFromPre.y, -marginY, marginY);
					}
				}

				var pos1 = anchorPos_ + (Vector3)(mouseDeltaFromStart);



				chara.data.position = pos1;
			}

			{
				var otherBounds = InnerMain.Instance.playerMovableArea.bounds;
				var ownCollider = GetComponentInChildren<Collider>();
				var ownBounds = ownCollider.bounds;
				ownBounds.center += chara.data.position - cpos0_;

				if (ownBounds.min.x < otherBounds.min.x) {
					var delta = otherBounds.min.x - ownBounds.min.x;
					chara.data.position.x += delta;
					anchorPos_.x += delta;
				} else if (otherBounds.max.x < ownBounds.max.x) {
					var delta = otherBounds.max.x - ownBounds.max.x;
					chara.data.position.x += delta;
					anchorPos_.x += delta;
				}

				if (ownBounds.min.y < otherBounds.min.y) {
					var delta = otherBounds.min.y - ownBounds.min.y;
					chara.data.position.y += delta;
					anchorPos_.y += delta;
				} else if (otherBounds.max.y < ownBounds.max.y) {
					var delta = otherBounds.max.y - ownBounds.max.y;
					chara.data.position.y += delta;
					anchorPos_.y += delta;
				}
			}

			pressed_ = btn.isPressed;
			hasCharge_ = pressed_;
		}

		public void ManualUpdate(Chara chara) {
			cpos0_ = chara.data.position;
			weapon_.UpdateShot(chara);
			chargeWeapon_.UpdateShot(chara, hasCharge_);
			UpdateMove(chara);
			transform.position = chara.data.position;
		}

		public void Report() {
			InnerMain.Instance.playerInfo.weaponChargeProgress = chargeWeapon_.progress;
		}

		public class Weapon {
			public float shotTime;
			public float shotInterval = 0.05f;

			public void UpdateShot(Chara chara) {
				if (shotTime < shotInterval) {
					shotTime += Time.deltaTime;
				} else {
					shotTime = 0f;
					var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.BLT_1_ASSET);
					var blt = CharaFactory.CreateBullet(info);
					blt.data.position = chara.data.position + chara.data.rotation * new Vector3(0f, Random.Range(-2f, 2f), 16f);
					blt.data.velocity = new Vector3(CharaFactory.SpeedByScreen(2f), 0f, 0f);

					InnerMain.Instance.stage.charaBank.Add(blt);
				}
			}
		}

		class ChargeWeapon {
			public WeaponChargeProgress progress = new WeaponChargeProgress(0f, 1f);

			public void UpdateShot(Chara chara, bool hasCharge) {
				if (hasCharge) {
					progress = progress.AddTime(Time.deltaTime);
					return;
				}

				if (!progress.IsComleted()) {
					progress = progress.Reset();
					return;
				}

				progress = progress.Reset();

				// アイデア
				// ジェスチャーチャージ
				// 右回転して離すと攻撃A
				// 左回転して離すと攻撃B

				var bullets = new (float angle, float speed)[] {
					(-10, CharaFactory.SpeedByScreen( 1f ) ),
					(- 5, CharaFactory.SpeedByScreen( 1f ) ),
					(  5, CharaFactory.SpeedByScreen( 1f ) ),
					( 10, CharaFactory.SpeedByScreen( 1f ) ),
					(-20, CharaFactory.SpeedByScreen( 1.5f ) ),
					(-10, CharaFactory.SpeedByScreen( 1.5f ) ),
					( 10, CharaFactory.SpeedByScreen( 1.5f ) ),
					( 20, CharaFactory.SpeedByScreen( 1.5f ) ),
				};
				foreach (var bullet in bullets) {
					var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.BLT_1_ASSET);
					var blt = CharaFactory.CreateBullet(info);
					blt.data.rotation = chara.data.rotation * Quaternion.AngleAxis(bullet.angle, new Vector3(1, 0, 0));
					blt.data.position = chara.data.position + blt.data.rotation * new Vector3(0f, Random.Range(-2f, 2f), 16f);
					blt.data.velocity = blt.data.rotation * Vector3.forward * bullet.speed;
					InnerMain.Instance.stage.charaBank.Add(blt);
				}
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
	}

	public struct WeaponChargeProgress {
		readonly float time;
		readonly float timeMax;

		public WeaponChargeProgress(float time, float timeMax) {
			this.time = time;
			this.timeMax = timeMax;
		}

		public bool IsComleted() => timeMax <= time;

		public WeaponChargeProgress AddTime(float addTime) {
			return new WeaponChargeProgress(Mathf.Clamp(this.time + addTime, 0f, timeMax), timeMax);
		}

		public WeaponChargeProgress Reset() {
			return new WeaponChargeProgress(0f, timeMax);
		}

		public string ToPercentString() {
			var percent = (int)(time * 100 / timeMax);
			return $"{percent}%";
		}
	}

}
