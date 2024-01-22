using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	[System.Serializable]
	public class InnerMain {
		static InnerMain instance_;
		[SerializeField] public RectTransform hudTr;
		[SerializeField] public BoxCollider bulletAliveArea;
		[SerializeField] public BoxCollider screenArea;
		[SerializeField] public BoxCollider playerMovableArea;
		public Stage stage = new Stage();
		public PlayerInfo playerInfo = new PlayerInfo();
		public bool hasExitRequest;
		readonly StateMachine sm_;
		
		public static InnerMain Instance => instance_;

		public InnerMain() {
			sm_ = new StateMachine();
			sm_.Add(createTitleState());
			sm_.Add(createStageState());
			sm_.Add(createGameoverState());
		}

		public void Init() {
			CollisionService.Init();
			instance_ = this;
			sm_.Transition(StateId.Title);
		}

		public bool CanExit() {
			if (sm_.Has(StateId.Stage)) return true;
			return false;
		}

		public void Update() {
			sm_.Update();
			CollisionService.Instance.Update();
			stage.Update();
		}

		StateBase createTitleState() {
			float time = 0f;
			GameObject title = null;

			return new StateBase() {
				Id = StateId.Title,
				Enter = () => {
					var titlePrefab = Main.Instance.resourceData.Get<GameObject>(ResourceNames.TITLE_PREFAB);
					title = GameObject.Instantiate(titlePrefab, InnerMain.instance_.hudTr);
					stage.Init();
					time = 0f;
				},
				Update = () => {
					time += Time.deltaTime;
					if (time < 1f) return;
					if (!Pointer.current.press.isPressed) return;
					sm_.Transition(StateId.Stage);
				},
				Exit = () => {
					GameObject.Destroy(title);
					title = null;
				},
			};
		}

		StateBase createStageState() {
			return new StateBase() {
				Id = StateId.Stage,
				Enter = () => {
					hasExitRequest = false;
					stage.Init();
					stage.AddPlayerIfNeeded();
				},
				Update = () => {
					if (hasExitRequest) {
						hasExitRequest = false;
						sm_.Transition(StateId.Title);
						return;
					}
					if (stage.ExistsPlayer()) {
						return;
					}
					sm_.Transition(StateId.Gameover);
				},
				Exit = () => {
				},
			};
		}

		StateBase createGameoverState() {
			var time = 0f;
			GameObject gameOver = null;

			return new StateBase() {
				Id = StateId.Gameover,
				Enter = () => {
					time = 0f;
					var gameOverPrefab = Main.Instance.resourceData.Get<GameObject>(ResourceNames.GAME_OVER_PREFAB);
					gameOver = GameObject.Instantiate(gameOverPrefab, InnerMain.instance_.hudTr);
				},
				Update = () => {
					time += Time.deltaTime;
					if (time < 2f) return;
					sm_.Transition(StateId.Title);
				},
				Exit = () => {
					GameObject.Destroy(gameOver);
					gameOver = null;
				},
			};
		}

		public static class StateId {
			public const int Title = 1;
			public const int Stage = 2;
			public const int Gameover = 3;
		}
	}
}
