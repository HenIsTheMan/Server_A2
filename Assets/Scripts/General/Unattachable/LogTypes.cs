namespace Server.General {
	internal static class LogTypes: object {
		internal enum LogType: int {
			Error,
			Assert,
			Warning,
			Log,
			Exception,
			Amt
		}
	}
}