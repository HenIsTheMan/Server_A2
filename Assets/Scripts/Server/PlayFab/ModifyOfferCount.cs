using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ModifyOfferCount: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text offerCountText;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ModifyOfferCount(): base() {
            offerCountText = null;
        }

        static ModifyOfferCount() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnModifyOfferCountButtonClick(int modAmt) {
            offerCountText.text = (System.Convert.ToInt32(offerCountText.text) + modAmt).ToString();
        }
    }
}