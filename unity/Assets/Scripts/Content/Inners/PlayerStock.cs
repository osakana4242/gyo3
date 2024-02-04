using System.Collections;
using System.Collections.Generic;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content.Inners {
	public readonly struct PlayerStock {
		public readonly int restCount;
		public readonly int maxCount;
		public PlayerStock(int restCount, int maxCount) {
			if (maxCount < 0) throw new System.ArgumentException($"maxCount: {maxCount}");
			if (maxCount < restCount) throw new System.ArgumentException($"maxCount: {maxCount}, restCount: {restCount}");
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
}
