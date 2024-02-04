
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy3 {
		/// <summary>一定時間追尾</summary>
		public static void Update(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (CharaAI.IsEnterTime(0f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (CharaAI.IsEnterTime(0.5f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 20f;
			}

			// 方向合わせ.
			if (TimeEventData.TryGetEvent(0.5f, 5f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						if (Stage.Current.charaBank.TryGetPlayer(out var player)) {
							self.data.rotation = CharaAI.LookAtAngle(self, player.data.position, 60);
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
			if (CharaAI.IsEnterTime(2f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}

		}

	}

}
