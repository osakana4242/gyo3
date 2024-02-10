
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy4 : ICharaComponent {
		public static readonly Enemy4 instance_g = new Enemy4();

		/// <summary>画面中央を横切ってから弧を描く</summary>
		public void Update(Chara self) {
			TimeEvent evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			// 直進.
			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.0f), Msec.FromSeconds(10f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 80f;
						break;
				}
			}

			// 方向合わせ.
			if (TimeEvent.TryGetEvent(Msec.FromSeconds(0.5f), Msec.FromSeconds(1f), preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						var target = CharaAI.GetPositionFromScreenCenter(new Vector2(0f, 0f));
						target.x = self.data.spawnedPosition.x;
						self.data.rotation = CharaAI.LookAtAngle(self, target, 60f);
						break;
				}
			}

			// 射撃.
			if (TimeEvent.IsEnter(Msec.FromSeconds(1.2f), preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}
		}
	}

}
