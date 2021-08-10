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

        internal TMP_Text friendsMsg;
        internal EllipsesControl friendsEllipsesControl;

        internal string processingText;
        internal Color processingColor;

        internal string successText;
        internal Color successColor;

        internal string failureText;
        internal Color failureColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal RemoveFriend(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;
            otherPlayFabID = string.Empty;

            friendSelectionPool = null;

            friendsMsg = null;
            friendsEllipsesControl = null;

            processingText = string.Empty;
            processingColor = Color.white;

            successText = string.Empty;
            successColor = Color.white;

            failureText = string.Empty;
            failureColor = Color.white;
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

            MyProcessingFunc();
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

            MySuccessFunc();
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");

            MyFailureFunc();
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");

            MyFailureFunc();
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");

            MyFailureFunc();
        }

        private void MyProcessingFunc() {
            friendsEllipsesControl.enabled = true;
            friendsMsg.text = processingText;
            friendsMsg.color = processingColor;
        }

        private void MySuccessFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = successText;
            friendsMsg.color = successColor;
        }

        private void MyFailureFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = failureText;
            friendsMsg.color = failureColor;
        }
    }
}