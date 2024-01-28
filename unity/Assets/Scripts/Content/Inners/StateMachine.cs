using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {

	public class StateMachine {
		readonly Dictionary<int, StateBase> stateDict = new Dictionary<int, StateBase>();
		StateBase state_ = StateBase.Empty;
		bool exit_;

		public void Clear() {
			Transition(StateBase.Empty);
			stateDict.Clear();
		}

		public bool Has(int stateId) => state_.Id == stateId;
		public void Update() {
			state_.Update();
		}
		public void Transition(int stateId) {
			if (!stateDict.TryGetValue(stateId, out var nextState))
				throw new System.Exception($"次の State が未登録. stateId: {stateId}");
			Transition(nextState);
		}
		void Transition(StateBase nextState) {
			if (exit_) throw new System.Exception($"Exit 中の Transition 禁止");
			exit_ = true;
			state_.Exit();
			exit_ = false;
			Debug.Log( $"state: {state_.Id} to {nextState.Id}" );
			state_ = nextState;
			state_.Enter();
		}

		public void Add(StateBase state) {
			stateDict.Add(state.Id, state);
		}
	}

	public class StateBase {
		public static readonly Func EmptyFunc = () => { };
		public static readonly StateBase Empty = new();
		public int Id = 0;
		public Func Enter = EmptyFunc;
		public Func Exit = EmptyFunc;
		public Func Update = EmptyFunc;

		public delegate void Func();
	}

}
