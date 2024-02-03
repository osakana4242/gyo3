
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy2 {
		public static void Update(Chara self) {
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
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-30f, 0f, 0f)));
			}

			if (TimeEventData.TryGetEvent(2.5f, 0f, preTime, self.data.stateTime, out evtData)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(20f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-20f, 0f, 0f)));
			}

			if (TimeEventData.TryGetEvent(3f, 0f, preTime, self.data.stateTime, out evtData)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(15f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-15f, 0f, 0f)));
			}
		}
	}

}
