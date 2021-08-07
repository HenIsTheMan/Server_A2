using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using Server.General;
using SimpleJSON;
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

        internal FacebookSignIn() : base() {
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
                OnLoginWithFacebookSuccess,
                OnLoginWithFacebookFailure
            );

            facebookSignInEllipsesControl.enabled = true;
            facebookSignInMsg.text = signingInText;
            facebookSignInMsg.color = signingInColor;
        }

        private void OnLoginWithFacebookSuccess(LoginResult result) {
            Console.Log("Partial Facebook Sign In Success: " + result.SessionTicket);

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAcctInfoSuccess,
                OnGetAcctInfoFailure
            );

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetPlayerProfile",
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptSuccess,
                OnExecuteCloudScriptFailure
            );
        }

        private void OnLoginWithFacebookFailure(PlayFabError error) {
            Console.Log("Facebook Sign In Failure: " + error.GenerateErrorReport());

            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInFailureText;
            facebookSignInMsg.color = signInFailureColor;
        }

        private void OnGetAcctInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAcctInfoSuccess!");

            if(string.IsNullOrEmpty(result.AccountInfo.Username)) {
            }

            if(string.IsNullOrEmpty(result.AccountInfo.PrivateInfo.Email)) {
            }
        }

        private void OnGetAcctInfoFailure(PlayFabError _) {
            Console.LogError("GetAcctInfoFailure!");
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            JSONNode playerProfile = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult)); //I guess

            if(string.IsNullOrEmpty(playerProfile["displayName"].Value)) {
            }

            if(string.IsNullOrEmpty(playerProfile["contactEmailAddress"].Value)) {
            }
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }

        private void Func() {
            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInSuccessText;
            facebookSignInMsg.color = signInSuccessColor;

            myUnityEvent?.Invoke();
        }
    }
}