
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Osakana4242.Content.Inners {

	[System.Serializable]
	public class CharaBank {
		public Dictionary<int, Chara> dict = new Dictionary<int, Chara>();
		int autoIncrement;
		List<int> listCache = new List<int>();
		List<(Chara, OnCharaEventDelegate)> listAdd = new List<(Chara, OnCharaEventDelegate)>();
		List<Chara> updateList = new List<Chara>();
		public int CreateId() => ++autoIncrement;
		int playerId;
		static readonly OnCharaEventDelegate emptyFunc_g = _ => false;
		public void Add(Chara chara) {
			chara.gameObject.SetActive(false);
			listAdd.Add((chara, emptyFunc_g));
		}

		public void Add(Chara chara, OnCharaEventDelegate act) {
			chara.gameObject.SetActive(false);
			listAdd.Add((chara, act));
		}

		void Add_((Chara chara, OnCharaEventDelegate func) param) {
			var chara = param.chara;
			dict.Add(chara.data.id, chara);
			updateList.Add(chara);
			chara.gameObject.SetActive(true);
			chara.ApplyTransform();

			chara.data.position = chara.transform.position;
			chara.data.spawnedTime = Stage.Current.time.time - Stage.Current.time.dt;
			chara.data.spawnedPosition = chara.data.position;

			if (chara.data.charaType == CharaType.Player) {
				InnerMain.Instance.playerInfo.stock = InnerMain.Instance.playerInfo.stock.Spawned();
				playerId = chara.data.id;
			}
			chara.data.observerList.Add(param.func);
			chara.data.FireEvent(CharaEventType.Added);
		}

		public void Update() {
			updateList.Clear();
			RemoveAll(this, (_item, _) => _item.data.removeRequested);
			ForEach(this, (_chara, _this) => _this.updateList.Add(_chara));
			updateList.ForEach(_chara => _chara.ManualUpdate());
			updateList.Clear();

			// 追加分.
			for (; 0 < listAdd.Count;) {
				FixAdd();
				updateList.ForEach(_chara => _chara.ManualUpdate());
				updateList.Clear();
			}
			RemoveAll(this, (_item, _) => _item.data.removeRequested);

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

		public void RemoveAll<T>(T prm, System.Func<Chara, T, bool> matcher) {
			foreach (var kv in dict) {
				var chara = kv.Value;
				if (!matcher(chara, prm)) continue;
				chara.data.FireEvent(CharaEventType.Removed);
				UnityEngine.Object.Destroy(chara.gameObject);
				listCache.Add(kv.Key);
			}
			foreach (var key in listCache) {
				var obj = dict[key];
				dict.Remove(key);
			}
			listCache.Clear();
		}

		public bool TryGetPlayer(out Chara chara) {
			return dict.TryGetValue(playerId, out chara);
		}

		public bool TryGetEnemy(out Chara chara) {
			foreach (var kv in dict) {
				var item = kv.Value;
				if (item.data.charaType == CharaType.Enemy) {
					chara = item;
					return true;
				}
			}
			chara = null;
			return false;
		}
	}

}
