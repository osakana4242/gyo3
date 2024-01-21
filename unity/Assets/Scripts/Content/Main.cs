using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Content.Inners;

namespace Osakana4242.Content {
	public class Main : MonoBehaviour {
		static Main instance_;
		[SerializeField] public Camera screenACamera;
		[SerializeField] public Camera screenBCamera;
		[SerializeField] public ResourceData resourceData;
		[SerializeField] public Outers.HUD hud;
		[SerializeField] public UnityEngine.UI.RawImage rawImage;
		[SerializeField] public Inners.InnerMain inner;
		void Awake() {
			instance_ = this;
		}

		public static Main Instance => instance_;
		bool initialzied_;

		IEnumerator Start() {
			Physics.reuseCollisionCallbacks = true;
			Config.instance = Resources.Load<Config>("config");
			ScreenAService.Init();
			ResourceService.Init();
			inner.Init();
			hud.Init();
			initialzied_ = true;
			yield return null;
		}

		void FixedUpdate() {
			if ( !initialzied_ ) return;
			InputSystem.Update();
			inner.Update();
		}

		void Update() {
			ScreenAService.Instance.AdjustIfNeeded();
			hud.Update();
		}
	}
}
