using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Server.PlayFab.AddFriendTypes;

namespace Server.PlayFab {
    internal sealed class AddFriend: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        private string displayNameOfRequester;
        private string playFabIdOfRequestee;

        [SerializeField]
        private TMP_Text friendsMsg;

        [SerializeField]
        private EllipsesControl friendsEllipsesControl;

        [SerializeField]
        private string sendingText;

        [SerializeField]
        private Color sendingColor;

        [SerializeField]
        private string sentText;

        [SerializeField]
        private Color sentColor;

        [SerializeField]
        private string failedToSendText;

        [SerializeField]
        private Color failedToSendColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal AddFriend(): base() {
            dropdown = null;
            inputField = null;

            displayNameOfRequester = string.Empty;
            playFabIdOfRequestee = string.Empty;

            friendsMsg = null;
            friendsEllipsesControl = null;

            sendingText = string.Empty;
            sendingColor = Color.white;

            sentText = string.Empty;
            sentColor = Color.white;

            failedToSendText = string.Empty;
            failedToSendColor = Color.white;
        }

        static AddFriend() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            friendsMsg.text = string.Empty;
        }

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

            displayNameOfRequester = result.AccountInfo.TitleInfo.DisplayName;

            GetAccountInfoRequest request = new GetAccountInfoRequest();
            bool hasFailed = false;

            switch((AddFriendType)dropdown.value) {
                case AddFriendType.DisplayName:
                    request.TitleDisplayName = inputField.text;

                    if(request.TitleDisplayName == displayNameOfRequester) {
                        hasFailed = true;
                    }

                    break;
                case AddFriendType.Username:
                    request.Username = inputField.text;

                    if(request.Username == result.AccountInfo.Username) {
                        hasFailed = true;
                    }

                    break;
                case AddFriendType.Email:
                    request.Email = inputField.text;

                    if(request.Email == result.AccountInfo.PrivateInfo.Email) {
                        hasFailed = true;
                    }

                    break;
                case AddFriendType.PlayFabID:
                    request.PlayFabId = inputField.text;

                    if(request.PlayFabId == result.AccountInfo.PlayFabId) {
                        hasFailed = true;
                    }

                    break;
            }

            inputField.text = string.Empty;

            if(hasFailed) { //Cannot add yourself
                MyFailureFunc();
                return;
            }

            PlayFabClientAPI.GetAccountInfo(
                request,
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            playFabIdOfRequestee = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.GetFriendsList(
                new GetFriendsListRequest {
                    IncludeSteamFriends = false,
                    IncludeFacebookFriends = true,
                    XboxToken = null
                },
                OnGetFriendsListSuccess,
                OnGetFriendsListFailure
            );
        }

        private void OnGetFriendsListSuccess(GetFriendsListResult result) {
            Console.Log("GetFriendsListSuccess!");

            foreach(FriendInfo friendInfo in result.Friends) { //Prevents sending friend requests to friends
                if(playFabIdOfRequestee == friendInfo.FriendPlayFabId) {
                    MyFailureFunc();
                    return;
                }
            }

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
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
			List<string> displayNames = new List<string>(); //For checking

			while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                displayNames.Add(myEnumerator.Current.Value);
            }

            if(!displayNames.Contains(displayNameOfRequester)) { //Prevents multi-requesting
                resultArr.Add(displayNameOfRequester);
            } else {
                MyFailureFunc();
                return;
            }

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
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

        private void OnGetFriendsListFailure(PlayFabError _) {
            Console.Log("GetFriendsListFailure!");

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
            friendsMsg.text = sendingText;
            friendsMsg.color = sendingColor;
        }

        private void MySuccessFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = sentText;
            friendsMsg.color = sentColor;
        }

        private void MyFailureFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = failedToSendText;
            friendsMsg.color = failedToSendColor;
        }
    }
}