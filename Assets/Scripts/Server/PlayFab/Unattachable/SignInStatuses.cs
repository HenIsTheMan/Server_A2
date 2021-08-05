namespace Server.General {
	internal static class SignInStatuses: object {
		internal enum SignInStatus: int {
			None,
			NoUsernameOrEmail,
			NoPassword,
			SpacesInUsernameOrEmail,
			InvalidUsernameLen,
			InvalidPasswordLen,
			WithUsername, //Signing in
			WithEmail, //Signing in
			Success,
			Failure,
			Amt
		}
	}
}