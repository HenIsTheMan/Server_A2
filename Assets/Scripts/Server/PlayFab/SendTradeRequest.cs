using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
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

        private JSONArray resultArr;

        private int doneCount;
        private int operationCount;

        private int togglesLen;
        private bool[] flags;

        private int offerCountTextsLen;
        private int[] offerCounts;

        private string displayNameOfRequester;
        private string playFabIdOfRequestee;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private string[] itemIDs;

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

        internal SendTradeRequest(): base() {
            resultArr = null;

            doneCount = 0;
            operationCount = 0;

            displayNameOfRequester = string.Empty;
            playFabIdOfRequestee = string.Empty;

            itemIDs = System.Array.Empty<string>();
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

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(itemIDs.Length == toggles.Length);
            UnityEngine.Assertions.Assert.IsTrue(toggles.Length == offerCountTexts.Length);
        }

        private void Awake() {
            sendTradeRequestMsg.text = string.Empty;

            togglesLen = toggles.Length; //Lame
            flags = new bool[togglesLen];

            offerCountTextsLen = offerCountTexts.Length; //Lame
            offerCounts = new int[offerCountTextsLen];
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

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
                        Key = "TradeRequests"
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        private void OnExecuteCloudScriptGetSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptGetSuccess!");

            resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            JSONNode.Enumerator myEnumerator = resultArr.GetEnumerator();

			List<string> serializedTradeRequestData = new List<string>();
            List<JSONArray> deserializedTradeRequestData = new List<JSONArray>();

            while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                serializedTradeRequestData.Add(myEnumerator.Current.Value);
            }
            serializedTradeRequestData.ForEach(dataPt => deserializedTradeRequestData.Add((JSONArray)JSON.Parse(dataPt)));

            foreach(JSONArray node in deserializedTradeRequestData) {
                if((string)node[0] == displayNameOfRequester) { //Prevents multi-requesting
                    MyFailureFunc();
                    return;
                }
            }

            for(int i = 0; i < togglesLen; ++i) {
                flags[i] = toggles[i].isOn;
            }
            for(int i = 0; i < offerCountTextsLen; ++i) {
               offerCounts[i] = System.Convert.ToInt32(offerCountTexts[i].text);
            }

            JSONArray newNode = new JSONArray();
            newNode.Add(displayNameOfRequester);
            newNode.Add(JsonWrapper.SerializeObject(flags));
            newNode.Add(JsonWrapper.SerializeObject(offerCounts));

            resultArr.Add(newNode);

            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                OnGetUserInventorySuccess,
                OnGetUserInventoryFailure
            );
        }

        private void OnGetUserInventorySuccess(GetUserInventoryResult result) {
            Console.Log("GetUserInventorySuccess!");

            doneCount = 0;
            operationCount = 2; //Lame

            int len = (int)ItemType.Amt;
            List<ItemInstance>[] items = new List<ItemInstance>[len];

            for(int i = 0; i < len; ++i) {
                items[i] = new List<ItemInstance>();
            }

            foreach(ItemInstance instance in result.Inventory) {
				for(int i = 0; i < len; ++i) {
					if(instance.ItemId == itemIDs[i]) {
						items[i].Add(instance);
						break;
					}
				}
			}

            for(int i = 0; i < len; ++i) { //Prevents offering more than possessed
                if(items[i].Count < offerCounts[i]) {
                    MyFailureFunc();
                    return;
                }
            }

            List<string> offeredInventoryInstanceIDs = new List<string>();
            for(int i = 0; i < len; ++i) {
                for(int j = 0; j < offerCounts[i]; ++j) {
                    offeredInventoryInstanceIDs.Add(items[i][j].ItemInstanceId);
                }
            }

            List<string> requestedCatalogItemIDs = new List<string>();
            for(int i = 0; i < togglesLen; ++i) {
                if(flags[i]) {
                    requestedCatalogItemIDs.Add(itemIDs[i]);
                }
            }

            PlayFabClientAPI.OpenTrade(
				new OpenTradeRequest() {
					AllowedPlayerIds = new List<string> {
						playFabIdOfRequestee
					},
					OfferedInventoryInstanceIds = offeredInventoryInstanceIDs,
					RequestedCatalogItemIds = requestedCatalogItemIDs
                },
				OnOpenTradeSuccess,
				OnOpenTradeFailure
			);

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playFabIdOfRequestee,
                        Keys = new string[1] {
                            "TradeRequests"
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

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnOpenTradeSuccess(OpenTradeResponse _) {
            Console.Log("OpenTradeSuccess!");

            if(doneCount == operationCount - 1) {
                MySuccessFunc();
            } else {
                ++doneCount;
            }
        }

        private void OnOpenTradeFailure(PlayFabError _) {
            Console.LogError("OpenTradeFailure!");

            MyFailureFunc();
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");

            MyFailureFunc();
        }

        private void OnGetUserInventoryFailure(PlayFabError _) {
            Console.LogError("GetUserInventoryFailure!");

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