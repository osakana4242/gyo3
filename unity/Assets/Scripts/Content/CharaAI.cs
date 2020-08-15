using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class CharaAI {
		public static void ShotToPlayer(Chara self, ShotParam prm) {
			var blt = CharaFactory.CreateBullet2();
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

		public static bool IsEnterTime(float target, float pre, float current) {
			return pre < target && target <= current;
		}

		public static void UpdateEnemy1(Chara self) {
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			if (IsEnterTime(0f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (IsEnterTime(0.5f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 20f;
			}

			if (IsEnterTime(2f, preTime, self.data.stateTime)) {
				ShotToPlayer(self, ShotParam.Create());
			}

			// if (IsEnterTime(4f, preTime, self.data.stateTime)) {
			// 	ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(15f, 0f, 0f)));
			// 	ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-15f, 0f, 0f)));
			// }
		}

		public static void UpdateEnemy2(Chara self) {
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;
			TimeEventData evtData;

			if (TimeEventData.TryGetEvent(0f, 0.5f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 40f;
						break;
				}
			}

			if (TimeEventData.TryGetEvent(0.5f, 9f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 5f;
						break;
				}
			}

			if (TimeEventData.TryGetEvent(10f, 2f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 40f;
						break;
				}
			}

			if (TimeEventData.TryGetEvent(12f, 0f, preTime, self.data.stateTime, out evtData)) {
				GameObject.Destroy(self.gameObject);
			}

			if (TimeEventData.TryGetEvent(2f, 0f, preTime, self.data.stateTime, out evtData)) {
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(30f, 0f, 0f)));
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-30f, 0f, 0f)));
			}

			if (TimeEventData.TryGetEvent(2.5f, 0f, preTime, self.data.stateTime, out evtData)) {
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(20f, 0f, 0f)));
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-20f, 0f, 0f)));
			}

			if (TimeEventData.TryGetEvent(3f, 0f, preTime, self.data.stateTime, out evtData)) {
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(15f, 0f, 0f)));
				ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-15f, 0f, 0f)));
			}
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
