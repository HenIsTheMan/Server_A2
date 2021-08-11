using UnityEngine;
using UnityEngine.UI;

namespace Server.PlayFab {
    internal sealed class InvPanelCloseButton: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject invPanelGO;

        [SerializeField]
        private Button invButton;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal InvPanelCloseButton(): base() {
            invPanelGO = null;
            invButton = null;
        }

        static InvPanelCloseButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            invPanelGO.SetActive(false);
            invButton.enabled = true;
        }
    }
}