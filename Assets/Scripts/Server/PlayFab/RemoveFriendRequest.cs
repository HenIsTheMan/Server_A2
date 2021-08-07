using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class RemoveFriendRequest: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameText;

        private string myPlayFabID;

        internal ObjPool friendRequestSelectionPool;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal RemoveFriendRequest(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;

            friendRequestSelectionPool = null;
        }

        static RemoveFriendRequest() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            myPlayFabID = result.AccountInfo.PlayFabId;

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

            _ = resultArr.Remove(displayNameText.text);

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
                        Key = "FriendRequests",
                        Val = resultArr.ToString()
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptSetSuccess,
                OnExecuteCloudScriptSetFailure
            );
        }

        private void OnExecuteCloudScriptSetSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptSetSuccess!");

            friendRequestSelectionPool.DeactivateObj(displayNameText.transform.parent.gameObject);
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