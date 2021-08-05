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

        [EnumIndices(typeof(SignInStatus)), SerializeField]
        private string[] signInMsgs;

        [EnumIndices(typeof(SignInStatus)), SerializeField]
        private Color[] signInMsgColors;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignIn(): base() {
            usernameEmailInputField = null;
            passwordInputField = null;

            onLoginSuccess = null;
            onLoginFailure = null;

            signInMsgTmp = null;
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

            LoginWithEmailAddressRequest req = new LoginWithEmailAddressRequest {
                Email = usernameEmailInputField.text,
                Password = passwordInputField.text 
            };

            PlayFabClientAPI.LoginWithEmailAddress(
                req,
                OnLoginSuccess,
                OnLoginFailure
            );

        ShowSignInMsg:
            ShowSignInMsg(status);
        }

        private void ShowSignInMsg(SignInStatus status) {
            signInMsgTmp.text = signInMsgs[(int)status];
            signInMsgTmp.color = signInMsgColors[(int)status];
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("User Login Successful!");

            ShowSignInMsg(SignInStatus.Success);

            onLoginSuccess?.Invoke();
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("User Login Failed (" + error.GenerateErrorReport() + ")!");

            switch(error.Error) {
                case PlayFabErrorCode.InvalidUsername:
                    ShowSignInMsg(SignInStatus.WrongUsername);
                    break;
                case PlayFabErrorCode.InvalidEmailAddress:
                    ShowSignInMsg(SignInStatus.WrongEmail);
                    break;
                case PlayFabErrorCode.InvalidPassword:
                    ShowSignInMsg(SignInStatus.WrongPassword);
                    break;
            }

            onLoginFailure?.Invoke();
        }
    }
}