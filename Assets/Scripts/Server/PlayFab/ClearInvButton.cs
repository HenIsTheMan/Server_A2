using PlayFab;
using PlayFab.ServerModels;
using Server.General;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ClearInvButton: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ClearInvButton(): base() {
        }

        static ClearInvButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabServerAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                OnGetUserInventorySuccess,
                OnGetUserInventoryFailure
            );
        }

        private void OnGetUserInventorySuccess(GetUserInventoryResult result) {
            Console.Log("GetUserInventorySuccess!");

            foreach(ItemInstance instance in result.Inventory) {
                PlayFabServerAPI.RevokeInventoryItem(
                    new RevokeInventoryItemRequest() {
                    },
                    OnRevokeInventoryItemSuccess,
                    OnRevokeInventoryItemFailure
                );
            }
        }

        private void OnRevokeInventoryItemSuccess(RevokeInventoryResult _) {
            Console.Log("RevokeInventoryItemSuccess!");
        }

        private void OnRevokeInventoryItemFailure(PlayFabError _) {
            Console.LogError("RevokeInventoryItemFailure!");
        }

        private void OnGetUserInventoryFailure(PlayFabError _) {
            Console.LogError("GetUserInventoryFailure!");
        }
    }
}