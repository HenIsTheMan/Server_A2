using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Server.PlayFab {
    internal sealed class DeleteAcct: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text editAcctMsgTmp;

        [SerializeField]
        private string acctDeletedText;

        [SerializeField]
        private Color acctDeletedTextColor;

        [SerializeField]
        private UnityEvent myUnityEvent;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal DeleteAcct(): base() {
            editAcctMsgTmp = null;
            acctDeletedText = string.Empty;
            acctDeletedTextColor = Color.white;

            myUnityEvent = null;
        }

        static DeleteAcct() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "DeleteAcct",
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptSuccess,
                OnExecuteCloudScriptFailure
            );
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            editAcctMsgTmp.text = acctDeletedText;
            editAcctMsgTmp.color = acctDeletedTextColor;

            myUnityEvent?.Invoke();
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }
    }
}