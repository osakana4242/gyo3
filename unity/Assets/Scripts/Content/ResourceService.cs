using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

namespace Osakana4242.Content {
	public class ResourceService {
		static ResourceService instance_;
		public static ResourceService Instance => instance_;

		public static void Init() {
			instance_ = new ResourceService();
		}

		public T Get<T>(string name) where T : Object {
			return Main.Instance.resourceData.Get<T>(name);
		}
	}
}
