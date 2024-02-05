
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;

namespace Osakana4242.Content.Inners.CharaAIs {
	public class Empty : ICharaComponent {
		public static readonly Empty instance_g = new Empty();
		
		public void Update(Chara self) { }
	}
}
