
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Effect {
		public static void Update(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (CharaAI.IsEnterTime(0f, preTime, self.data.stateTime)) {
				var info = self.GetComponentInChildren<Osakana4242.Content.Inners.Effect>();
				self.data.duration = info.Duration;
				var ps = info.GetComponentInChildren<ParticleSystem>();
				ps.Play(true);
			}

			if (TimeEventData.TryGetEvent(self.data.duration, 999f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = self.data.rotation * Vector3.forward * 0f;
						break;
					default:
						self.data.removeRequested = true;
						break;
				}
			}
		}
	}
}
