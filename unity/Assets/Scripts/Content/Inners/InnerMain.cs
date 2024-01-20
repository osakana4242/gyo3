using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	[System.Serializable]
	public class InnerMain {
		static InnerMain instance_;
		[SerializeField] public BoxCollider bulletAliveArea;
		[SerializeField] public BoxCollider screenArea;
		public Stage stage = new Stage();
		public PlayerInfo playerInfo = new PlayerInfo();

		public static InnerMain Instance => instance_;

		public void Init() {
			CollisionService.Init();
			instance_ = this;
		}

		public void Update() {
			CollisionService.Instance.Update();
			stage.Update();
		}
	}
}
