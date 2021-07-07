using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Server {
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

        private void Start() {
            if(string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) {
                /*
                Please change the titleId below to your own titleId from PlayFab Game Manager.
                If you have already set the value in the Editor Extensions, this can be skipped.
                */
                PlayFabSettings.staticSettings.TitleId = "42";

                Debug.Log("here", gameObject);
            }
            var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }

        #endregion

        private void OnLoginSuccess(LoginResult result) {
            Debug.Log("Congratulations, you made your first successful API call!");
        }

        private void OnLoginFailure(PlayFabError error) {
            Debug.LogWarning("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}