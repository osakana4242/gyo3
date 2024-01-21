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

		public static InnerMain Instance => instance_;

		State state_;
		StateTime stateTime_;
		GameObject title_;
		GameObject gameOver_;
		public bool hasExitRequest;

		public void Init() {
			CollisionService.Init();
			instance_ = this;
		}

		public bool CanExit() {
			switch ( state_ ) {
				case State.Stage: return true;
			}
			return false;
		}

		public void Update() {
			var ret = UpdateState();
			if ( ret.hasNextState ) {
				state_ = ret.nextState;
				stateTime_ = new StateTime( 0f, 0f );
			}

			CollisionService.Instance.Update();
			stage.Update();
		}


		( State nextState, bool hasNextState ) UpdateState() {
			stateTime_ = stateTime_.Evalute( Time.deltaTime );
			switch ( state_ ) {
				case State.None: {
					return ( State.TitleEnter, true );
				}
				case State.TitleEnter: {
					var titlePrefab = Main.Instance.resourceData.Get<GameObject>( ResourceNames.TITLE_PREFAB );
					title_ = GameObject.Instantiate( titlePrefab, InnerMain.instance_.hudTr );
					stage.Init();
					return ( State.Title, true );
				}
				case State.Title: {
					if ( stateTime_.time < 1f ) {
						// 操作不能時間
						break;
					}
					if ( !Pointer.current.press.isPressed ) {
						break;
					}
					GameObject.Destroy( title_ );
					title_ = null;
					return ( State.StageEnter, true );
				}
				case State.StageEnter: {
					hasExitRequest = false;
					stage.Init();
					stage.AddPlayerIfNeeded();
					return ( State.Stage, true );
				}
				case State.Stage: {
					if ( hasExitRequest ) {
						hasExitRequest = false;
						return ( State.TitleEnter, true );
					}
					if ( stage.ExistsPlayer() ) {
						break;
					}
					return ( State.GameOverEnter, true );
				}
				case State.GameOverEnter: {
					var gameOverPrefab = Main.Instance.resourceData.Get<GameObject>( ResourceNames.GAME_OVER_PREFAB );
					gameOver_ = GameObject.Instantiate( gameOverPrefab, InnerMain.instance_.hudTr );
					return ( State.GameOver, true );
				}
				case State.GameOver: {
					if ( stateTime_.time < 2f ) {
						break;
					}
					GameObject.Destroy( gameOver_ );
					gameOver_ = null;
					return ( State.TitleEnter, true );
				}
			}
			return ( state_, false );
		}

		struct StateTime {
			public readonly float time;
			public readonly float deltaTime;

			public StateTime( float time, float deltaTime ) {
				this.time = time;
				this.deltaTime = deltaTime;
			}

			public float PrevTime => time - deltaTime;

			public StateTime Evalute( float deltaTime ) {
				return new StateTime( time + deltaTime, deltaTime );
			}
		}

		public enum State {
			None,
			TitleEnter,
			Title,
			StageEnter,
			Stage,
			GameOverEnter,
			GameOver,
		}
	}
}
