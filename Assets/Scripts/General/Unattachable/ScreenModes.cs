namespace Server.General {
	internal static class ScreenModes: object {
		internal enum ScreenMode: int {
			ExclusiveFullscreen,
			FullscreenWindow,
			MaximizedWindow,
			Windowed,
			Amt
		}
	}
}