using Server.General;
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

        internal RemoveFriendRequest(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;

            friendRequestSelectionPool = null;

            friendsMsg = null;
            friendsEllipsesControl = null;

            processingText = string.Empty;
            processingColor = Color.white;

            successText = string.Empty;
            successColor = Color.white;

            failureText = string.Empty;
            failureColor = Color.white;
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

            MyProcessingFunc();
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

            int index = 0;
            string displayName = displayNameText.text;
            while(myEnumerator.MoveNext()) { //Iterate through JSONArray 
                if(displayName == myEnumerator.Current.Value) {
                    _ = resultArr.Remove(index);
                    break;
                }
                ++index;
            }

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
                        Keys = new string[1] {
                            "FriendRequests"
                        },
                        Vals = new string[1] {
                            resultArr.ToString()
                        }
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptUpdateSuccess,
                OnExecuteCloudScriptUpdateFailure
            );
        }

        private void OnExecuteCloudScriptUpdateSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptUpdateSuccess!");

            friendRequestSelectionPool.DeactivateObj(displayNameText.transform.parent.gameObject);

            MySuccessFunc();
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");

            MyFailureFunc();
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");

            MyFailureFunc();
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");

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