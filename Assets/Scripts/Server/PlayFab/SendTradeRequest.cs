using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class SendTradeRequest: MonoBehaviour {
        #region Fields

        [EnumIndices(typeof(ItemType)), SerializeField]
        private Toggle[] toggles;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private TMP_Text[] offerCountTexts;

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SendTradeRequest() : base() {
            toggles = System.Array.Empty<Toggle>();
            offerCountTexts = System.Array.Empty<TMP_Text>();

            dropdown = null;
            inputField = null;
        }

        static SendTradeRequest() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
        }
    }
}