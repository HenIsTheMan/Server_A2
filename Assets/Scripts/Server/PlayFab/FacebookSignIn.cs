using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class FacebookSignIn: MonoBehaviour {
        #region Fields

        public string tokenStr;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FacebookSignIn(): base() {
        }

        static FacebookSignIn() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.LoginWithFacebook(
                new LoginWithFacebookRequest() {
                    CreateAccount = true,
                    AccessToken = tokenStr
                },
                OnPlayfabFacebookAuthComplete,
                OnPlayfabFacebookAuthFailed
            );
        }

        private void OnPlayfabFacebookAuthComplete(LoginResult result) {
            Console.Log("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
        }

        private void OnPlayfabFacebookAuthFailed(PlayFabError error) {
            Console.Log("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport());
        }
    }
}