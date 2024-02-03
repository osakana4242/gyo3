using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using Osakana4242.SystemExt;
using System.Linq;
using System.Threading;
using Osakana4242.Lib.AssetServices;

namespace Osakana4242.Content.Inners {
	public static class IEnumrableExt {
		public static IEnumerable<T> ForEach_Ext<T>(this IEnumerable<T> self) {
			for (var e = self.GetEnumerator(); e.MoveNext();) { }
			return self;
		}

		public static IEnumerable ForEach_Ext<T>(this IEnumerable<T> self, System.Action<T> func) {
			for (var e = self.GetEnumerator(); e.MoveNext();) {
				func(e.Current);
			}
			return self;
		}
	}
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
			sm_.Add(CreateTitleState());
			sm_.Add(CreateStageReadyState());
			sm_.Add(CreateStageState());
			sm_.Add(CreateGameoverState());
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

		StateBase CreateTitleState() {
			ElapsedTime elapsedTime = default;
			GameObject title = null;
			GameObject touchToStart = null;
			Task titleTask = null;
			CancellationTokenSource cancellationTokenSource = default;

			return new StateBase() {
				Id = StateId.Title,
				Enter = () => {
					cancellationTokenSource = new();
					titleTask = loadAsync(cancellationTokenSource.Token);
					stage.Init();
					elapsedTime = ElapsedTime.empty;
				},
				Update = () => {
					elapsedTime = elapsedTime.Evaluete(Time.deltaTime);
					updateAnim();
					if (elapsedTime.value < 1f) return;
					if (!Pointer.current.press.isPressed) return;
					sm_.Transition(StateId.StageReady);
				},
				Exit = () => {
					cancellationTokenSource.Cancel();
					DisposableUtil.DisposeAndNullSafe(ref cancellationTokenSource);
					ObjectUtil.DestroyAndNullSafe(ref title);
					ObjectUtil.DestroyAndNullSafe(ref touchToStart);
				},
			};

			async Task loadAsync(CancellationToken cancellationToken) {
				var titlePrefab = AssetService.Instance.GetAsync<GameObject>(AssetInfos.TITLE_PREFAB, cancellationToken);
				var touchToStartPrefab = AssetService.Instance.GetAsync<GameObject>(AssetInfos.TITLE_TOUCH_TO_START_PREFAB, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();
				title = GameObject.Instantiate(await titlePrefab, InnerMain.instance_.hudTr);
				touchToStart = GameObject.Instantiate(await touchToStartPrefab, InnerMain.instance_.hudTr);
			}

			void updateAnim() {
				if (null == touchToStart) return;
				var touchToStartAnim = touchToStart.GetComponent<Animation>();
				var targetAnimInfo = elapsedTime.value < 1f ?
					AssetInfos.TITLE_TOUCH_TO_START_HIDDEN_ANIM :
					AssetInfos.TITLE_TOUCH_TO_START_IDLE_ANIM;
				if (!touchToStartAnim.IsPlaying(targetAnimInfo.AnimationClipName)) {
					touchToStartAnim.Play(targetAnimInfo.AnimationClipName);
				}
			}
		}

		StateBase CreateStageReadyState() {
			return new StateBase() {
				Id = StateId.StageReady,
				Enter = () => {
					stage.Init();
				},
				Update = () => {
					if (stage.IsLoading()) return;
					if (stage.IsLoading()) throw new System.Exception("stage が末ロード");
					sm_.Transition(StateId.Stage);
				},
				Exit = () => {
				},
			};
		}

		StateBase CreateStageState() {
			return new StateBase() {
				Id = StateId.Stage,
				Enter = () => {
					if (stage.IsLoading()) throw new System.Exception("stage が末ロード");
					hasExitRequest = false;
					playerInfo.stock = new PlayerStock(1);
					if (stage.IsLoading()) throw new System.Exception("stage が末ロード");
				},
				Update = () => {
					stage.AddPlayerIfNeeded();
					if (hasExitRequest) {
						hasExitRequest = false;
						sm_.Transition(StateId.Title);
						return;
					}
					if (!playerInfo.stock.IsEmpty())
						return;
					if (stage.ExistsPlayer())
						return;

					sm_.Transition(StateId.Gameover);
				},
				Exit = () => {
				},
			};
		}

		StateBase CreateGameoverState() {
			ElapsedTime time = default;
			GameObject gameOver = null;
			CancellationTokenSource cancellationTokenSource = default;

			return new StateBase() {
				Id = StateId.Gameover,
				Enter = () => {
					cancellationTokenSource = new();
					time = ElapsedTime.empty;
					_ = loadAsync(cancellationTokenSource.Token);
				},
				Update = () => {
					time = time.Evaluete(Time.deltaTime);
					if (time.value < 2f) return;
					sm_.Transition(StateId.Title);
				},
				Exit = () => {
					cancellationTokenSource.Cancel();
					DisposableUtil.DisposeAndNullSafe(ref cancellationTokenSource);
					ObjectUtil.DestroyAndNullSafe(ref gameOver);
				},
			};

			async Task loadAsync(CancellationToken cancellationToken) {
				var gameOverPrefab = AssetService.Instance.GetAsync<GameObject>(AssetInfos.GAME_OVER_PREFAB, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();
				gameOver = GameObject.Instantiate(await gameOverPrefab, InnerMain.instance_.hudTr);
			}
		}

		public static class StateId {
			public const int Title = 1;
			public const int StageReady = 2;
			public const int Stage = 3;
			public const int Gameover = 4;
		}
	}

	public readonly struct ElapsedTime {
		public static readonly ElapsedTime empty = FromSeconds(0);
		public readonly float value;

		public ElapsedTime(float value) {
			this.value = value;
		}

		public ElapsedTime Evaluete(float dt) {
			return new ElapsedTime(value + dt);
		}

		public static ElapsedTime FromSeconds(float seconds) {
			return new ElapsedTime(seconds);
		}
	}
}
