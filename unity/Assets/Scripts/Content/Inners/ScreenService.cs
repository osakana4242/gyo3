using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
namespace Osakana4242.Content.Inners {

	public class ScreenAService {
		static ScreenAService instance_;
		public static ScreenAService Instance => instance_;
		UnityEngine.UI.RawImage screen_;

		public static void Init() {
			instance_ = new ScreenAService(Main.Instance.rawImage);
			instance_.AdjustIfNeeded();
		}

		public ScreenAService(UnityEngine.UI.RawImage rawImage) {
			screen_ = rawImage;
		}

		public void Dispose() {
			screen_ = null;
		}

		public Vector2 Size => new Vector2(screen_.texture.width, screen_.texture.height);
		public void AdjustIfNeeded() {
		}

		/// <summary><see cref="Pointer.current.position.ReadValue" /> をこの画像内の位置、解像度に変換する</summary>
		public Vector2 ToLocalPosition(Vector2 screenPos) {
			var screenAPos = (Vector2)screen_.transform.position;
			// 
			var posFromCenter = (Vector2)Main.Instance.screenBCamera.ScreenToWorldPoint(screenPos);
			var posFromScreenA = posFromCenter - screenAPos;
			var screenASize = screen_.rectTransform.GetWorldSizeXY_Ext();
			var scaleBToA = new Vector2(
				screen_.texture.width / screenASize.x,
				screen_.texture.height / screenASize.y
			);
			// ScreenA の解像度に正規化.
			var posFromScreenANormaliozed = Vector2.Scale(posFromScreenA, scaleBToA);
			return posFromScreenANormaliozed;
		}

	}
}
