using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class GiftTrade: MonoBehaviour {
        #region Fields

        [EnumIndices(typeof(ItemType)), SerializeField]
        private static TMP_Text[] itemCountTexts;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private static string[] itemIDs;

        private static int[] itemCounts;

        private static int commonLen;

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
            itemCountTexts = System.Array.Empty<TMP_Text>();
            itemIDs = System.Array.Empty<string>();
            itemCounts = System.Array.Empty<int>();

            commonLen = 0;

            IsInvUpdating = false;
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(itemCountTexts.Length == itemIDs.Length);
        }

        private void Awake() {
            commonLen = itemIDs.Length;
            itemCounts = new int[commonLen];

            ResetItemCounts();

            UpdateInv();
        }

        #endregion

        private static void ResetItemCounts() {
            for(int i = 0; i < commonLen; ++i) {
                itemCounts[i] = 0;
            }
        }

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

            ResetItemCounts();

            foreach(ItemInstance instance in result.Inventory) {
                for(int i = 0; i < commonLen; ++i) {
                    if(instance.ItemId == itemIDs[i]) {
                        ++itemCounts[i];
                        break;
                    }
                }
			}

            for(int i = 0; i < commonLen; ++i) {
                itemCountTexts[i].text = itemCounts[i].ToString();
            }

            IsInvUpdating = false;
        }

        private static void OnGetUserInventoryFailure(PlayFabError error) {
            Console.LogError("GetUserInventoryFailure!");

            IsInvUpdating = false;
        }
    }
}