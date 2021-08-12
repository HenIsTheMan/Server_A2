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
            if(!invPanelGO.activeInHierarchy) {
                _ = StartCoroutine(nameof(OpenInv));
            }
        }

        private System.Collections.IEnumerator OpenInv() {
            while(GiftTrade.globalObj.IsInvUpdating) {
                yield return null;
            }

            invPanelGO.SetActive(true);
            enabled = false;
        }
    }
}