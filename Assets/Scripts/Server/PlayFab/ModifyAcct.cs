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

        [SerializeField]
        private TMP_InputField usernameInputField;

        [SerializeField]
        private TMP_InputField emailInputField;

        [SerializeField]
        private TMP_InputField displayNameInputField;

        [SerializeField]
        private TMP_InputField contactEmailInputField;

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

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ModifyAcct(): base() {
            doneCount = 0;

            usernameInputField = null;
            emailInputField = null;
            displayNameInputField = null;
            contactEmailInputField = null;

            editAcctMsgTmp = null;
            editAcctEllipsesControl = null;

            savingText = string.Empty;
            savingTextColor = Color.white;

            savedText = string.Empty;
            savedTextColor = Color.white;

            failedToSaveText = string.Empty;
            failedToSaveTextColor = Color.white;
        }

        static ModifyAcct() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
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

            displayNameInputField.text = playerProfile["displayName"].Value;
            contactEmailInputField.text = playerProfile["contactEmailAddress"].Value;
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }

        public void OnClick() {
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest {
                    DisplayName = displayNameInputField.text
                },
                OnUpdateUserTitleDisplayNameSuccess,
                OnUpdateUserTitleDisplayNameFailure
            );

            PlayFabClientAPI.AddOrUpdateContactEmail(
                new AddOrUpdateContactEmailRequest {
                    EmailAddress = contactEmailInputField.text
                },
                OnAddOrUpdateContactEmailSuccess,
                OnAddOrUpdateContactEmailFailure
            );

            editAcctEllipsesControl.enabled = true;
            editAcctMsgTmp.text = savingText;
            editAcctMsgTmp.color = savingTextColor;
        }

        private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult _) {
            Console.Log("UpdateUserTitleDisplayNameSuccess!");

            if(doneCount == 1) {
                editAcctEllipsesControl.enabled = false;
                editAcctMsgTmp.text = savedText;
                editAcctMsgTmp.color = savedTextColor;
            } else {
                ++doneCount;
            }
        }

        private void OnUpdateUserTitleDisplayNameFailure(PlayFabError _) {
            Console.LogError("UpdateUserTitleDisplayNameFailure!");

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = failedToSaveText;
            editAcctMsgTmp.color = failedToSaveTextColor;
        }

        private void OnAddOrUpdateContactEmailSuccess(AddOrUpdateContactEmailResult _) {
            Console.Log("AddOrUpdateContactEmailSuccess!");

            if(doneCount == 1) {
                editAcctEllipsesControl.enabled = false;
                editAcctMsgTmp.text = savedText;
                editAcctMsgTmp.color = savedTextColor;
            } else {
                ++doneCount;
            }
        }

        private void OnAddOrUpdateContactEmailFailure(PlayFabError _) {
            Console.LogError("AddOrUpdateContactEmailFailure!");

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = failedToSaveText;
            editAcctMsgTmp.color = failedToSaveTextColor;
        }
    }
}