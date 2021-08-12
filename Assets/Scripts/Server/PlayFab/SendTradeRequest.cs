using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Server.PlayFab.InputTypes;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class SendTradeRequest: MonoBehaviour {
        #region Fields

        private string displayNameOfRequester;
        private string playFabIdOfRequestee;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private Toggle[] toggles;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private TMP_Text[] offerCountTexts;

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TMP_Text sendTradeRequestMsg;

        [SerializeField]
        private EllipsesControl sendTradeRequestEllipsesControl;

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

        internal SendTradeRequest() : base() {
            displayNameOfRequester = string.Empty;
            playFabIdOfRequestee = string.Empty;

            toggles = System.Array.Empty<Toggle>();
            offerCountTexts = System.Array.Empty<TMP_Text>();

            dropdown = null;
            inputField = null;

            sendTradeRequestMsg = null;
            sendTradeRequestEllipsesControl = null;

            sendingText = string.Empty;
            sendingColor = Color.white;

            sentText = string.Empty;
            sentColor = Color.white;

            failedToSendText = string.Empty;
            failedToSendColor = Color.white;
        }

        static SendTradeRequest() {
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

            displayNameOfRequester = result.AccountInfo.TitleInfo.DisplayName;

            GetAccountInfoRequest request = new GetAccountInfoRequest();
            bool hasFailed = false;

            switch((InputType)dropdown.value) {
                case InputType.DisplayName:
                    request.TitleDisplayName = inputField.text;

                    if(request.TitleDisplayName == displayNameOfRequester) {
                        hasFailed = true;
                    }

                    break;
                case InputType.Username:
                    request.Username = inputField.text;

                    if(request.Username == result.AccountInfo.Username) {
                        hasFailed = true;
                    }

                    break;
                case InputType.Email:
                    request.Email = inputField.text;

                    if(request.Email == result.AccountInfo.PrivateInfo.Email) {
                        hasFailed = true;
                    }

                    break;
                case InputType.PlayFabID:
                    request.PlayFabId = inputField.text;

                    if(request.PlayFabId == result.AccountInfo.PlayFabId) {
                        hasFailed = true;
                    }

                    break;
            }

            inputField.text = string.Empty;

            if(hasFailed) { //Cannot trade with yourself
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

            //open trade??

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
            ); //??
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
            sendTradeRequestEllipsesControl.enabled = true;
            sendTradeRequestMsg.text = sendingText;
            sendTradeRequestMsg.color = sendingColor;
        }

        private void MySuccessFunc() {
            sendTradeRequestEllipsesControl.enabled = false;
            sendTradeRequestMsg.text = sentText;
            sendTradeRequestMsg.color = sentColor;
        }

        private void MyFailureFunc() {
            sendTradeRequestEllipsesControl.enabled = false;
            sendTradeRequestMsg.text = failedToSendText;
            sendTradeRequestMsg.color = failedToSendColor;
        }
    }
}