using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class InputChecker : MonoBehaviour {
		bool isPressed_;
		void FixedUpdate() {
			isPressed_ = Mouse.current.leftButton.isPressed;
		}

		void OnGUI() {
			GUI.matrix = Matrix4x4.Scale(new Vector3(2, 2, 2));
			GUI.Box(new Rect(0, 0, 320, 16), "");
			var r = new Rect(0, 0, 320, 16);
			GUI.Label(r, "press: " + isPressed_);
		}
	}
}