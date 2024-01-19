using System.Collections;
using System.Collections.Generic;
using Osakana4242.UnityEngineExt;

namespace Osakana4242.Content {
	[System.Serializable]
	public class PlayerInfo {
		public Score score;
	}

	public struct AddScore {
		public readonly int value;
		public AddScore( int value ) {
			if ( value < 0 ) throw new System.ArgumentException( $"value: {value}" );
			this.value = value;
		}
		
		public override string ToString() => value.ToString();
	}

	public struct Score {
		public readonly int value;
		public Score( int value ) {
			if ( value < 0 ) throw new System.ArgumentException( $"value: {value}" );
			this.value = value;
		}

		public static Score operator+ ( Score a, AddScore addScore ) {
			return new Score( a.value + addScore.value );
		}

		public override string ToString() => value.ToString();
	}
}
