namespace Server.General {
	internal static class SignInStatuses: object {
		internal enum SignInStatus: byte {
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