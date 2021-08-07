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

        private int doneCount;

        private string email;
        private string username;

        [SerializeField]
        private int passwordLen;

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
            doneCount = 0;

            email = string.Empty;
            username = string.Empty;

            passwordLen = 0;

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
                    AccessToken = userAccessTokenInputField.text,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                        GetPlayerProfile = true,
                        ProfileConstraints = new PlayerProfileViewConstraints {
                            ShowLinkedAccounts = true
                        }
                    }
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

            doneCount = 0;

            foreach(var acct in result.InfoResultPayload.PlayerProfile.LinkedAccounts) {
                if(acct.Platform == LoginIdentityProvider.Facebook) {
                    email = acct.Email;
                    username = acct.Username;
                    break;
                }
            }

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

            if(!string.IsNullOrEmpty(result.AccountInfo.Username)) {
                username = result.AccountInfo.Username;
            }
            if(!string.IsNullOrEmpty(result.AccountInfo.PrivateInfo.Email)) {
                email = result.AccountInfo.PrivateInfo.Email;
            }

            if(username == null) {
                username = string.Empty;
            }
            if(email == null) {
                email = string.Empty;
            }

            string password = string.Empty;
            for(int i = 0; i < passwordLen; ++i) {
                password += (char)Random.Range(33, 127);
            }

            PlayFabClientAPI.AddUsernamePassword(
                new AddUsernamePasswordRequest() {
                    Email = email,
                    Username = username,
                    Password = password,
                },
                OnAddUsernamePasswordSuccess,
                OnAddUsernamePasswordFailure
            );
        }

        private void OnAddUsernamePasswordSuccess(AddUsernamePasswordResult _) {
            Console.Log("AddUsernamePasswordSuccess!");

            if(doneCount == 2) {
                MyFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnAddUsernamePasswordFailure(PlayFabError _) {
            Console.LogError("AddUsernamePasswordFailure!");
        }

        private void OnGetAcctInfoFailure(PlayFabError _) {
            Console.LogError("GetAcctInfoFailure!");
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            JSONNode playerProfile = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult)); //I guess

            if(string.IsNullOrEmpty(playerProfile["displayName"].Value)) {
                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest {
                        DisplayName = "Test"
                    },
                    OnUpdateUserTitleDisplayNameSuccess,
                    OnUpdateUserTitleDisplayNameFailure
                );
            }

            if(string.IsNullOrEmpty(playerProfile["contactEmailAddress"].Value)) {
                PlayFabClientAPI.AddOrUpdateContactEmail(
                    new AddOrUpdateContactEmailRequest {
                        EmailAddress = "Test"
                    },
                    OnAddOrUpdateContactEmailSuccess,
                    OnAddOrUpdateContactEmailFailure
                );
            }
        }

        private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult _) {
            Console.Log("UpdateUserTitleDisplayNameSuccess!");

            if(doneCount == 2) {
                MyFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnUpdateUserTitleDisplayNameFailure(PlayFabError error) {
            Console.LogError("UpdateUserTitleDisplayNameFailure!" + ' ' + error.ErrorMessage);
        }

        private void OnAddOrUpdateContactEmailSuccess(AddOrUpdateContactEmailResult _) {
            Console.Log("AddOrUpdateContactEmailSuccess!");

            if(doneCount == 2) {
                MyFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnAddOrUpdateContactEmailFailure(PlayFabError error) {
            Console.LogError("AddOrUpdateContactEmailFailure!" + ' ' + error.ErrorMessage);
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }

        private void MyFunc() {
            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInSuccessText;
            facebookSignInMsg.color = signInSuccessColor;

            myUnityEvent?.Invoke();
        }
    }
}