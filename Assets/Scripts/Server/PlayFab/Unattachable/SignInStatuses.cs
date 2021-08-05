namespace Server.General {
	internal static class SignInStatuses: object {
		internal enum Axis: byte {
			None,
			NoUsernameOrEmail,
			NoPassword,
			SpacesInUsernameOrEmail,
			Success,
			WrongUsername,
			WrongEmail,
			WrongPassword,
			Amt
		}
	}
}