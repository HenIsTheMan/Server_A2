using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using Server.General;
using SimpleJSON;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ModifyAcct: MonoBehaviour {
        #region Fields

        private int doneCount;
        private int operationCount;

        private string displayNameSave;
        private string contactEmailSave;

        [SerializeField]
        private TMP_InputField usernameInputField;

        [SerializeField]
        private TMP_InputField emailInputField;

        [SerializeField]
        private TMP_InputField displayNameInputField;

        [SerializeField]
        private TMP_InputField contactEmailInputField;

        [SerializeField]
        private TMP_InputField passwordInputField;

        [SerializeField]
        private TMP_Text editAcctMsgTmp;

        [SerializeField]
        private EllipsesControl editAcctEllipsesControl;

        [SerializeField]
        private string savingText;

        [SerializeField]
        private Color savingTextColor;

        [SerializeField]
        private string savedText;

        [SerializeField]
        private Color savedTextColor;

        [SerializeField]
        private string failedToSaveText;

        [SerializeField]
        private Color failedToSaveTextColor;

        [SerializeField]
        private string nthToSaveText;

        [SerializeField]
        private Color nthToSaveTextColor;

        [SerializeField]
        private string wrongPasswordText;

        [SerializeField]
        private Color wrongPasswordTextColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ModifyAcct(): base() {
            doneCount = 0;
            operationCount = 0;

            displayNameSave = string.Empty;
            contactEmailSave = string.Empty;

            usernameInputField = null;
            emailInputField = null;
            displayNameInputField = null;
            contactEmailInputField = null;
            passwordInputField = null;

            editAcctMsgTmp = null;
            editAcctEllipsesControl = null;

            savingText = string.Empty;
            savingTextColor = Color.white;

            savedText = string.Empty;
            savedTextColor = Color.white;

            failedToSaveText = string.Empty;
            failedToSaveTextColor = Color.white;

            nthToSaveText = string.Empty;
            nthToSaveTextColor = Color.white;

            wrongPasswordText = string.Empty;
            wrongPasswordTextColor = Color.white;
        }

        static ModifyAcct() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            editAcctMsgTmp.text = string.Empty;

            if(PlayFabClientAPI.IsClientLoggedIn()) {
                PlayFabClientAPI.GetAccountInfo( 
                    new GetAccountInfoRequest (), 
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
        }

        #endregion

        private void OnGetAcctInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAcctInfoSuccess!");

            usernameInputField.text = result.AccountInfo.Username;
            emailInputField.text = result.AccountInfo.PrivateInfo.Email;
        }

        private void OnGetAcctInfoFailure(PlayFabError _) {
            Console.LogError("GetAcctInfoFailure!");
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            JSONNode playerProfile = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult)); //I guess

            displayNameSave = displayNameInputField.text = playerProfile["displayName"].Value;
            contactEmailSave = contactEmailInputField.text = playerProfile["contactEmailAddress"].Value;
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }

        public void OnClick() {
            PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest {
                    Email = emailInputField.text,
                    Password = passwordInputField.text
                },
                OnLoginSuccess,
                OnLoginFailure
            );

            editAcctEllipsesControl.enabled = true;
            editAcctMsgTmp.text = savingText;
            editAcctMsgTmp.color = savingTextColor;

            passwordInputField.text = string.Empty;
        }

        private void OnLoginSuccess(LoginResult _) {
            Console.Log("LoginSuccess!");

            doneCount = 0;
            operationCount = 0;

            if(displayNameInputField.text != displayNameSave) {
                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest {
                        DisplayName = displayNameInputField.text
                    },
                    OnUpdateUserTitleDisplayNameSuccess,
                    OnUpdateUserTitleDisplayNameFailure
                );

                displayNameInputField.readOnly = true;
                ++operationCount;
            }

            if(contactEmailInputField.text != contactEmailSave) {
                PlayFabClientAPI.AddOrUpdateContactEmail(
                    new AddOrUpdateContactEmailRequest {
                        EmailAddress = contactEmailInputField.text
                    },
                    OnAddOrUpdateContactEmailSuccess,
                    OnAddOrUpdateContactEmailFailure
                );

                contactEmailInputField.readOnly = true;
                ++operationCount;
            }

            if(operationCount == 0) {
                editAcctEllipsesControl.enabled = false;
                editAcctMsgTmp.text = nthToSaveText;
                editAcctMsgTmp.color = nthToSaveTextColor;
            }
        }

        private void OnLoginFailure(PlayFabError error) {
            Console.Log("LoginFailure!");

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = wrongPasswordText;
            editAcctMsgTmp.color = wrongPasswordTextColor;
        }

        private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult result) {
            Console.Log("UpdateUserTitleDisplayNameSuccess!");

            displayNameSave = result.DisplayName;
            displayNameInputField.readOnly = false;

            if(doneCount == operationCount - 1) {
                editAcctEllipsesControl.enabled = false;
                editAcctMsgTmp.text = savedText;
                editAcctMsgTmp.color = savedTextColor;
            } else {
                ++doneCount;
            }
        }

        private void OnUpdateUserTitleDisplayNameFailure(PlayFabError _) {
            Console.LogError("UpdateUserTitleDisplayNameFailure!");

            displayNameInputField.readOnly = false;

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = failedToSaveText;
            editAcctMsgTmp.color = failedToSaveTextColor;
        }

        private void OnAddOrUpdateContactEmailSuccess(AddOrUpdateContactEmailResult _) {
            Console.Log("AddOrUpdateContactEmailSuccess!");

            contactEmailSave = contactEmailInputField.text; //Only way as far as I can tell
            contactEmailInputField.readOnly = false;

            if(doneCount == operationCount - 1) {
                editAcctEllipsesControl.enabled = false;
                editAcctMsgTmp.text = savedText;
                editAcctMsgTmp.color = savedTextColor;
            } else {
                ++doneCount;
            }
        }

        private void OnAddOrUpdateContactEmailFailure(PlayFabError error) {
            Console.Log("AddOrUpdateContactEmailFailure!" + ' ' + error.ErrorMessage);

            contactEmailInputField.readOnly = false;

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = failedToSaveText;
            editAcctMsgTmp.color = failedToSaveTextColor;
        }
    }
}