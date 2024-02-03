using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Layer {
		public static int Player = UnityEngine.LayerMask.NameToLayer("Player");
		public static int PlayerBullet = UnityEngine.LayerMask.NameToLayer("PlayerBullet");
		public static int Enemy = UnityEngine.LayerMask.NameToLayer("Enemy");
		public static int EnemyBullet = UnityEngine.LayerMask.NameToLayer("EnemyBullet");
		public static int Effect = UnityEngine.LayerMask.NameToLayer("Effect");

	}

	public enum CharaType {
		Player = 1,
		PlayerBullet = 2,
		Enemy = 3,
		EnemyBullet = 4,
		Effect = 5,
	}

	public static class CharaTypeHelper {
		static readonly Dictionary<CharaType, int> layerDict_g_ = new Dictionary<CharaType, int>() {
			{ CharaType.Player, Layer.Player },
			{ CharaType.PlayerBullet, Layer.PlayerBullet },
			{ CharaType.Enemy, Layer.Enemy },
			{ CharaType.EnemyBullet, Layer.EnemyBullet },
			{ CharaType.Effect, Layer.Effect },
		};
		static readonly Dictionary<CharaType, int> sortingOrderDict_g_ = new Dictionary<CharaType, int>() {
			{ CharaType.Enemy, 10 },
			{ CharaType.Player, 20 },
			{ CharaType.Effect, 30 },
			{ CharaType.PlayerBullet, 40 },
			{ CharaType.EnemyBullet, 50 },
		};
		public static int GetLayer(this CharaType self) => layerDict_g_[self];
		public static int GetSortingOrder(this CharaType self) => sortingOrderDict_g_[self];
	}
}
