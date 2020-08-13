using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class Pointer : MonoBehaviour {
		void FixedUpdate() {
			if (Mouse.current.leftButton.isPressed) {
				var pos0 = Mouse.current.position.ReadValue();
				var pos1 = ScreenService.Instance.Convert(pos0, Main.Instance.camera);
				transform.position = pos1;
			}
		}
	}
}