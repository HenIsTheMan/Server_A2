namespace Server.General {
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
			EmailHasInvalidChars,
			UsernameNotUnique,
			EmailNotUnique,
			Success,
			Amt
		}
	}
}