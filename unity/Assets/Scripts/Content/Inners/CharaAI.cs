using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners {
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

			if (TimeEventData.TryGetEvent(10f, 5f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					self.data.velocity = self.data.rotation * Vector3.forward * 0f;
					break;
					default:
					self.data.velocity = self.data.rotation * Vector3.forward * 40f;
					break;
				}
			}

			if (TimeEventData.TryGetEvent(15f, 0f, preTime, self.data.stateTime, out evtData)) {
				self.data.removeRequested = true;
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

		/// <summary>一定時間追尾</summary>
		public static void UpdateEnemy3(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			if (IsEnterTime(0f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (IsEnterTime(0.5f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 20f;
			}

			// 方向合わせ.
			if (TimeEventData.TryGetEvent(0.5f, 5f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					break;
					default:
					if (Stage.Current.charaBank.TryGetPlayer(out var player)) {
						self.data.rotation = LookAtAngle(self, player.data.position, 60);
					}
					break;
				}
			}

			// 直進.
			if (TimeEventData.TryGetEvent(0.5f, 10f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					break;
					default:
					self.data.velocity = self.data.rotation * Vector3.forward * 40f;
					break;
				}
			}

			// 射撃.
			if (IsEnterTime(2f, preTime, self.data.stateTime)) {
				ShotToPlayer(self, ShotParam.Create());
			}

		}

		/// <summary>画面中央を横切ってから弧を描く</summary>
		public static void UpdateEnemy4(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			// 直進.
			if (TimeEventData.TryGetEvent(0.0f, 10f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					break;
					default:
					self.data.velocity = self.data.rotation * Vector3.forward * 80f;
					break;
				}
			}

			// 方向合わせ.
			if (TimeEventData.TryGetEvent(0.5f, 1f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					break;
					default:
						var target = GetPositionFromScreenCenter(new Vector2(0f, 0f));
						target.x = self.data.spawnedPosition.x;
						self.data.rotation = LookAtAngle(self, target, 60f);
					break;
				}
			}

			// 射撃.
			if (IsEnterTime(1.2f, preTime, self.data.stateTime)) {
				ShotToPlayer(self, ShotParam.Create());
			}
		}

		/// 一定時間追尾
		public static void UpdateEnemyRotationTest(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			if (IsEnterTime(0f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (TimeEventData.TryGetEvent(0.5f, 3f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					self.data.velocity = self.data.rotation * Vector3.forward * 0f;
					break;
					default:
					var target = new Vector3(0f, 0f, 0f);
					var toT = target - self.data.position;
					self.data.velocity = toT * 20f;
					break;
				}
			}

			if (TimeEventData.TryGetEvent(0.5f, 999f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
					break;
					default:
					if (Stage.Current.charaBank.TryGetPlayer(out var player)) {
						self.data.rotation = LookAtAngle(self, player.data.position, 60);
					}
					break;
				}
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
