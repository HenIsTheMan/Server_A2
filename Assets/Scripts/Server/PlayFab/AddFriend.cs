using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using Server.General;
using SimpleJSON;
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

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal AddFriend(): base() {
        }

        static AddFriend() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
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

            PlayFabClientAPI.GetAccountInfo(
                request,
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

		private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetFriendRequests",
                    FunctionParameter = new {
                        PlayFabID = result.AccountInfo.PlayFabId
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        private void OnExecuteCloudScriptGetSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptGetSuccess!");


            Console.Log(result.FunctionResult);
            return;

            JSONNode resultNode = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult));

            if(string.IsNullOrEmpty(resultNode["friendRequests"].Value)) {
                JSONNode friendRequestsNode = new JSONArray();
                friendRequestsNode.Add(resultNode["requester"].Value);

                PlayFabClientAPI.ExecuteCloudScript(
                    new ExecuteCloudScriptRequest() {
                        FunctionName = "SetFriendRequests",
                        FunctionParameter = new {
                            PlayFabID = resultNode["requestee"].Value,
                            FriendRequests = friendRequestsNode.ToString()
                        },
                        GeneratePlayStreamEvent = true,
                    },
                    OnExecuteCloudScriptSetSuccess,
                    OnExecuteCloudScriptSetFailure
                );
            } else {
                JSONArray arr = (JSONArray)resultNode;
                Console.Log(arr.Count);
            }
        }

        private void OnExecuteCloudScriptSetSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptSetSuccess!");
        }

        private void OnExecuteCloudScriptSetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptSetFailure!");
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");
        }
    }
}