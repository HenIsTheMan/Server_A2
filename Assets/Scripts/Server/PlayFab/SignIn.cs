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

        [EnumIndices(typeof(SignInStatus)), SerializeField]
        private string[] signInMsgs;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignIn(): base() {
            usernameEmailInputField = null;
            passwordInputField = null;

            onLoginSuccess = null;
            onLoginFailure = null;

            signInMsgs = System.Array.Empty<string>();
        }

        static SignIn() {
        }

        #endregion

        #region Unity User Callback Event Funcs
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
            Console.Log(signInMsgs[(int)status]);
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("User Login Successful!");

            ShowSignInMsg(SignInStatus.Success);

            onLoginSuccess?.Invoke();
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("User Login Failed (" + error.GenerateErrorReport() + ")!");

            onLoginFailure?.Invoke();
        }
    }
}