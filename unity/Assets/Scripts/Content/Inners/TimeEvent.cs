using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
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
		public Msec time;

		// static int ToMsec(float sec) => (int)(sec * 1000);

		// public static bool IsEnter(float startTime, float preTime, float currentTime) {
		// 	return IsEnter(ToMsec(startTime), ToMsec(preTime), ToMsec(currentTime));
		// }

		public static bool IsEnter(Msec startTime, Msec preTime, Msec currentTime) {
			return preTime <= startTime && startTime < currentTime;
		}

		// public static bool TryGetEvent(float startTime, float preTime, float currentTime, out TimeEvent evtData) =>
		// 	TryGetEvent(ToMsec(startTime), 999999999, ToMsec(preTime), ToMsec(currentTime), out evtData);

		public static bool TryGetEvent(Msec startTime, Msec preTime, Msec currentTime, out TimeEvent evtData) {
			evtData = new TimeEvent();
			evtData.type = TimeEventType.None;
			var left = startTime;
			if (currentTime <= left) {
				return false;
			}

			if (IsEnter(startTime, preTime, currentTime)) {
				evtData.type = TimeEventType.Enter;
				return true;
			}

			evtData.type = TimeEventType.Loop;
			evtData.time = currentTime - startTime;
			evtData.progress = 1.0f;
			return true;
		}

		// public static bool TryGetEvent(float startTime, float duration, float preTime, float currentTime, out TimeEvent evtData) =>
		// 	TryGetEvent(ToMsec(startTime), ToMsec(duration), ToMsec(preTime), ToMsec(currentTime), out evtData);

		public static bool TryGetEvent(Msec startTime, Msec duration, Msec preTime, Msec currentTime, out TimeEvent evtData) {
			evtData = new TimeEvent();
			evtData.type = TimeEventType.None;
			var left = startTime;
			var right = left + duration;
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
