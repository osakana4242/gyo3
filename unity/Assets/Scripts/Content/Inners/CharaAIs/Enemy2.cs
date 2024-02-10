
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy2 : ICharaComponent {
		public static readonly Enemy2 instance_g = new Enemy2();

		public void Update(Chara self) {
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;
			TimeEvent evtData;

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0f), Msec.FromSeconds(0.5f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 40f;
						break;
				}
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(9f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 5f;
						break;
				}
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(10f), Msec.FromSeconds(5f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 40f;
						break;
				}
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(15f), Msec.FromSeconds(0f), preTime, self.data.stateTime, out evtData)) {
				self.data.removeRequested = true;
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(2f), Msec.FromSeconds(0f), preTime, self.data.stateTime, out evtData)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-30f, 0f, 0f)));
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(2.5f), Msec.FromSeconds(0f), preTime, self.data.stateTime, out evtData)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(20f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-20f, 0f, 0f)));
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(3f), Msec.FromSeconds(0f), preTime, self.data.stateTime, out evtData)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(15f, 0f, 0f)));
				CharaAI.ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-15f, 0f, 0f)));
			}
		}
	}

}
