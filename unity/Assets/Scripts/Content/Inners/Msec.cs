using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using Osakana4242.UnityEngineUtil;
using Osakana4242.Lib.AssetServices;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Osakana4242.Content.Inners {

	[Serializable]
	public struct Msec : IEquatable<Msec>, IComparable<Msec> {
		public static readonly Msec Zero = new Msec(0);
		public static readonly Msec OneSec = new Msec(1000);
		public static readonly Msec Max = new Msec(int.MaxValue);
		[SerializeField] int value_;
		Msec(int value) {
			value_ = value;
		}
		public static Msec FromMsec(int msec) {
			return new Msec(msec);
		}
		public static Msec FromSeconds(float sec) {
			return new Msec(Mathf.RoundToInt(sec * 1000));
		}

		public static bool operator ==(Msec left, Msec right) =>
			left.Equals(right);
		public static bool operator !=(Msec left, Msec right) =>
			!left.Equals(right);
		public static bool operator <(Msec left, Msec right) =>
			left.CompareTo(right) < 0;
		public static bool operator >(Msec left, Msec right) =>
			0 < left.CompareTo(right);
		public static bool operator <=(Msec left, Msec right) =>
			left == right || left < right;
		public static bool operator >=(Msec left, Msec right) =>
			left == right || right < left;
		public static Msec operator +(Msec left, Msec right) =>
			new Msec(left.value_ + right.value_);
		public static Msec operator -(Msec left, Msec right) =>
			new Msec(left.value_ - right.value_);
		public static Msec operator *(Msec left, float scale) =>
			new Msec(Mathf.RoundToInt(left.value_ * scale));
		public static float operator /(Msec left, Msec right) =>
			left.value_ / right.value_;
		public int CompareTo(Msec other)
			=> value_.CompareTo(other.value_);
		public bool Equals(Msec other) =>
			value_.Equals(other.value_);
		public override bool Equals(object otherObject) {
			var other = otherObject as Msec?;
			if (null == other)
				return false;
			return Equals(other);
		}
		public override int GetHashCode() =>
			value_.GetHashCode();
		/// <summary>秒速をこの時間毎の速度に変換する係数</summary>
		public float PerSecToPerThis =>
			(float)value_ / (float)OneSec.value_;
		/// <summary>秒速に変換する係数</summary>
		public float PerThisToPerSec =>
			(float)OneSec.value_ / (float)value_;

#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(Msec))]
		public class MsecDrawer : PropertyDrawer {
			Msec model = default;

			static float LineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			static void PropertyField(ref Rect position, bool isLayout, SerializedProperty sp, GUIContent content = null) {
				if (!isLayout) EditorGUI.PropertyField(position, sp, content);
				position.y += LineHeight;
			}

			float Draw(Rect position, SerializedProperty property, GUIContent label, bool isLayout) {
				// https://docs.unity3d.com/ja/current/ScriptReference/PropertyDrawer.html

				var startY = position.y;
				if (!isLayout) EditorGUI.BeginProperty(position, label, property);
				{
					position.height = EditorGUIUtility.singleLineHeight;

					var spType = property.FindPropertyRelative(nameof(model.value_));
					PropertyField(ref position, isLayout, spType, label);

				}
				if (!isLayout) EditorGUI.EndProperty();
				return position.y - startY;
			}

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
				// https://docs.unity3d.com/ja/current/ScriptReference/PropertyDrawer.html
				Draw(position, property, label, false);
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				return Draw(new Rect(), property, label, true);
			}
		}
#endif
	}

}
