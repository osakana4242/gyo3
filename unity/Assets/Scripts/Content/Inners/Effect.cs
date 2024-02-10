using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;
using Osakana4242.Lib.AssetServices;
using System.Threading.Tasks;

namespace Osakana4242.Content.Inners {
	public class Effect : MonoBehaviour {
		[SerializeField] float duration_;
		public Msec Duration => Msec.FromSeconds(duration_);
	}
}
