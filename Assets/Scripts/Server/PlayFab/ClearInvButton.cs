using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ClearInvButton: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text invPanelMsg;

        [SerializeField]
        private EllipsesControl invPanelEllipsesControl;

        [SerializeField]
        private string processingText;

        [SerializeField]
        private Color processingColor;

        [SerializeField]
        private string successText;

        [SerializeField]
        private Color successColor;

        [SerializeField]
        private string failureText;

        [SerializeField]
        private Color failureColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ClearInvButton(): base() {
            invPanelMsg = null;
            invPanelEllipsesControl = null;

            processingText = string.Empty;
            processingColor = Color.white;

            successText = string.Empty;
            successColor = Color.white;

            failureText = string.Empty;
            failureColor = Color.white;
        }

        static ClearInvButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnEnable() { //I guess
            invPanelMsg.text = string.Empty;
            invPanelEllipsesControl.enabled = false;
        }

        #endregion

        public void OnClick() {
            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "ClearInv",
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptSuccess,
                OnExecuteCloudScriptFailure
            );

            MyProcessingFunc();
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptSuccess!");

            GiftTrade.globalObj.UpdateInv();

            StartCoroutine(nameof(SuccessIsComing));
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");

            MyFailureFunc();
        }

        private System.Collections.IEnumerator SuccessIsComing() {
            while(GiftTrade.globalObj.IsInvUpdating) {
                yield return null;
            }

            MySuccessFunc();
        }

        private void MyProcessingFunc() {
            invPanelEllipsesControl.enabled = true;
            invPanelMsg.text = processingText;
            invPanelMsg.color = processingColor;
        }

        private void MySuccessFunc() {
            invPanelEllipsesControl.enabled = false;
            invPanelMsg.text = successText;
            invPanelMsg.color = successColor;
        }

        private void MyFailureFunc() {
            invPanelEllipsesControl.enabled = false;
            invPanelMsg.text = failureText;
            invPanelMsg.color = failureColor;
        }
    }
}