using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Server.Ref {
    internal sealed class PlayFabLogin: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal PlayFabLogin(): base() {
        }

        static PlayFabLogin() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        //private void Start() {
        //    if(string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) {
        //        /*
        //        Please change the titleId below to your own titleId from PlayFab Game Manager.
        //        If you have already set the value in the Editor Extensions, this can be skipped.
        //        */
        //        PlayFabSettings.staticSettings.TitleId = "42";
        //    }
        //    var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //    PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        //}

        #endregion

        public void OnLoginClicked() {
            LoginWithEmailAddressRequest req = new LoginWithEmailAddressRequest {
                Email = "jimeroyesyes@test.com",
                Password = "123456"
            };
            PlayFabClientAPI.LoginWithEmailAddress(req,
                // Another way to register the callback function to handle
                // success and failure cases
                OnLoginSuccess, // Function defined below
                OnLoginFailure // Function defined below
            );
        }

        private void OnLoginSuccess(LoginResult result) {
            Debug.Log("Login is successful");
        }

        private void OnLoginFailure(PlayFabError error) {
            Debug.LogError("Login failed with error: \n" + error.GenerateErrorReport());
        }
    }
}