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

        internal static string displayNameCache;

        internal static string contactEmailCache;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Profile(): base() {
            displayNameTextTmp = null;
            contactEmailTextTmp = null;
        }

        static Profile() {
            displayNameCache = string.Empty;
            contactEmailCache = string.Empty;
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

            displayNameCache = displayNameTextTmp.text = playerProfile["displayName"].Value;
            contactEmailCache = contactEmailTextTmp.text = playerProfile["contactEmailAddress"].Value;
        }

        private void OnExecuteCloudScriptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptFailure!");
        }
    }
}