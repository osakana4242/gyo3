using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class Pointer : MonoBehaviour {
		bool isPressed_;
		void FixedUpdate() {
			isPressed_ = Mouse.current.leftButton.isPressed;
			if (Mouse.current.leftButton.isPressed) {
				Vector2 pos0 = Mouse.current.position.ReadValue();
				Vector3 pos1 = ScreenService.Instance.Convert(pos0, Main.Instance.camera);
				pos1.z = -3f;
				transform.position = pos1;
			}
		}

		void OnGUI() {
			var width = 320f;
			var scale = Screen.width / width;
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
			GUI.Box( new Rect( 0, 0, width, 24 ), "" );
			var r = new Rect( 0, 0, width, 20 );
			GUI.Label( r, "press: " + isPressed_ );
		}
	}
}