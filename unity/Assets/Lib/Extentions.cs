using System.Collections.Generic;

namespace Osakana4242.SystemExt {
	public static class IListExt {
		public static int FindIndex_Ext<T1, T2>(this IReadOnlyList<T1> self, T2 prm, System.Func<T1, T2, bool> matcher) {
			for (int i = 0, iCount = self.Count; i < iCount; ++i) {
				var item = self[i];
				if (!matcher(item, prm)) continue;
				return i;
			}
			return -1;
		}
		public static int RemoveAll_Ext<T1, T2>(this IList<T1> self, T2 prm, System.Func<T1, T2, bool> matcher) {
			for (int i = self.Count - 1; 0 <= i; --i) {
				var item = self[i];
				if (!matcher(item, prm)) continue;
				self.RemoveAt(i);
			}
			return -1;
		}
	}
}

namespace Osakana4242.UnityEngineExt {
	using UnityEngine;

	public static class TransformExt {
		public static void ForEachChildWithSelf_Ext<T>(this Transform self, T prm, System.Action<Transform, T> func) {
			self.ForEachChild_Ext(prm, func);
			func(self, prm);
		}

		public static void ForEachChild_Ext<T>(this Transform self, T prm, System.Action<Transform, T> func) {
			for (int i = 0, iCount = self.childCount; i < iCount; ++i) {
				var item = self.GetChild(i);
				item.ForEachChild_Ext(prm, func);
				func(item, prm);
			}
		}
	}
}
