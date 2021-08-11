using PlayFab;
using PlayFab.ServerModels;
using Server.General;
using System.Collections.Generic;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Store: MonoBehaviour {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Store(): base() {
        }

        static Store() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnBuyButtonClicked(string itemID) {
            PlayFabServerAPI.GrantItemsToUser(
                new GrantItemsToUserRequest() {
                    ItemIds = new List<string> {
                        itemID
                    }
                },
                OnGrantItemsToUserSuccess,
                OnGrantItemsToUserFailure
            );
        }

        private void OnGrantItemsToUserSuccess(GrantItemsToUserResult result) {
            Console.Log("GrantItemsToUserSuccess!");

            result.ItemGrantResults.ForEach(instance => Console.Log(instance.ItemId));
        }

        private void OnGrantItemsToUserFailure(PlayFabError _) {
            Console.LogError("GrantItemsToUserFailure!");
        }
    }
}