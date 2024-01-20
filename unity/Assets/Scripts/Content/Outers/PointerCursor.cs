using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content.Outers {
	public class PointerCursor : MonoBehaviour {
		bool isPressed_;
		[SerializeField] UnityEngine.UI.Image image_;

		void Update() {
			isPressed_ = Pointer.current.press.isPressed;
			if (isPressed_) {
				
				Vector2 pos1 = Main.Instance.screenBCamera.ScreenToWorldPoint( Pointer.current.position.ReadValue() );
				// Vector3 pos1 = ScreenService.Instance.ConvertB(pos0);
				var pos2 = transform.position;
				var pos3 = new Vector3( pos1.x, pos1.y, pos2.z );

				transform.position = pos3;
			}
			image_.enabled = isPressed_;
		}
	}
}
