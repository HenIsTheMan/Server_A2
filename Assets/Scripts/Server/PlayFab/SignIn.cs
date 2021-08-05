using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Server.General.SignInStatuses;

namespace Server.PlayFab {
    internal sealed class SignIn: MonoBehaviour {
        #region Fields

        private bool canClick;

        [SerializeField]
        private TMP_InputField usernameEmailInputField;

        [SerializeField]
        private TMP_InputField passwordInputField;

        [SerializeField]
        private UnityEvent onLoginSuccess;

        [SerializeField]
        private UnityEvent onLoginFailure;

        [SerializeField]
        private TMP_Text signInMsgTmp;

        [SerializeField]
        private EllipsesControl signInEllipsesControl;

        [EnumIndices(typeof(SignInStatus)), SerializeField]
        private string[] signInMsgs;

        [EnumIndices(typeof(SignInStatus)), SerializeField]
        private Color[] signInMsgColors;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignIn(): base() {
            canClick = true;

            usernameEmailInputField = null;
            passwordInputField = null;

            onLoginSuccess = null;
            onLoginFailure = null;

            signInMsgTmp = null;
            signInEllipsesControl = null;
            signInMsgs = System.Array.Empty<string>();
            signInMsgColors = System.Array.Empty<Color>();
        }

        static SignIn() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            signInMsgTmp.text = string.Empty;
        }

        #endregion

        public void OnClick() {
            if(!canClick) {
                return;
            }
            canClick = false;

            string usernameOrEmail = usernameEmailInputField.text;
            SignInStatus status = SignInStatus.None;

            if(string.IsNullOrEmpty(usernameOrEmail)) {
                status = SignInStatus.NoUsernameOrEmail;
                goto ShowSignInMsg;
            }

            string password = passwordInputField.text;

            if(string.IsNullOrEmpty(password)) {
                status = SignInStatus.NoPassword;
                goto ShowSignInMsg;
            }

            if(usernameOrEmail.Contains(' ')) {
                status = SignInStatus.SpacesInUsernameOrEmail;
                goto ShowSignInMsg;
            }

            bool isEmail = true;
            int atIndex = usernameOrEmail.IndexOf('@');
            int dotIndex = usernameOrEmail.IndexOf('.');
            int usernameOrEmailLen = usernameOrEmail.Length;

            if(usernameOrEmail.Count(myChar => myChar == '@') != 1
                || usernameOrEmail.Count(myChar => myChar == '.') != 1
                || atIndex < 1
                || dotIndex < 3
                || atIndex > usernameOrEmailLen - 4
                || dotIndex > usernameOrEmailLen - 2
                || (atIndex >= dotIndex - 1)
            ) {
                isEmail = false;
            } else {
                string substr0 = usernameOrEmail.Substring(0, atIndex);
                string substr1 = usernameOrEmail.Substring(atIndex + 1, dotIndex - atIndex - 1);
                string substr2 = usernameOrEmail.Substring(dotIndex + 1, usernameOrEmailLen - dotIndex - 1);

                if(!substr0.All(char.IsLetterOrDigit)
                    || !substr1.All(char.IsLetterOrDigit)
                    || !substr2.All(char.IsLetterOrDigit)
                ) { //Diff from Sign Up (on purpose)
                    isEmail = false;
                }
            }

			if(!isEmail && (usernameOrEmailLen < 3 || usernameOrEmailLen > 20)) {
				status = SignInStatus.InvalidUsernameLen;
				goto ShowSignInMsg;
			}

            int passwordLen = password.Length;
            if(passwordLen < 6 || passwordLen > 100) {
                status = SignInStatus.InvalidPasswordLen;
                goto ShowSignInMsg;
            }

			if(isEmail) {
                LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest {
                    Email = usernameOrEmail,
                    Password = password
                };

                PlayFabClientAPI.LoginWithEmailAddress(
                    request,
                    OnLoginSuccess,
                    OnLoginFailure
                );

                signInEllipsesControl.enabled = true;
                ShowSignInMsg(SignInStatus.ProcessingWithEmail, false);
            } else {
                LoginWithPlayFabRequest request = new LoginWithPlayFabRequest {
                    Username = usernameOrEmail,
                    Password = password
                };

                PlayFabClientAPI.LoginWithPlayFab(
                    request,
                    OnLoginSuccess,
                    OnLoginFailure
                );

                signInEllipsesControl.enabled = true;
                ShowSignInMsg(SignInStatus.ProcessingWithUsername, false);
            }

            return;

        ShowSignInMsg:
            ShowSignInMsg(status);
        }

        private void ShowSignInMsg(SignInStatus status, bool shldSetCanClick = true) {
            if(shldSetCanClick) {
                canClick = true;
            }

            signInMsgTmp.text = signInMsgs[(int)status];
            signInMsgTmp.color = signInMsgColors[(int)status];
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("User Login Successful!");

            signInEllipsesControl.enabled = false;
            ShowSignInMsg(SignInStatus.Success);

            onLoginSuccess?.Invoke();
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("User Login Failed (" + error.GenerateErrorReport() + ")!");

            signInEllipsesControl.enabled = false;
            ShowSignInMsg(SignInStatus.Failure);

            onLoginFailure?.Invoke();
        }
    }
}