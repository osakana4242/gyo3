using System.Collections;
using System.Collections.Generic;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	[System.Serializable]
	public class PlayerInfo {
		public Score score;
		public PlayerStock stock = new PlayerStock(1);
		public WeaponChargeProgress weaponChargeProgress = new WeaponChargeProgress(0f, 1f);
	}

	public readonly struct PlayerStock {
		public readonly int restCount;
		public readonly int maxCount;
		public PlayerStock(int maxCount, int restCount) {
			if (maxCount < 0) throw new System.ArgumentException($"maxCount: {maxCount}");
			this.maxCount = maxCount;
			this.restCount = restCount;
		}

		public PlayerStock(int maxCount) : this(maxCount, maxCount) { }

		public PlayerStock Spawned() {
			if (IsEmpty())
				throw new System.Exception("empty");
			return new PlayerStock(restCount - 1, maxCount);
		}

		public bool IsEmpty() => restCount <= 0;
	}

	public readonly struct AddScore {
		public readonly int value;
		public AddScore(int value) {
			if (value < 0) throw new System.ArgumentException($"value: {value}");
			this.value = value;
		}

		public override string ToString() => value.ToString();
	}

	public readonly struct Score {
		public readonly int value;
		public Score(int value) {
			if (value < 0) throw new System.ArgumentException($"value: {value}");
			this.value = value;
		}

		public static Score operator +(Score a, AddScore addScore) {
			return new Score(a.value + addScore.value);
		}

		public override string ToString() => value.ToString();
	}
}
