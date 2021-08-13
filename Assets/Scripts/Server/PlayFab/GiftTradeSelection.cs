using Server.General;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Server.PlayFab.ItemTypes;
using System.Collections.Generic;
using System.Linq;

namespace Server.PlayFab {
    internal sealed class GiftTradeSelection: MonoBehaviour {
        #region Fields

        private bool isCancelTrade;

        internal ObjPool selectionPool;

        private string myDisplayName;
        internal string otherDisplayName;
        internal string playerID;
        internal string tradeID;

        [SerializeField]
        internal TMP_Text displayNameText;

        [EnumIndices(typeof(ItemType)), SerializeField]
        internal TMP_Text[] itemCountTexts;

        [EnumIndices(typeof(ItemType)), SerializeField]
        internal Image[] itemImgs;

        [EnumIndices(typeof(ItemType)), SerializeField]
        internal string[] itemIDs;

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

        internal GiftTradeSelection(): base() {
            isCancelTrade = false;

            selectionPool = null;

            myDisplayName = string.Empty;
            otherDisplayName = string.Empty;
            playerID = string.Empty;
            tradeID = string.Empty;

            displayNameText = null;

            itemCountTexts = System.Array.Empty<TMP_Text>();
            itemImgs = System.Array.Empty<Image>();
            itemIDs = System.Array.Empty<string>();
        }

        static GiftTradeSelection() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(itemCountTexts.Length == itemImgs.Length);
            UnityEngine.Assertions.Assert.IsTrue(itemImgs.Length == itemIDs.Length);
        }

        #endregion

        public void AcceptTrade() {
            isCancelTrade = false;

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playerID,
                        Key = "TradeRequests"
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        public void CancelTrade() {
            isCancelTrade = true;

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfo1stSuccess,
                OnGetAccountInfo1stFailure
            );
		}

        private void OnGetAccountInfo1stSuccess(GetAccountInfoResult result) {
			Console.Log("GetAccountInfo1stSuccess!");

            myDisplayName = result.AccountInfo.TitleInfo.DisplayName;

			PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playerID,
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

            string displayName = isCancelTrade ? myDisplayName : otherDisplayName;

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            foreach(JSONArray node in resultArr) { //Oh, can just do this way
                if((string)node[0] == displayName) {
                    resultArr.Remove((JSONNode)node);
                    break;
                }
            }

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "UpdateUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = playerID,
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

            if(isCancelTrade) {
                PlayFabClientAPI.CancelTrade(
                    new CancelTradeRequest() {
                        TradeId = tradeID
                    },
                    OnCancelTradeSuccess,
                    OnCancelTradeFailure
                );
            } else {
                PlayFabClientAPI.GetUserInventory(
                    new GetUserInventoryRequest(),
                    OnGetUserInventorySuccess,
                    OnGetUserInventoryFailure
                );
            }
        }

        private void OnGetUserInventorySuccess(GetUserInventoryResult result) {
            Console.Log("GetUserInventorySuccess!");

            int commonLen = itemImgs.Length;
            List<string> itemIDsToFulfill = new List<string>(commonLen);

            for(int i = 0; i < commonLen; ++i) {
                if(itemImgs[i].enabled) {
                    itemIDsToFulfill.Add(itemIDs[i]);
                }
            }

            List<string> acceptedInvInstanceIDs = new List<string>();
            int count = itemIDsToFulfill.Count;

            foreach(ItemInstance instance in result.Inventory) {
                if(!itemIDsToFulfill.Any()) {
                    break;
                }

                for(int i = 0; i < count; ++i) {
                    if(instance.ItemId == itemIDsToFulfill[i]) {
                        acceptedInvInstanceIDs.Add(instance.ItemInstanceId);

                        itemIDsToFulfill.Remove(instance.ItemId);

                        break;
                    }
                }
            }

            PlayFabClientAPI.AcceptTrade(
                new AcceptTradeRequest() {
                    OfferingPlayerId = playerID,
                    TradeId = tradeID,
                    AcceptedInventoryInstanceIds = acceptedInvInstanceIDs
                },
                OnAcceptTradeSuccess,
                OnAcceptTradeFailure
            );
        }

        private void OnAcceptTradeSuccess(AcceptTradeResponse _) {
            Console.Log("AcceptTradeSuccess!");

            selectionPool.DeactivateObj(gameObject);
        }

        private void OnAcceptTradeFailure(PlayFabError _) {
            Console.LogError("AcceptTradeFailure!");
        }

        private void OnGetUserInventoryFailure(PlayFabError _) {
            Console.LogError("GetUserInventoryFailure!");
        }

        private void OnCancelTradeSuccess(CancelTradeResponse _) {
            Console.Log("CancelTradeSuccess!");

            selectionPool.DeactivateObj(gameObject);
        }

        private void OnCancelTradeFailure(PlayFabError _) {
            Console.LogError("CancelTradeFailure!");
        }

        private void OnExecuteCloudScriptUpdateFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptUpdateFailure!");
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");
        }
    }
}