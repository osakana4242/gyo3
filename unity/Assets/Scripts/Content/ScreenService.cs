using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {

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

		/// <summary><see cref="Pointer.current.position.ReadValue" /> をこの画像内の位置に変換する</summary>
		public Vector2 ToLocalPosition(Vector2 screenPos) {
			// var posFromCenter = posFromLeftBottom - new Vector2( Screen.width, Screen.height ) * 0.5f;
			var screenAPos = (Vector2)screen_.transform.position;
			// 
			var posFromCenter = (Vector2)Main.Instance.screenBCamera.ScreenToWorldPoint(screenPos);
			var posFromScreenA = posFromCenter - screenAPos;
			var screenASize = GetWorldSize(screen_.rectTransform);
			var scaleBToA = new Vector2(
				screen_.texture.width / screenASize.x,
				screen_.texture.height / screenASize.y
			);
			// ScreenA の解像度に正規化.
			var posFromScreenANormaliozed = Vector2.Scale(posFromScreenA, scaleBToA);
			return posFromScreenANormaliozed;
		}

		static Vector3[] fourCorners_g_ = new Vector3[4];
		static Vector2 GetWorldSize(RectTransform tr) {
			tr.GetWorldCorners(fourCorners_g_);
			var leftBottom = fourCorners_g_[0];
			var rightTop = fourCorners_g_[2];
			var size = new Vector2(
				rightTop.x - leftBottom.x,
				rightTop.y - leftBottom.y
			);
			return size;
			// return Vector2.Scale( tr.sizeDelta, tr.lossyScale );
		}
	}
}
