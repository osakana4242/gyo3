
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Bullet : ICharaComponent {
		public float speed;

		public void Update(Chara self) {
			self.data.velocity = self.data.rotation * Vector3.forward * speed;
		}
	}

}
