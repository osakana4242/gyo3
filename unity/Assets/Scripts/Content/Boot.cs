using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	public class Boot : MonoBehaviour {
		[SerializeField]
		Camera camera_;
		[SerializeField]
		ParticleSystem.MinMaxGradient gradient;
		IEnumerator Start() {
			for (int i = 0, iCount = 240; i < iCount; ++i) {
				Debug.Log($"{i}");
				var t = ((float)i) / (iCount - 1);
				camera_.backgroundColor = gradient.Evaluate(t);
				yield return 0;
			}
			UnityEngine.SceneManagement.SceneManager.LoadScene("mainScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
	}
}
