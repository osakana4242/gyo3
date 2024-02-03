
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy4 {
		/// <summary>画面中央を横切ってから弧を描く</summary>
		public static void Update(Chara self) {
			TimeEventData evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime = Stage.Current.time.time - self.data.spawnedTime;

			// 直進.
			if (TimeEventData.TryGetEvent(0.0f, 10f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						break;
					default:
						self.data.velocity = self.data.rotation * Vector3.forward * 80f;
						break;
				}
			}

			// 方向合わせ.
			if (TimeEventData.TryGetEvent(0.5f, 1f, preTime, self.data.stateTime, out evtData)) {
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
			if (CharaAI.IsEnterTime(1.2f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPlayer(self, ShotParam.Create());
			}
		}
	}

}