
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy1 : ICharaComponent {
		public static readonly Enemy1 instance_g = new Enemy1();

		public void Update(Chara self) {
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (TimeEvent.IsEnter(0f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 40f;
			}

			if (TimeEvent.IsEnter(0.5f, preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.forward * 20f;
			}

			if (TimeEvent.IsEnter(2f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}

			// if (IsEnterTime(4f, preTime, self.data.stateTime)) {
			// 	ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(15f, 0f, 0f)));
			// 	ShotToPlayer(self, ShotParam.Create(Vector3.zero, Quaternion.Euler(-15f, 0f, 0f)));
			// }
		}
	}
}
