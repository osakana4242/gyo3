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

		public static void Init() {
			instance_ = new ScreenService();
			instance_.Calc();
		}

		public float Scale => scale_;
		public Vector2 GameResolution => new Vector2(width_, height_);
		public Vector2 GameMainSize => new Vector2(width_, mainHeight_);
		public Vector2 GameSubSize => new Vector2(width_, subHeight_);

		public void Calc() {
			width_ = 320f;
			scale_ = width_ / Screen.width;
			height_ = width_ * Screen.height / Screen.width;

			mainHeight_ = 240f;
			subHeight_ = height_ - mainHeight_;
		}

		public Vector2 Convert(Vector2 pos0, Camera camera) {
			var gameResolution = ScreenService.Instance.GameResolution;
			var pos1 = pos0 * ScreenService.Instance.Scale + new Vector2(gameResolution.x * -0.5f, gameResolution.y * -0.5f);
			var camPos = camera.transform.position;
			var pos2 = new Vector2(pos1.x + camPos.x, pos1.y + camPos.y);
			return pos2;
		}
	}
}
