using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEnginUtil;

namespace Osakana4242.Content {
	[System.Serializable]
	public class Stage {

		public Wave wave;

		public CharaBank charaBank = new CharaBank();
		public StageTime time = new StageTime();

		public static Stage Current => Main.Instance.stage;

		public Stage() {
			wave = new Wave();

		}

		public void Init() {
			var player = CharaFactory.CreatePlayer();
			Main.Instance.stage.charaBank.Add(player);
		}

		public void Update() {
			charaBank.FixAdd();
			charaBank.RemoveAll(this, (_item, _) => _item == null);
			charaBank.ForEach(this, (_chara, _) => _chara.ManualUpdate());
			if (charaBank.TryGetPlayer(out var player)) {
				player.GetComponent<Player>().ManualUpdate(player);
			} else {
				if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) {
					player = CharaFactory.CreatePlayer();
					Main.Instance.stage.charaBank.Add(player);
				}

			}
			wave.Update();


			time.Update(UnityEngine.Time.deltaTime);
		}

		[System.Serializable]
		public class CharaBank {
			public SortedDictionary<int, Chara> dict = new SortedDictionary<int, Chara>();
			int autoIncrement;
			List<int> listCache = new List<int>();
			List<Chara> listAdd = new List<Chara>();
			public int CreateId() => ++autoIncrement;
			int playerId;
			public void Add(Chara chara) {
				chara.gameObject.SetActive(false);
				listAdd.Add(chara);
			}

			void Add_(Chara chara) {
				dict.Add(chara.data.id, chara);
				chara.gameObject.SetActive(true);
				chara.ApplyTransform();
				if (chara.data.layer == Layer.Player) {
					playerId = chara.data.id;
				}
			}

			public void FixAdd() {
				foreach (var item in listAdd) {
					Add_(item);
				}
				listAdd.Clear();
			}

			public void ForEach<T>(T prm, System.Action<Chara, T> act) {
				foreach (var kv in dict) {
					act(kv.Value, prm);
				}
			}

			public void RemoveAll<T>(T prm, System.Func<Chara, T, bool> act) {
				foreach (var kv in dict) {
					if (!act(kv.Value, prm)) continue;
					listCache.Add(kv.Key);
				}
				foreach (var key in listCache) {
					dict.Remove(key);
				}
				listCache.Clear();
			}

			public Chara TryGetPlayer(out Chara chara) {
				dict.TryGetValue(playerId, out chara);
				return chara;
			}
		}

		[System.Serializable]
		public class Wave {
			public float startTime;
			public float time;
			public WaveData data;

			public void Update() {
				if (startTime == 0f) {
					startTime = Stage.Current.time.time;
				}
				var preTime = time;
				time = Stage.Current.time.time - startTime;

				for (int i = 0, iCount = data.eventList.Length; i < iCount; ++i) {
					var row = data.eventList[i];
					if (!TimeEventData.TryGetEvent(row.startTime / 1000f, 0f, preTime, time, out var eventData)) continue;
					var pos = row.position * 16f;
					var enemy = CharaFactory.CreateEnemy(row.enemyName);
					enemy.data.position = pos;
					var vec = Vector2Util.FromDeg(row.angle);
					enemy.data.rotation = Quaternion.LookRotation((Vector3)vec);
					enemy.data.velocity = enemy.data.rotation * Vector3.forward;

					Main.Instance.stage.charaBank.Add(enemy);
				}
			}
		}
	}

	[System.Serializable]
	public class WaveEventData {
		public float startTime;
		public string enemyName;
		public Vector2 position;
		// 0: 右, 90 下, 180: 左, 270: 上
		public float angle;
	}

	[System.Serializable]
	public class StageTime {
		public float time;
		public float dt;

		public void Update(float dt) {
			this.dt = dt;
			this.time += dt;
		}
	}

	public enum TimeEventType {
		None,
		Enter,
		Loop,
		Exit,
	}

	public struct TimeEventData {
		public TimeEventType type;
		public float progress;
		public float time;

		public static bool TryGetEvent(float startTime, float duration, float preTime, float currentTime, out TimeEventData evtData) {
			evtData = new TimeEventData();
			evtData.type = TimeEventType.None;
			float left = startTime;
			float right = left + duration;
			if (currentTime < left) {
				return false;
			}
			if (right <= preTime) {
				return false;
			}
			if (preTime < left && left <= currentTime) {
				evtData.type = TimeEventType.Enter;
				return true;
			}
			if (preTime < right && right <= currentTime) {
				evtData.type = TimeEventType.Exit;
				evtData.time = duration;
				evtData.progress = 1f;
				return true;
			}
			evtData.type = TimeEventType.Loop;
			evtData.time = currentTime - startTime;
			evtData.progress = evtData.time / duration;
			return true;
		}
	}


}
