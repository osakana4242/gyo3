
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy3 : ICharaComponent {
		public static readonly Enemy3 instance_g = new Enemy3();

		/// <summary>一定時間追尾</summary>
		public void Update(Chara self) {
			TimeEvent evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (TimeEvent.IsEnter(Msec.FromSeconds(0f), preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (TimeEvent.IsEnter(Msec.FromSeconds(0.5f), preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 20f;
			}

			// 方向合わせ.
			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(5f), preTime, self.data.stateTime, out evtData)) {
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
			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(10f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 40f;
						break;
				}
			}

			// 射撃.
			if (TimeEvent.IsEnter(Msec.FromSeconds(2f), preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}

		}

	}

}
