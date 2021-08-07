using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class RemoveFriend: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameText;

        private string myPlayFabID;
        private string otherPlayFabID;

        internal ObjPool friendSelectionPool;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal RemoveFriend(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;
            otherPlayFabID = string.Empty;

            friendSelectionPool = null;
        }

        static RemoveFriend() {
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
                    FunctionName = "RemoveFriend",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
                        OtherPlayFabID = otherPlayFabID
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptSuccess,
                OnExecuteCloudScriptFailure
            );
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptSuccess!");

            friendSelectionPool.DeactivateObj(displayNameText.transform.parent.gameObject);
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");
        }
    }
}