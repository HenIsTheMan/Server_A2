namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseOutExpo(float x) {
			return UnityEngine.Mathf.Approximately(x, 1.0f) ? 1.0f : 1.0f - UnityEngine.Mathf.Pow(2.0f, -10.0f * x);
		}
	}
}