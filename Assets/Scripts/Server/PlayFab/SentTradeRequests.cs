using Server.General;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class SentTradeRequests: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject selectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private int amtOfSelections;

        [SerializeField]
        private ObjPool selectionPool;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SentTradeRequests(): base() {
            selectionPrefab = null;
            contentTransform = null;

            amtOfSelections = 0;
            selectionPool = null;
        }

        static SentTradeRequests() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            selectionPool.InitMe(
                amtOfSelections,
                selectionPrefab,
                contentTransform,
                false
            );
        }

        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetPlayerTrades(
                new GetPlayerTradesRequest() {
                    StatusFilter = TradeStatus.Open
                },
                OnGetPlayerTradesSuccess,
                OnGetPlayerTradesFailure
            );
        }

        private void OnGetPlayerTradesSuccess(GetPlayerTradesResponse response) {
            Console.Log("GetPlayerTradesSuccess!");

            foreach(Transform childTransform in contentTransform) {
                selectionPool.DeactivateObj(childTransform.gameObject);
            }

            GameObject selectionGO;
            GiftTradeSelection selection;
            int len = (int)ItemType.Amt - 1;
            int[] itemCounts = new int[len];

            foreach(TradeInfo tradeInfo in response.OpenedTrades) {
                selectionGO = selectionPool.ActivateObj();
                selection = selectionGO.GetComponent<GiftTradeSelection>();

                selection.selectionPool = selectionPool;

                selection.playerID = tradeInfo.AllowedPlayerIds[0];
                selection.tradeID = tradeInfo.TradeId;

                selection.displayNameText.text = tradeInfo.AllowedPlayerIds[0]; //Lol

                for(int i = 0; i < len; ++i) {
                    itemCounts[i] = 0;
                }

                foreach(string itemID in tradeInfo.OfferedInventoryInstanceIds) {
                    for(int i = 0; i < len; ++i) {
                        if(itemID == selection.itemIDs[i]) {
                            ++itemCounts[i];
                            break;
                        }
                    }
                }

                for(int i = 0; i < len; ++i) {
                    selection.itemCountTexts[i].text = itemCounts[i].ToString();
                }

                foreach(string itemID in tradeInfo.RequestedCatalogItemIds) {
                    for(int i = 0; i < len; ++i) {
                        if(itemID == selection.itemIDs[i]) {
                            selection.itemImgs[i].enabled = true;
                            break;
                        }
                    }
                }
            }
        }

        private void OnGetPlayerTradesFailure(PlayFabError _) {
            Console.LogError("GetPlayerTradesFailure!");
        }
    }
}