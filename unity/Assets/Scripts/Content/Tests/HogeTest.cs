using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Tests {
	public class HogeTest {
		[Test]
		public void ToAngleTest() {
			Assert.AreEqual(45, new Vector2(1, 1).ToAngle_Ext());
			Assert.AreEqual(90, new Vector2(0, 1).ToAngle_Ext());
			Assert.AreEqual(135, new Vector2(-1, 1).ToAngle_Ext());
			Assert.AreEqual(180, new Vector2(-1, 0).ToAngle_Ext());

			Assert.AreEqual(-45, new Vector2(1, -1).ToAngle_Ext());
			Assert.AreEqual(-90, new Vector2(0, -1).ToAngle_Ext());
			Assert.AreEqual(-135, new Vector2(-1, -1).ToAngle_Ext());
			// Assert.AreEqual( -180, new Vector2(-1, 0).ToAngle());
		}

		[Test]
		public void MoveTowardsByAngle() {
			Assert.AreEqual(Vector2Util.FromDeg(45f), Vector2Util.MoveTowardsByAngle(new Vector2(0f, 0f), new Vector2(-1f, 0.1f), 45f));
			Assert.AreEqual(Vector2Util.FromDeg(90f), Vector2Util.MoveTowardsByAngle(new Vector2(0f, 0f), new Vector2(-1f, 0.1f), 90f));
			Assert.AreEqual(Vector2Util.FromDeg(-45f), Vector2Util.MoveTowardsByAngle(new Vector2(0f, 0f), new Vector2(-1f, -0.1f), 45f));
			Assert.AreEqual(Vector2Util.FromDeg(-90f), Vector2Util.MoveTowardsByAngle(new Vector2(0f, 0f), new Vector2(-1f, -0.1f), 90f));
		}

		// A Test behaves as an ordinary method
		[Test]
		public void HogeTestSimplePasses() {
			// Use the Assert class to test conditions

			Vector2 vec = new Vector3(1f, 0f, 0f);
			Vector2 toTargetVec = new Vector3(0f, 1f, 0f);
			var a1 = Vector2.Angle(Vector2.right, vec);
			var a2 = Vector2.Angle(Vector2.right, toTargetVec);
			Assert.AreEqual(0, a1, "unko");
			Assert.AreEqual(90, a2, "pantsu");
			var a3 = Mathf.MoveTowardsAngle(a1, a2, 45);
			Assert.AreEqual(45, a3, "pon");


			// var nextX = Mathf.MoveTowardsAngle(self.data.rotation.x);
			// self.data.velocity = self.data.rotation * Vector3.forward * 5f;


		}
		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		[UnityTest]
		public IEnumerator HogeTestWithEnumeratorPasses() {
			// Use the Assert class to test conditions.
			// Use yield to skip a frame.
			yield return null;
		}
	}
}
