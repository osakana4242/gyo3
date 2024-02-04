
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {

	public class CharaAI {
		public static void ShotToPlayer(Chara self, ShotParam prm) {
			var info = AssetService.Instance.Get<CharaInfo>(AssetInfos.BLT_2_ASSET);
			var blt = CharaFactory.CreateBullet(info);
			Stage.Current.charaBank.Add(blt);
			blt.data.position = self.data.position;
			Vector3 target;
			if (Stage.Current.charaBank.TryGetPlayer(out var player)) {
				target = player.data.position;
			} else {
				target = self.data.position + self.data.rotation * Vector3.forward;
			}
			Vector3 toTarget = (target - blt.data.position).ToXY0_Ext();
			blt.data.rotation = Quaternion.LookRotation(toTarget);
			blt.data.position += blt.data.rotation * prm.offsetPos;
			blt.data.rotation *= prm.offsetRot;
			blt.data.velocity = blt.data.rotation * Vector3.forward * 60f;
			blt.ApplyTransform();
		}

		public static Quaternion LookAtAngle(Chara self, Vector3 position, float maxDelta) {
			Vector2 vec = self.data.rotation * Vector3.forward;
			Vector2 toTargetVec = position - self.data.position;
			var angleSpeed = maxDelta * Stage.Current.time.dt;
			Vector2 vec2 = Vector2Util.MoveTowardsByAngle(vec, toTargetVec, angleSpeed);
			return Quaternion.LookRotation(vec2);
		}

		public static Vector3 GetPositionFromScreenCenter(Vector3 position) {
			return position;
		}

		public static bool IsEnterTime(float target, float pre, float current) {
			return pre < target && target <= current;
		}

		/// <summary>指定距離を通りすぎないようにスピードを抑制する</summary>
		public static float ClampSpeed(float distance, float speed, float dt) {
			if (dt <= 0) return speed;
			var movedDistance = dt * speed;
			if (movedDistance <= distance) return speed;
			var clampedSpeed = distance / dt;
			return clampedSpeed;
		}
	}

	public struct ShotParam {
		public Vector3 offsetPos;
		public Quaternion offsetRot;

		public static ShotParam Create(Vector3 pos = default) {
			return Create(pos, Quaternion.identity);
		}
		public static ShotParam Create(Vector3 pos, Quaternion rot) {
			return new ShotParam() {
				offsetPos = pos,
				offsetRot = rot,
			};
		}
	}
}
