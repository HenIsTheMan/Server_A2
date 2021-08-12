using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class GiftTrade: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties

        internal static bool IsInvUpdating {
            get;
            private set;
        }

        #endregion

        #region Ctors and Dtor

        internal GiftTrade(): base() {
        }

        static GiftTrade() {
            IsInvUpdating = false;
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            UpdateInv();
        }

        #endregion

        internal static void UpdateInv() {
            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                OnGetUserInventorySuccess,
                OnGetUserInventoryFailure
            );

            IsInvUpdating = true;
        }

        private static void OnGetUserInventorySuccess(GetUserInventoryResult result) {
            Console.Log("GetUserInventorySuccess!");

            result.Inventory.ForEach(instance => Console.Log(instance.ItemId));

            IsInvUpdating = false;
        }

        private static void OnGetUserInventoryFailure(PlayFabError error) {
            Console.LogError("GetUserInventoryFailure!");

            IsInvUpdating = false;
        }
    }
}