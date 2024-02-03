using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;
using System.Threading.Tasks;

namespace Osakana4242.Content.Inners {
	public class CharaInfo : ScriptableObject {
		public CharaType charaType;
		public string comment;
		public int hp;
		public bool hasDeadArea;
		public bool hasBlast;
		public string modelName;
		public string aiName;
	}
}
