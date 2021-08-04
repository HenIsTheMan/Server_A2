namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseInBounce(float x) {
			return 1.0f - EaseOutBounce(1.0f - x);
		}
	}
}