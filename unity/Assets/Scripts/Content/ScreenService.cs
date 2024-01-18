using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class ScreenService {
		static ScreenService instance_;
		public static ScreenService Instance => instance_;
		float width_;
		float height_;
		float scale_;
		float mainHeight_;
		float subHeight_;
		Vector2 sc_;

		RenderTexture tex_;

		public static void Init() {
			instance_ = new ScreenService();
			instance_.AdjustIfNeeded();
		}

		public void Dispose() {
			tex_.Release();
			tex_ = null;
		}

		public float Scale => scale_;
		public Vector2 GameResolution => new Vector2(width_, height_);
		public Vector2 GameMainSize => new Vector2(width_, mainHeight_);
		public Vector2 GameSubSize => new Vector2(width_, subHeight_);

		public void Adjust(Vector2 sc) {
			if ( null == tex_ ) {
				tex_ = new RenderTexture( 160, 120, 16, RenderTextureFormat.ARGB32 );
				Main.Instance.camera.targetTexture = tex_;
				Main.Instance.rawImage.texture = tex_;
			}
			var designAspect = Config.instance.screen.size.x / Config.instance.screen.size.y;
			var screenAspect = sc.x / sc.y;
			// w / h は縦長なほど小さくなる.
			if (designAspect < screenAspect) {
				// スクリーンが横長
				width_ = Config.instance.screen.size.x;
				height_ = Config.instance.screen.size.y;
				scale_ = height_ / sc.y;

				mainHeight_ = Config.instance.screen.main.y;
				subHeight_ = height_ - mainHeight_;

				var sw = sc.x * scale_;
				var margin = (sw - width_) / sw;
				Main.Instance.camera.rect = new Rect(
					margin * 0.5f,
					0f,
					1f - margin,
					1f
				);
				Main.Instance.camera.orthographicSize = height_ * 0.5f;
			} else {
				// スクリーンが縦長
				width_ = Config.instance.screen.size.x;
				scale_ = width_ / sc.x;
				height_ = Config.instance.screen.size.y;
				//height_ = width_ * sc.y / sc.x;

				mainHeight_ = Config.instance.screen.main.y;
				subHeight_ = height_ - mainHeight_;
				Main.Instance.camera.rect = new Rect(0f, 0f, 1f, 1f);
				Main.Instance.camera.orthographicSize = height_ * 0.5f;
			}
			var h = Config.instance.screen.size.y;
			var h2 = height_ - h;
			// Main.Instance.camera.transform.position = new Vector3(0f, h * -0.25f - h2 * 0.5f, -10f);

		}

		public void AdjustIfNeeded() {
			var sc = new Vector2(Screen.width, Screen.height);
			if ( null != tex_ ) {
				sc = new Vector2( tex_.width, tex_.height );
			}
			if (sc_ == sc) return;
			sc_ = sc;
			Adjust(sc);
		}

		public Vector2 Convert(Vector2 posFromCenter) {
			// var posFromCenter = posFromLeftBottom - new Vector2( Screen.width, Screen.height ) * 0.5f;
			var screenA = Main.Instance.rawImage;
			var screenAPos = (Vector2)screenA.transform.position;
			// 
			var posFromScreenA = posFromCenter - screenAPos;
			var screenASize = GetWorldSize( screenA.rectTransform );
			var scaleBToA = new Vector2(
				screenA.texture.width / screenASize.x,
				screenA.texture.height / screenASize.y
			);
			// ScreenA の解像度に正規化.
			var posFromScreenANormaliozed = Vector2.Scale( posFromScreenA, scaleBToA );
			return posFromScreenANormaliozed;
		}

		static Vector3[] fourCorners_g_ = new Vector3[4];
		static Vector2 GetWorldSize( RectTransform tr ) {
			tr.GetWorldCorners( fourCorners_g_ );
			var leftBottom = fourCorners_g_[ 0 ];
			var rightTop = fourCorners_g_[ 2 ];
			var size = new Vector2(
				rightTop.x - leftBottom.x,
				rightTop.y - leftBottom.y
			);
			return size;
			// return Vector2.Scale( tr.sizeDelta, tr.lossyScale );
		}
	}
}
