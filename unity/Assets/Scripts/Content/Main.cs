using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Main : MonoBehaviour {
		static Main instance_;
		[SerializeField] public new Camera camera;
		[SerializeField] public BoxCollider bulletAliveArea;
		[SerializeField] public BoxCollider screenArea;
		[SerializeField] public ResourceData resourceData;
		public Stage stage = new Stage();

		void Awake() {
			instance_ = this;
		}

		public static Main Instance => instance_;
		bool initialzied_;

		IEnumerator Start() {
			Physics.reuseCollisionCallbacks = true;
			Config.instance = Resources.Load<Config>("config");
			ScreenService.Init();
			ResourceService.Init();
			CollisionService.Init();
			stage.Init();
			// var t = ResourceService.Init();
			// while ( !t.IsCompleted ) {
			// 	yield return null;
			// }
			// Debug.Log( "r: " + t.Result );
			initialzied_ = true;
			yield return null;
		}

		void FixedUpdate() {
			if ( !initialzied_ ) return;
			CollisionService.Instance.Update();
			stage.Update();
		}

		void Update() {
			ScreenService.Instance.AdjustIfNeeded();
		}
	}
}
