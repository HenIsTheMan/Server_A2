namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseInQuint(float x) {
			return x * x * x * x * x;
		}
	}
}