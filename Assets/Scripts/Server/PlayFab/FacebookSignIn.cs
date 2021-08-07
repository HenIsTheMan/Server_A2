using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Server.PlayFab {
    internal sealed class FacebookSignIn: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_InputField userAccessTokenInputField;

        [SerializeField]
        private TMP_Text facebookSignInMsg;

        [SerializeField]
        private EllipsesControl facebookSignInEllipsesControl;

        [SerializeField]
        private string signingInText;

        [SerializeField]
        private Color signingInColor;

        [SerializeField]
        private string signInSuccessText;

        [SerializeField]
        private Color signInSuccessColor;

        [SerializeField]
        private string signInFailureText;

        [SerializeField]
        private Color signInFailureColor;

        [SerializeField]
        private UnityEvent myUnityEvent;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FacebookSignIn(): base() {
            userAccessTokenInputField = null;

            facebookSignInMsg = null;
            facebookSignInEllipsesControl = null;

            signingInText = string.Empty;
            signingInColor = Color.white;

            signInSuccessText = string.Empty;
            signInSuccessColor = Color.white;

            signInFailureText = string.Empty;
            signInFailureColor = Color.white;

            myUnityEvent = null;
        }

        static FacebookSignIn() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            facebookSignInMsg.text = string.Empty;
        }

        #endregion

        public void OnClick() {
            PlayFabClientAPI.LoginWithFacebook(
                new LoginWithFacebookRequest() {
                    CreateAccount = true,
                    AccessToken = userAccessTokenInputField.text
                },
                OnPlayfabFacebookAuthComplete,
                OnPlayfabFacebookAuthFailed
            );

            facebookSignInEllipsesControl.enabled = true;
            facebookSignInMsg.text = signingInText;
            facebookSignInMsg.color = signingInColor;
        }

        private void OnPlayfabFacebookAuthComplete(LoginResult result) {
            Console.Log("Facebook Sign In Success: " + result.SessionTicket);

            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInSuccessText;
            facebookSignInMsg.color = signInSuccessColor;

            myUnityEvent?.Invoke();
        }

        private void OnPlayfabFacebookAuthFailed(PlayFabError error) {
            Console.Log("Facebook Sign In Failure: " + error.GenerateErrorReport());

            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInFailureText;
            facebookSignInMsg.color = signInFailureColor;
        }
    }
}