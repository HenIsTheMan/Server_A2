namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseOutCirc(float x) {
			return UnityEngine.Mathf.Sqrt(1.0f - (x - 1.0f) * (x - 1.0f));
		}
	}
}