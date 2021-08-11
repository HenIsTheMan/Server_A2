using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using Server.General;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Store: MonoBehaviour {
        #region Fields

        private string itemID;

        [SerializeField]
        private TMP_Text storeMsg;

        [SerializeField]
        private EllipsesControl storeEllipsesControl;

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

        internal Store(): base() {
            itemID = string.Empty;

            storeMsg = null;
            storeEllipsesControl = null;

            processingText = string.Empty;
            processingColor = Color.white;

            successText = string.Empty;
            successColor = Color.white;

            failureText = string.Empty;
            failureColor = Color.white;
        }

        static Store() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            storeMsg.text = string.Empty;
        }

        #endregion

        public void OnBuyButtonClicked(string itemID) {
            this.itemID = itemID;

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );

            MyProcessingFunc();
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            PlayFabServerAPI.GrantItemsToUser(
                new GrantItemsToUserRequest() {
                    ItemIds = new List<string> {
                        itemID
                    },
                    PlayFabId = result.AccountInfo.PlayFabId
                },
                OnGrantItemsToUserSuccess,
                OnGrantItemsToUserFailure
            );
        }

        private void OnGrantItemsToUserSuccess(GrantItemsToUserResult result) {
            Console.Log("GrantItemsToUserSuccess!");

            result.ItemGrantResults.ForEach(instance => Console.Log("Bought: " + instance.ItemId));

            MySuccessFunc();
        }

        private void OnGrantItemsToUserFailure(PlayFabError _) {
            Console.LogError("GrantItemsToUserFailure!");

            MyFailureFunc();
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");

            MyFailureFunc();
        }

        private void MyProcessingFunc() {
            storeEllipsesControl.enabled = true;
            storeMsg.text = processingText;
            storeMsg.color = processingColor;
        }

        private void MySuccessFunc() {
            storeEllipsesControl.enabled = false;
            storeMsg.text = successText;
            storeMsg.color = successColor;
        }

        private void MyFailureFunc() {
            storeEllipsesControl.enabled = false;
            storeMsg.text = failureText;
            storeMsg.color = failureColor;
        }
    }
}