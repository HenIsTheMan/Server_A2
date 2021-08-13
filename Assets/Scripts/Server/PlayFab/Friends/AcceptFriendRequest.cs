using Server.General;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class AcceptFriendRequest: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameText;

        private string myPlayFabID;
        private string otherPlayFabID;

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

        internal AcceptFriendRequest(): base() {
            displayNameText = null;

            myPlayFabID = string.Empty;
            otherPlayFabID = string.Empty;

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
                    FunctionName = "AcceptFriendRequest",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
                        OtherPlayFabID = otherPlayFabID,
                        Keys = new string[1] {
                            "FriendRequests"
                        },
                        Vals = new string[1] {
                            resultArr.ToString()
                        }
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptAcceptSuccess,
                OnExecuteCloudScriptAcceptFailure
            );
        }

        private void OnExecuteCloudScriptAcceptSuccess(ExecuteCloudScriptResult _) {
            Console.Log("ExecuteCloudScriptAcceptSuccess!");

            friendRequestSelectionPool.DeactivateObj(displayNameText.transform.parent.gameObject);

            MySuccessFunc();
        }

        private void OnExecuteCloudScriptAcceptFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptAcceptFailure!");

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