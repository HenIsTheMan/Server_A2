using System.Collections.Generic;

namespace Server.General {
    internal static class ShuffleElements: object {
		internal static void Shuffle<T>(this IList<T> container) { //Fisher-Yates shuffle
			for(int i = container.Count - 1; i > 0; --i) {
				Math.Val.Swap(ref container, UnityEngine.Random.Range(0, i + 1), i);
			}
		}
	}
}