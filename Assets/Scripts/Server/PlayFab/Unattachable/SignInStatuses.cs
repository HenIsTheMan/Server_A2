namespace Server.PlayFab {
	internal static class SignInStatuses: object {
		internal enum SignInStatus: int {
			None,
			NoUsernameOrEmail,
			NoPassword,
			SpacesInUsernameOrEmail,
			InvalidUsernameLen,
			InvalidPasswordLen,
			ProcessingWithUsername,
			ProcessingWithEmail,
			Success,
			Failure,
			Amt
		}
	}
}