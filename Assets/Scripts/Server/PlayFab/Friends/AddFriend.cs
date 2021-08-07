using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Server.PlayFab.AddFriendTypes;

namespace Server.PlayFab {
    internal sealed class AddFriend: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        private string displayNameOfRequester;

        private string playFabIdOfRequestee;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal AddFriend(): base() {
            dropdown = null;
            inputField = null;
            displayNameOfRequester = string.Empty;
            playFabIdOfRequestee = string.Empty;
        }

        static AddFriend() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfo1stSuccess,
                OnGetAccountInfo1stFailure
            );
        }

        private void OnGetAccountInfo1stSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfo1stSuccess!");

            displayNameOfRequester = result.AccountInfo.TitleInfo.DisplayName;

            GetAccountInfoRequest request = new GetAccountInfoRequest();

            switch((AddFriendType)dropdown.value) {
                case AddFriendType.DisplayName:
                    request.TitleDisplayName = inputField.text;
                    break;
                case AddFriendType.Username:
                    request.Username = inputField.text;
                    break;
                case AddFriendType.Email:
                    request.Email = inputField.text;
                    break;
                case AddFriendType.PlayFabID:
                    request.PlayFabId = inputField.text;
                    break;
            }

            inputField.text = string.Empty;

            PlayFabClientAPI.GetAccountInfo(
                request,
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            playFabIdOfRequestee = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
                        Key = "FriendRequests"
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        private void OnExecuteCloudScriptGetSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptGetSuccess!");

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            JSONNode.Enumerator myEnumerator = resultArr.GetEnumerator();
			List<string> displayNames = new List<string>(); //For checking

			while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                displayNames.Add(myEnumerator.Current.Value);
            }

            if(!displayNames.Contains(displayNameOfRequester)) { //Prevents multi-requesting
                resultArr.Add(displayNameOfRequester);
            }

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
                        Key = "FriendRequests",
                        Val = resultArr.ToString()
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptUpdateSuccess,
                OnExecuteCloudScriptUpdateFailure
            );
        }

        private void OnExecuteCloudScriptUpdateSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptUpdateSuccess!");
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");
        }
    }
}