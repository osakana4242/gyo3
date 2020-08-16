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

		public static void Init() {
			instance_ = new ScreenService();
			instance_.AdjustIfNeeded();
		}

		public float Scale => scale_;
		public Vector2 GameResolution => new Vector2(width_, height_);
		public Vector2 GameMainSize => new Vector2(width_, mainHeight_);
		public Vector2 GameSubSize => new Vector2(width_, subHeight_);

		public void Adjust(Vector2 sc) {

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
				height_ = width_ * sc.y / sc.x;

				mainHeight_ = Config.instance.screen.main.y;
				subHeight_ = height_ - mainHeight_;
				Main.Instance.camera.rect = new Rect(0f, 0f, 1f, 1f);
				Main.Instance.camera.orthographicSize = height_ * 0.5f;
			}
			var h = Config.instance.screen.size.y;
			var h2 = height_ - h;
			Main.Instance.camera.transform.position = new Vector3(0f, h * -0.25f - h2 * 0.5f, -10f);

		}

		public void AdjustIfNeeded() {
			var sc = new Vector2(Screen.width, Screen.height);
			if (sc_ == sc) return;
			sc_ = sc;
			Adjust(sc);
		}

		public Vector2 Convert(Vector2 pos0, Camera camera) {
			var gameResolution = ScreenService.Instance.GameResolution;
			var pos1 = pos0 * Scale + new Vector2(gameResolution.x * -0.5f, gameResolution.y * -0.5f);
			var camPos = camera.transform.position;

			var offsetX = ( width_ - (sc_.x * Scale) ) * 0.5f;
			var pos2 = new Vector2(pos1.x + camPos.x + offsetX, pos1.y + camPos.y);
			return pos2;
		}
	}
}
