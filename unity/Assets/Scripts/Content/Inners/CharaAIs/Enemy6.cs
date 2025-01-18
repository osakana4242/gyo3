
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy6 : ICharaComponent {
		public static readonly Enemy6 instance_g = new Enemy6();

		public void Update(Chara self) {
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			if (TimeEvent.IsEnter(Msec.FromSeconds(0f), preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.up * 40f;
			}

			if (TimeEvent.IsEnter(Msec.FromSeconds(0.5f), preTime, self.data.stateTime)) {
				self.data.velocity = self.data.rotation * Vector3.up * 20f;
			}
		}
	}
}
