using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using Osakana4242.Content;
using Osakana4242.Content.Inners;

namespace Osakana4242.Content.Inners.Tests {
	public class TimeEventDataTest {
		[Test]
		public void FloatTimeTest() {
			var dt = 1.0f / 60.0f;
			float time = 0f;
			time += dt;
			for (int i = 0; i < 32; ++i) {
				var preTime = time;
				time += dt;
				var calcedPreTime = time - dt;
				Assert.AreEqual(preTime, calcedPreTime);
			}
		}

		[Test]
		public void TryGetEventTest() {
			var evtData = default(TimeEvent);

			Assert.IsFalse(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(0.9f), Msec.FromSeconds(1.0f), out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

			Assert.IsFalse(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(1.0f), Msec.FromSeconds(1.0f), out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(1.0f), Msec.FromSeconds(1.1f), out evtData));
			Assert.AreEqual(TimeEventType.Enter, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(1.1f), Msec.FromSeconds(1.1f), out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(2.9f), Msec.FromSeconds(3.0f), out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(3.0f), Msec.FromSeconds(3.0f), out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(3.0f), Msec.FromSeconds(3.1f), out evtData));
			Assert.AreEqual(TimeEventType.Exit, evtData.type);

			Assert.IsFalse(TimeEvent.TryGetEvent(Msec.FromSeconds(1.0f), Msec.FromSeconds(2.0f), Msec.FromSeconds(3.1f), Msec.FromSeconds(3.1f), out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

		}
	}
}
