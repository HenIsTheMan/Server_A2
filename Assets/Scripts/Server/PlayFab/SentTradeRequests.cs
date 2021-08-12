using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

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
                new GetPlayerTradesRequest(),
                OnGetPlayerTradesSuccess,
                OnGetPlayerTradesFailure
            );
        }

        private void OnGetPlayerTradesSuccess(GetPlayerTradesResponse response) {
            Console.Log("GetPlayerTradesSuccess!");

            GameObject selectionGO;
            foreach(TradeInfo tradeInfo in response.OpenedTrades) {
                selectionGO = selectionPool.ActivateObj();
            }
        }

        private void OnGetPlayerTradesFailure(PlayFabError _) {
            Console.LogError("GetPlayerTradesFailure!");
        }
    }
}