namespace Server.PlayFab {
	internal static class SignUpStatuses: object {
		internal enum SignUpStatus: int {
			None,
			NoUsername,
			NoEmail,
			NoNewPassword,
			NoConfirmPassword,
			PasswordsNotMatching,
			SpacesInUsername,
			SpacesInEmail,
			InvalidUsernameLen,
			InvalidPasswordLen,
			UsernameHasInvalidChars,
			InvalidEmail,
			UsernameNotAvailable,
			EmailNotAvailable,
			Processing,
			Success,
			Amt
		}
	}
}