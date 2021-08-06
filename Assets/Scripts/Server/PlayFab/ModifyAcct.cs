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

        [SerializeField]
        private TMP_InputField usernameInputField;

        [SerializeField]
        private TMP_InputField emailInputField;

        [SerializeField]
        private TMP_InputField displayNameInputField;

        [SerializeField]
        private TMP_InputField contactEmailInputField;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ModifyAcct(): base() {
            usernameInputField = null;
            emailInputField = null;
            displayNameInputField = null;
            contactEmailInputField = null;
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
    }
}