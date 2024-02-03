using System;
using System.Collections.Generic;

namespace Osakana4242.SystemExt {

	public static class EnumUtil<T> {
		public static readonly T[] valueList = (T[])System.Enum.GetValues(typeof(T));
		public static T GetValueAt(int index) {
			return valueList[index];
		}
	}

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

	public static class DisposableUtil {
		public static void DisposeAndNullSafe<T>( ref T disposable ) where T : class, System.IDisposable {
			if ( null == disposable ) return;
			disposable.Dispose();
			disposable = null;
		}
	}
}

namespace Osakana4242.UnityEngineUtil {
	using UnityEngine;
	using Osakana4242.UnityEngineExt;

	public static class ObjectUtil {
		public static void DestroyAndNullSafe<T>( ref T obj ) where T : Object {
			if ( null != obj ) {
				Object.Destroy( obj );
			}
			obj = null;
		}
	}
	public static class Vector2Util {
		public static Vector2 FromDeg(float deg) {
			switch (deg) {
				case 0f: return new Vector2(1, 0f);
				case 90f: return new Vector2(0, 1f);
				case 180f: return new Vector2(-1, 0f);
				case 270f: return new Vector2(0, -1f);
			}
			var rad = (deg % 360f) * Mathf.Deg2Rad;
			return new Vector2(
				Mathf.Cos(rad),
				Mathf.Sin(rad)
			);
		}

		public static Vector2 MoveTowardsByAngle(Vector2 vec, Vector2 toTargetVec, float maxDeltaAngle) {
			var right = new Vector2(1f, 0f);
			var a1 = vec.ToAngle_Ext();
			var a2 = toTargetVec.ToAngle_Ext();
			var a3 = Mathf.MoveTowardsAngle(a1, a2, maxDeltaAngle);
			var v = Vector2Util.FromDeg(a3);
			return v;
		}

	}
}

namespace Osakana4242.UnityEngineExt {
	using UnityEngine;

	public static class Vector2Ext {
		public static float ToAngle_Ext(this in Vector2 self) {
			return Mathf.Atan2(self.y, self.x) * Mathf.Rad2Deg;
		}
	}

	public static class Vector3Ext {
		public static Vector3 ToXY0_Ext(this in Vector3 self) {
			return new Vector3(self.x, self.y, 0f);
		}
	}

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

	public static class RectTransformExt {
		static Vector3[] fourCorners_g_ = new Vector3[4];

		public static Vector2 GetWorldSizeXY_Ext(this RectTransform tr) {
			tr.GetWorldCorners(fourCorners_g_);
			var leftBottom = fourCorners_g_[0];
			var rightTop = fourCorners_g_[2];
			var size = new Vector2(
				rightTop.x - leftBottom.x,
				rightTop.y - leftBottom.y
			);
			return size;
			// return Vector2.Scale( tr.sizeDelta, tr.lossyScale );
		}
	}
}
