namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseInBack(float x) {
			return c3 * x * x * x - c1 * x * x;
		}
	}
}