namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseInOutBounce(float x) {
			return x < 0.5f ? (1.0f - EaseOutBounce(1.0f - 2.0f * x)) * 0.5f : (1.0f + EaseOutBounce(2.0f * x - 1.0f)) * 0.5f;
		}
	}
}