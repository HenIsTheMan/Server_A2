using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ClearInvButton: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ClearInvButton(): base() {
        }

        static ClearInvButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs
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
        }

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptSuccess!");
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }
    }
}