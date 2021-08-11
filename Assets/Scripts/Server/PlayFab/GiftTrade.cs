using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class GiftTrade: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal GiftTrade(): base() {
        }

        static GiftTrade() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                OnGetUserInventorySuccess,
                OnGetUserInventoryFailure
            );
        }

        #endregion

        private void OnGetUserInventorySuccess(GetUserInventoryResult result) {
            Console.Log("GetUserInventorySuccess!");

            result.Inventory.ForEach(instance => Console.Log(instance.ItemId));
        }

        private void OnGetUserInventoryFailure(PlayFabError error) {
            Console.LogError("GetUserInventoryFailure!");
        }
    }
}