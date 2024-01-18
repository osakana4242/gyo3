using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class PointerCursor : MonoBehaviour {
		bool isPressed_;

		InputAct.ShipActionActions input;
		Vector2 pos;
		int btn;

		void Awake() {
			// input = new InputAct.ShipActionActions(new InputAct());
			// input.SetCallbacks(this);
		}


		void Start() {
		}

		void OnEnable() {
			// input.Enable();
		}

		void OnDisable() {
			// input.Disable();
		}

		void Update() {
		}

		void FixedUpdate() {
			isPressed_ = Pointer.current.press.isPressed;
			if (Pointer.current.press.isPressed) {
				Vector2 pos0 = Pointer.current.position.ReadValue();
				// Vector3 pos1 = ScreenService.Instance.ConvertB(pos0);
				transform.position = pos0;
			}
		}

		// void OnGUI() {
		// 	var width = 320f;
		// 	var scale = Screen.width / width;
		// 	GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
		// 	GUI.Box(new Rect(0, 0, width, 24), "");
		// 	var r = new Rect(0, 0, width, 20);
		// 	GUI.Label(r, "t: " + Time.time.ToString("F1") + ", pos: " + pos + ", btn:" + btn);
		// 	var evt = Event.current;
		// 	switch (evt.type) {
		// 		case EventType.Repaint:
		// 		case EventType.Layout:
		// 		break;
		// 		default:
		// 		Debug.Log("t:" + Time.time.ToString("F1") + ", evt: " + evt.type + ", pos:" + evt.mousePosition);
		// 		break;
		// 	}
		// }

		// void InputAct.IShipActionActions.OnMove(InputAction.CallbackContext context) {
		// 	var vec = context.ReadValue<Vector2>();
		// 	pos = vec;
		// 	//			Debug.Log($"f: {Time.frameCount}, t: {Time.time}, vec: {vec}");
		// }
		// void InputAct.IShipActionActions.OnPress(InputAction.CallbackContext context) {
		// 	// context.ReadValueAsButton();
		// 	btn += 1;
		// 	//			Debug.Log($"f: {Time.frameCount}, t: {Time.time}, vec: {vec}");
		// }
	}
}
