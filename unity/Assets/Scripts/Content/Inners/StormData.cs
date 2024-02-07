using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using System.Threading;
using Osakana4242.Lib.AssetServices;
using Cysharp.Threading.Tasks;
using System;
namespace Osakana4242.Content.Inners {

	public class StormData : ScriptableObject {
		static StormData empty_g_;
		public static StormData Empty => (null == empty_g_) ? (empty_g_ = ScriptableObject.CreateInstance<StormData>()) : empty_g_;

		public StormEventData[] eventList = System.Array.Empty<StormEventData>();

		public void ForEachEvent<T>(HashSet<StormData> hashSet, T prm, Action<StormEventData, T> act) {
			ForEachEvent(this, hashSet, prm, act);
		}

		static void ForEachEvent<T>(StormData data, HashSet<StormData> hashSet, T prm, Action<StormEventData, T> act) {
			if (hashSet.Contains(data)) return;
			hashSet.Add(data);
			foreach (var evt in data.eventList) {
				if (evt.type == StormEventType.SetStorm) {
					ForEachEvent<T>(evt.GetSetStorm().storm, hashSet, prm, act);
				}
				act(evt, prm);
			}
		}
	}

	public enum StormEventType {
		None = 0,
		SpawnWave = 1,
		Telop = 2,
		WaitDestroyedAll = 3,
		Delay = 4,
		/// <summary>
		/// s0: stormName
		/// i0: railIndex. 0 or 1
		/// </summary>
		SetStorm = 5,
	}

	/// <summary>
	/// Telop STAGE1 START
	/// Wave wave_01
	/// Delay 1
	/// Wave wave_02
	/// </summary>
	[Serializable]
	public class StormEventData {
		public StormEventType type;
		public int i0;
		public string s0;
		public ScriptableObject so0;

		public Delay GetDelay() => new Delay(this);
		public SetStorm GetSetStorm() => new SetStorm(this);
		public SpawnWave GetSpawnWave() => new SpawnWave(this);

		public struct Delay {
			public float delay;

			public Delay(StormEventData data) {
				delay = data.i0 / 1000.0f;
			}
		}
		public struct SetStorm {
			public StormData storm;
			public int railIndex;

			public SetStorm(StormEventData data) {
				storm = (StormData)data.so0;
				if (null == storm) throw new ArgumentNullException("storm");
				railIndex = data.i0;
			}
		}

		public struct SpawnWave {
			public WaveData wave;

			public SpawnWave(StormEventData data) {
				wave = (WaveData)data.so0;
				if (null == wave) throw new ArgumentNullException("wave");
			}
		}
	}
}
