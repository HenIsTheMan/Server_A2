using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Server.PlayFab.ItemTypes;

namespace Server.PlayFab {
    internal sealed class SendTradeRequest: MonoBehaviour {
        #region Fields

        private string displayNameOfRequester;
        private string playFabIdOfRequestee;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private Toggle[] toggles;

        [EnumIndices(typeof(ItemType)), SerializeField]
        private TMP_Text[] offerCountTexts;

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TMP_Text sendTradeRequestMsg;

        [SerializeField]
        private EllipsesControl sendTradeRequestEllipsesControl;

        [SerializeField]
        private string sendingText;

        [SerializeField]
        private Color sendingColor;

        [SerializeField]
        private string sentText;

        [SerializeField]
        private Color sentColor;

        [SerializeField]
        private string failedToSendText;

        [SerializeField]
        private Color failedToSendColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SendTradeRequest() : base() {
            displayNameOfRequester = string.Empty;
            playFabIdOfRequestee = string.Empty;

            toggles = System.Array.Empty<Toggle>();
            offerCountTexts = System.Array.Empty<TMP_Text>();

            dropdown = null;
            inputField = null;

            sendTradeRequestMsg = null;
            sendTradeRequestEllipsesControl = null;

            sendingText = string.Empty;
            sendingColor = Color.white;

            sentText = string.Empty;
            sentColor = Color.white;

            failedToSendText = string.Empty;
            failedToSendColor = Color.white;
        }

        static SendTradeRequest() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfo1stSuccess,
                OnGetAccountInfo1stFailure
            );

            MyProcessingFunc();
        }

        private void MyProcessingFunc() {
            sendTradeRequestEllipsesControl.enabled = true;
            sendTradeRequestMsg.text = sendingText;
            sendTradeRequestMsg.color = sendingColor;
        }

        private void MySuccessFunc() {
            sendTradeRequestEllipsesControl.enabled = false;
            sendTradeRequestMsg.text = sentText;
            sendTradeRequestMsg.color = sentColor;
        }

        private void MyFailureFunc() {
            sendTradeRequestEllipsesControl.enabled = false;
            sendTradeRequestMsg.text = failedToSendText;
            sendTradeRequestMsg.color = failedToSendColor;
        }
    }
}