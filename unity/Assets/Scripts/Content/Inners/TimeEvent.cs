using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using System.Threading;
using Osakana4242.Lib.AssetServices;
using Cysharp.Threading.Tasks;
namespace Osakana4242.Content.Inners {

	public enum TimeEventType {
		None,
		Enter,
		Loop,
		Exit,
	}

	public struct TimeEvent {
		public TimeEventType type;
		public float progress;
		public float time;

		public static bool IsEnter(float startTime, float preTime, float currentTime) {
			return preTime <= startTime && startTime < currentTime;
		}

		public static bool TryGetEvent(float startTime, float duration, float preTime, float currentTime, out TimeEvent evtData) {
			evtData = new TimeEvent();
			evtData.type = TimeEventType.None;
			float left = startTime;
			float right = left + duration;
			if (currentTime <= left) {
				return false;
			}
			if (right < preTime) {
				return false;
			}

			if (IsEnter(startTime, preTime, currentTime)) {
				evtData.type = TimeEventType.Enter;
				return true;
			}
			if (preTime <= right && right < currentTime) {
				evtData.type = TimeEventType.Exit;
				evtData.time = duration;
				evtData.progress = 1f;
				return true;
			}
			evtData.type = TimeEventType.Loop;
			evtData.time = currentTime - startTime;
			evtData.progress = evtData.time / duration;
			return true;
		}
	}


}
