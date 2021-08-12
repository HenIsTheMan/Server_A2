using UnityEngine;

namespace Server.PlayFab {
    internal sealed class InvButton: MonoBehaviour {
        #region Fields

        private bool canClick;

        [SerializeField]
        private GameObject invPanelGO;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal InvButton(): base() {
            canClick = true;

            invPanelGO = null;
        }

        static InvButton() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            if(canClick) {
                _ = StartCoroutine(nameof(OpenInv));
                canClick = false;
            }
        }

        private System.Collections.IEnumerator OpenInv() {
            while(GiftTrade.globalObj.IsInvUpdating) {
                yield return null;
            }

            invPanelGO.SetActive(true);
            enabled = false;

            canClick = true;
        }
    }
}