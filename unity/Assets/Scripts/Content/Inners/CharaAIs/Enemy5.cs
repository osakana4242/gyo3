
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy5 {

		/// <summary>ボス</summary>
		public static void Update(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			// 直進.
			if (TimeEventData.TryGetEvent(0.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = Vector3.zero;
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 80f;
						break;
				}
			}

			// 上.
			if (TimeEventData.TryGetEvent(1.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = Vector3.zero;
						break;
					default:
						var targetPos = CharaAI.GetPositionFromScreenCenter(new Vector2(40, 40));
						var deltaPos = targetPos - self.data.position;
						var distance = deltaPos.magnitude;
						var speed = CharaAI.ClampSpeed(distance, 80, Stage.Current.time.dt);
						self.data.velocity = deltaPos.normalized * speed;
						break;
				}
			}

			// 下.
			if (TimeEventData.TryGetEvent(3.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = Vector3.zero;
						break;
					default:
						var targetPos = CharaAI.GetPositionFromScreenCenter(new Vector2(40, -40));
						var deltaPos = targetPos - self.data.position;
						var distance = deltaPos.magnitude;
						var speed = CharaAI.ClampSpeed(distance, 80, Stage.Current.time.dt);
						self.data.velocity = deltaPos.normalized * speed;
						break;
				}
			}

			// 射撃.
			if (CharaAI.IsEnterTime(2.0f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}

			// 射撃.
			if (CharaAI.IsEnterTime(4.0f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}

			// 巻き戻し.
			if (TimeEventData.TryGetEvent(5.0f, 999f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						self.data.stateTime = 1.0f;
						break;
				}
			}

		}
	}

}
