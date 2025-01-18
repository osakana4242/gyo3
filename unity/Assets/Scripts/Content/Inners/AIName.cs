using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.SystemExt;
using Osakana4242.UnityEngineExt;
using System.ComponentModel;

namespace Osakana4242.Content.Inners {
	public sealed class AIName {
		static readonly Dictionary<HashKey, System.Func<Chara, ICharaComponent>> createFuncs_g_ = new Dictionary<HashKey, System.Func<Chara, ICharaComponent>> {
			{ "effect".ToHashKey_Ext(), _ => CharaAIs.Effect.instance_g },
			{ "enemy_test_rotation".ToHashKey_Ext(), _ => CharaAIs.EnemyTestRoation.instance_g },
			{ "enemy_1".ToHashKey_Ext(), _ => CharaAIs.Enemy1.instance_g },
			{ "enemy_2".ToHashKey_Ext(), _ => CharaAIs.Enemy2.instance_g },
			{ "enemy_3".ToHashKey_Ext(), _ => CharaAIs.Enemy3.instance_g },
			{ "enemy_4".ToHashKey_Ext(), _ => CharaAIs.Enemy4.instance_g },
			{ "enemy_5".ToHashKey_Ext(), _ => new CharaAIs.Enemy5() },
			{ "enemy_6".ToHashKey_Ext(), _ => CharaAIs.Enemy6.instance_g },
		};
		static readonly System.Func<Chara, ICharaComponent> emptyFunc_g_ = (_) => CharaAIs.Empty.instance_g;
		public static readonly AIName Empty = new AIName();

		public readonly string name = "";
		public readonly System.Func<Chara, ICharaComponent> createFunc;

		AIName() {
		}
		public AIName(string name) {
			if (string.IsNullOrEmpty(name))
				return;
			this.name = name;
			if (!createFuncs_g_.TryGetValue(name.ToHashKey_Ext(), out var func)) throw new System.ArgumentException($"not found. {name}");
			this.createFunc = func;
		}

		public bool IsEmpty() => string.IsNullOrEmpty(name);
	}


}
