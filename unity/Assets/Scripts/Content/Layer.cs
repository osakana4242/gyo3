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

	}

	public enum CharaType {
		Player = 1,
		PlayerBullet = 2,
		Enemy = 3,
		EnemyBullet = 4,
	}

	public static class CharaTypeHelper {
		static readonly Dictionary<CharaType, int> layerDict_g_ = new Dictionary<CharaType, int>() {
			{ CharaType.Player, Layer.Player },
			{ CharaType.PlayerBullet, Layer.PlayerBullet },
			{ CharaType.Enemy, Layer.Enemy },
			{ CharaType.EnemyBullet, Layer.EnemyBullet },
		};
		public static int ToLayer(this CharaType self) => layerDict_g_[self];
	}
}
