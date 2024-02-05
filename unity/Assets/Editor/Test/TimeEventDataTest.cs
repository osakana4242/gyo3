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
		public void TryGetEventTest() {
			var evtData = default(TimeEvent);

			Assert.IsFalse(TimeEvent.TryGetEvent(1.0f, 2.0f, 0.9f, 1.0f, out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

			Assert.IsFalse(TimeEvent.TryGetEvent(1.0f, 2.0f, 1.0f, 1.0f, out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(1.0f, 2.0f, 1.0f, 1.1f, out evtData));
			Assert.AreEqual(TimeEventType.Enter, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(1.0f, 2.0f, 1.1f, 1.1f, out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(1.0f, 2.0f, 2.9f, 3.0f, out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(1.0f, 2.0f, 3.0f, 3.0f, out evtData));
			Assert.AreEqual(TimeEventType.Loop, evtData.type);

			Assert.IsTrue(TimeEvent.TryGetEvent(1.0f, 2.0f, 3.0f, 3.1f, out evtData));
			Assert.AreEqual(TimeEventType.Exit, evtData.type);

			Assert.IsFalse(TimeEvent.TryGetEvent(1.0f, 2.0f, 3.1f, 3.1f, out evtData));
			Assert.AreEqual(TimeEventType.None, evtData.type);

		}
	}
}
