using UnityEngine;

namespace Server.PlayFab {
    internal sealed class InvButton: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject invPanelGO;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal InvButton(): base() {
            invPanelGO = null;
        }

        static InvButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            invPanelGO.SetActive(true);
            enabled = false;
        }
    }
}