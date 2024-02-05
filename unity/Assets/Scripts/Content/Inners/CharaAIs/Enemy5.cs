
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Enemy5 : ICharaComponent {
		public static readonly Enemy5 instance_g = new Enemy5();
		Vector3 targetPos_;

		/// <summary>ボス</summary>
		public void Update(Chara self) {

			TimeEvent evtData;
			var preTime = self.data.stateTime;
			self.data.stateTime += Stage.Current.time.dt;

			// 直進.
			if (TimeEvent.TryGetEvent(0.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
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
			if (TimeEvent.TryGetEvent(1.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = Vector3.zero;
						break;
					default:
						var targetPos = CharaAI.GetPositionFromScreenCenter(new Vector2(40, 20));
						var deltaPos = targetPos - self.data.position;
						var distance = deltaPos.magnitude;
						var speed = CharaAI.ClampSpeed(distance, 40, Stage.Current.time.dt);
						self.data.velocity = deltaPos.normalized * speed;
						break;
				}
			}

			// 下.
			if (TimeEvent.TryGetEvent(3.0f, 1.0f, preTime, self.data.stateTime, out evtData)) {
				switch (evtData.type) {
					case TimeEventType.Exit:
						self.data.velocity = Vector3.zero;
						break;
					default:
						var targetPos = CharaAI.GetPositionFromScreenCenter(new Vector2(40, -20));
						var deltaPos = targetPos - self.data.position;
						var distance = deltaPos.magnitude;
						var speed = CharaAI.ClampSpeed(distance, 40, Stage.Current.time.dt);
						self.data.velocity = deltaPos.normalized * speed;
						break;
				}
			}

			// 射撃.
			if (TimeEvent.IsEnter(2.0f, preTime, self.data.stateTime)) {
				targetPos_ = CharaAI.GetPlayerPosition(self);
			}
			if (TimeEvent.IsEnter(2.0f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}
			if (TimeEvent.IsEnter(2.2f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}
			if (TimeEvent.IsEnter(2.4f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}

			// 射撃.
			if (TimeEvent.IsEnter(2.0f, preTime, self.data.stateTime)) {
				targetPos_ = CharaAI.GetPlayerPosition(self);
			}
			if (TimeEvent.IsEnter(4.0f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}
			if (TimeEvent.IsEnter(4.2f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}
			if (TimeEvent.IsEnter(4.4f, preTime, self.data.stateTime)) {
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(-30f, 0f, 0f)));
				CharaAI.ShotToPosition(self, targetPos_, ShotParam.Create(new Vector3(0, 0, 16), Quaternion.Euler(0f, 0f, 0f)));
			}

			// 巻き戻し.
			if (TimeEvent.TryGetEvent(5.0f, 999f, preTime, self.data.stateTime, out evtData)) {
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
