using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;

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
				listAdd.Add(chara);
			}

			void Add_(Chara chara) {
					dict.Add(chara.data.id, chara);
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

		public class Wave {
			public float time;
			public float interval = 0.5f;
			public void Update() {
				if (time < interval) {
					time += Time.deltaTime;
				} else {
					time = 0f;
					var enemy = CharaFactory.CreateEnemy();
					Main.Instance.stage.charaBank.Add(enemy);
				}
			}
		}
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
}
