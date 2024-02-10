
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class EnemyTestRoation : ICharaComponent {
		public static readonly EnemyTestRoation instance_g = new EnemyTestRoation();
		
		/// 一定時間追尾
		public void Update(Chara self) {
			TimeEvent evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (TimeEvent.IsEnter(Msec.FromSeconds(0f), preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(3f), preTime, self.data.stateTime, out evtData)) {
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

			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(999f), preTime, self.data.stateTime, out evtData)) {
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
		}
	}
}
