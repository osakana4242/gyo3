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
}
