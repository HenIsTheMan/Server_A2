using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Guest: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Guest(): base() {
        }

        static Guest() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest {
                    CustomId = "GettingStartedGuide",
                    CreateAccount = true
                },
                OnLoginSuccess,
                OnLoginFailure
            );
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("Guest Login Successful!");
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("Guest Login Failed (" + error.GenerateErrorReport() + ")!");
        }
    }
}