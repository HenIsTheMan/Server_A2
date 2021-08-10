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

        private bool shldCreateAcct;

        private int doneCount;
        private int operationCount;

        [SerializeField]
        private int passwordLen;

        [SerializeField]
        private TMP_InputField usernameInputField;

        [SerializeField]
        private TMP_InputField emailInputField;

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
            shldCreateAcct = true;

            doneCount = 0;
            operationCount = 0;

            passwordLen = 0;

            usernameInputField = null;
            emailInputField = null;

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
            shldCreateAcct
                = !string.IsNullOrEmpty(usernameInputField.text)
                || !string.IsNullOrEmpty(emailInputField.text);

            operationCount = shldCreateAcct ? 4 : 3;

            PlayFabClientAPI.LoginWithFacebook(
                new LoginWithFacebookRequest() {
                    CreateAccount = shldCreateAcct,
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

            if(!shldCreateAcct) {
                MySuccessFunc();
                return;
            }

            doneCount = 0;

            usernameInputField.readOnly = true;
            emailInputField.readOnly = true;
            userAccessTokenInputField.readOnly = true;

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

            MyFailureFunc();
        }

        private void OnGetAcctInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAcctInfoSuccess!");

            string username = result.AccountInfo.Username;
            if(string.IsNullOrEmpty(username)) {
                username = usernameInputField.text;
            }

            string email = result.AccountInfo.PrivateInfo.Email;
            if(string.IsNullOrEmpty(email)) {
                email = emailInputField.text;
            }

            string password = string.Empty;
            for(int i = 0; i < passwordLen; ++i) {
                password += (char)Random.Range(33, 127);
            }

            if(shldCreateAcct) {
                PlayFabClientAPI.ExecuteCloudScript(
                    new ExecuteCloudScriptRequest() {
                        FunctionName = "UpdateUserReadOnlyData",
                        FunctionParameter = new {
                            PlayFabID = result.AccountInfo.PlayFabId,
                            Key = "FriendRequests",
                            Val = new JSONArray().ToString()
                        },
                        GeneratePlayStreamEvent = true,
                    },
                    OnExecuteCloudScriptUpdateSuccess,
                    OnExecuteCloudScriptUpdateFailure
                );
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

        private void OnExecuteCloudScriptUpdateSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptUpdateSuccess!");

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");
        }

        private void OnAddUsernamePasswordSuccess(AddUsernamePasswordResult _) {
            Console.Log("AddUsernamePasswordSuccess!");

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnAddUsernamePasswordFailure(PlayFabError error) {
            Console.LogError("AddUsernamePasswordFailure! " + error.GenerateErrorReport());

            MyFailureFunc();
        }

        private void OnGetAcctInfoFailure(PlayFabError _) {
            Console.LogError("GetAcctInfoFailure!");

            MyFailureFunc();
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            JSONNode playerProfile = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult)); //I guess

            string displayName = playerProfile["displayName"].Value;
            if(string.IsNullOrEmpty(displayName)) {
                displayName = usernameInputField.text;
            }

            string contactEmail = playerProfile["contactEmailAddress"].Value;
            if(string.IsNullOrEmpty(contactEmail)) {
                contactEmail = emailInputField.text;
            }

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest {
                    DisplayName = displayName
                },
                OnUpdateUserTitleDisplayNameSuccess,
                OnUpdateUserTitleDisplayNameFailure
            );

            _ = StartCoroutine(UpdateContactEmail(contactEmail));
        }

        private System.Collections.IEnumerator UpdateContactEmail(string contactEmail) {
            yield return new WaitForSeconds(0.1f); //Force a delay to prevent weird HTTP error

            PlayFabClientAPI.AddOrUpdateContactEmail(
                new AddOrUpdateContactEmailRequest {
                    EmailAddress = contactEmail
                },
                OnAddOrUpdateContactEmailSuccess,
                OnAddOrUpdateContactEmailFailure
            );
        }

        private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult _) {
            Console.Log("UpdateUserTitleDisplayNameSuccess!");

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnUpdateUserTitleDisplayNameFailure(PlayFabError error) {
            Console.LogError("UpdateUserTitleDisplayNameFailure!" + ' ' + error.ErrorMessage);

            MyFailureFunc();
        }

        private void OnAddOrUpdateContactEmailSuccess(AddOrUpdateContactEmailResult _) {
            Console.Log("AddOrUpdateContactEmailSuccess!");

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnAddOrUpdateContactEmailFailure(PlayFabError error) {
            Console.LogError("AddOrUpdateContactEmailFailure!" + ' ' + error.ErrorMessage);

            MyFailureFunc();
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");

            MyFailureFunc();
        }

        private void MySuccessFunc() {
             Console.Log("Full Facebook Sign In Success!");

            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInSuccessText;
            facebookSignInMsg.color = signInSuccessColor;

            myUnityEvent?.Invoke();
        }

        private void MyFailureFunc() {
            facebookSignInEllipsesControl.enabled = false;
            facebookSignInMsg.text = signInFailureText;
            facebookSignInMsg.color = signInFailureColor;

            usernameInputField.readOnly = false;
            emailInputField.readOnly = false;
            userAccessTokenInputField.readOnly = false;
        }
    }
}