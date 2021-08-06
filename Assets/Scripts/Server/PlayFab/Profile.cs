using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using Server.General;
using SimpleJSON;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Profile: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameTextTmp;

        [SerializeField]
        private TMP_Text contactEmailTextTmp;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Profile(): base() {
            displayNameTextTmp = null;
            contactEmailTextTmp = null;
        }

        static Profile() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            if(PlayFabClientAPI.IsClientLoggedIn()) {
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

        private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptSuccess!");

            JSONNode playerProfile = JSON.Parse(JsonWrapper.SerializeObject(result.FunctionResult)); //I guess

            displayNameTextTmp.text = playerProfile["displayName"].Value;
            contactEmailTextTmp.text = playerProfile["contactEmailAddress"].Value;
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }
    }
}