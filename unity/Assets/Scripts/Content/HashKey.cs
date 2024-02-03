using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osakana4242.UnityEngineExt;
using System;

namespace Osakana4242.Content {
	public readonly struct HashKey : IEquatable<HashKey> {

#if UNITY_EDITOR
		static readonly HashSet<HashKey> editor_dict_g = new HashSet<HashKey>();
#endif

		public readonly int value;
		public readonly string str;

		HashKey(string str) {
			var hashCode = str.GetHashCode();
			this.value = hashCode;
			this.str = str;
		}

		public static HashKey ToHashKey(string s) {
			var key = new HashKey(s);
#if UNITY_EDITOR
			if (editor_dict_g.TryGetValue(key, out var key2)) {
				if (key.str != key2.str) throw new System.Exception($"キーの衝突を検知. key: {key.value}, str1: {key.str}, str2: {key2.str}");
			}
			editor_dict_g.Add(key);
#endif
			return key;
		}

		public override int GetHashCode() => value;
		public override bool Equals(object otherObj) {
			var otherOrNull = otherObj as HashKey?;
			if (null == otherOrNull) return false;
			return this.Equals(otherOrNull.Value);
		}

		public bool Equals(HashKey other) => this.value == other.value;

		public static bool operator ==(HashKey left, HashKey right) => left.Equals(right);
		public static bool operator !=(HashKey left, HashKey right) => !left.Equals(right);
	}

	public static class HashKeyStringExt {
		public static HashKey ToHashKey_Ext(this string s) => HashKey.ToHashKey(s);
	}
}
