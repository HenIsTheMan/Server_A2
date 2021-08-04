namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseOutBack(float x) {
			return 1.0f + c3 * UnityEngine.Mathf.Pow(x - 1.0f, 3.0f) + c1 * UnityEngine.Mathf.Pow(x - 1.0f, 2.0f);
		}
	}
}