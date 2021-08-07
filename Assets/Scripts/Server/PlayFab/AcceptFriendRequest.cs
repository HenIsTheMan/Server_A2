using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class AcceptFriendRequest: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameText;

        private string myPlayFabID;
        private string otherPlayFabID;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal AcceptFriendRequest(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;
            otherPlayFabID = string.Empty;
        }

        static AcceptFriendRequest() {
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

            myPlayFabID = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest() {
                    TitleDisplayName = displayNameText.text
                },
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            otherPlayFabID = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
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
            _ = displayNames.Remove(displayNameText.text);

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "AcceptFriendRequest",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
                        OtherPlayFabID = otherPlayFabID,
                        Key = "FriendRequests",
                        Val = resultArr.ToString()
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptAcceptSuccess,
                OnExecuteCloudScriptAcceptFailure
            );
        }

        private void OnExecuteCloudScriptAcceptSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptAcceptSuccess!");
        }

        private void OnExecuteCloudScriptAcceptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptAcceptFailure!");
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