using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Osakana4242.Content {
	public class Bullet : MonoBehaviour {

		Vector3 cpos_;
		public Vector3 velocity;

		void OnTriggerEnter(Collider col) {
			Debug.Log("self: " + name + ", col: " + col);
		}
		void OnCollisionEnter(Collision collision) {
			
			Debug.Log("self: " + name + ", col: " + collision);
			GameObject.Destroy(gameObject);
			GameObject.Destroy(collision.gameObject);
		}

		void FixedUpdate() {
			cpos_ = transform.position;

			// 移動.
			var delta = velocity * Time.deltaTime;
			cpos_ += delta;

			// エリア外.
			var area = Main.Instance.bulletAliveArea;
			var a = area.bounds;
			var b = GetComponentInChildren<Collider>().bounds;

			if (!a.Intersects(b)) {
				GameObject.Destroy(gameObject);
			}


			ApplyTransform();
		}

		void ApplyTransform() {
			transform.position = cpos_;
		}


	}
}