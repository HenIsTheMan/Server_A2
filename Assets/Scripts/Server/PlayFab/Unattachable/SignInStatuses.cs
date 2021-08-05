namespace Server.General {
	internal static class SignInStatuses: object {
		internal enum SignInStatus: int {
			None,
			NoUsernameOrEmail,
			NoPassword,
			SpacesInUsernameOrEmail,
			WithUsername, //Signing in
			WithEmail, //Signing in
			Success,
			WrongUsername,
			WrongEmail,
			WrongPassword,
			Amt
		}
	}
}