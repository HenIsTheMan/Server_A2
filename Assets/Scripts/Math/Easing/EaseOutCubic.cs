﻿namespace Server.Math {
	internal static partial class Easing: object {
		#region Fields
		#endregion

		#region Properties
		#endregion

		public static float EaseOutCubic(float x) {
			return 1.0f - UnityEngine.Mathf.Pow(1.0f - x, 3.0f);
		}
	}
}