using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class SendTradeRequestSelection: MonoBehaviour {
        #region Fields

        [SerializeField]
        internal TMP_Text displayNameText;

        [EnumIndices(typeof(ItemType)), SerializeField]
        internal TMP_Text[] itemCountTexts;

        [EnumIndices(typeof(ItemType)), SerializeField]
        internal Image[] itemImgs;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SendTradeRequestSelection(): base() {
            displayNameText = null;
            itemCountTexts = System.Array.Empty<TMP_Text>();
            itemImgs = System.Array.Empty<Image>();
        }

        static SendTradeRequestSelection() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnValidate() {
            UnityEngine.Assertions.Assert.IsTrue(itemCountTexts.Length == itemImgs.Length);
        }

        #endregion
    }
}