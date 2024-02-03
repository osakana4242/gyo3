using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Content.Inners;
using System.Threading;
using Osakana4242.AssetServices;

namespace Osakana4242.Content {
	public class Main : MonoBehaviour {
		static Main instance_;
		[SerializeField] public Camera screenACamera;
		[SerializeField] public Camera screenBCamera;
		[SerializeField] public Outers.HUD hud;
		[SerializeField] public UnityEngine.UI.RawImage rawImage;
		[SerializeField] public Inners.InnerMain inner;
		readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		void Awake() {
			instance_ = this;
		}

		void OnDestroy() {
			AssetService.Instance.Dispose();
		}

		public static Main Instance => instance_;
		bool initialzied_;

		async Task Start() {
			AssetService.Init();
			Config.instance = await AssetService.Instance.GetAsync<Config>(AssetInfos.CONFIG_ASSET, cancellationTokenSource.Token);
			Physics.reuseCollisionCallbacks = true;

			ScreenAService.Init();
			inner.Init();
			hud.Init();
			initialzied_ = true;
		}

		void FixedUpdate() {
			if (!initialzied_) return;
			InputSystem.Update();
			inner.Update();
		}

		void Update() {
			if (!initialzied_) return;
			ScreenAService.Instance.AdjustIfNeeded();
			hud.Update();
		}
	}
}
