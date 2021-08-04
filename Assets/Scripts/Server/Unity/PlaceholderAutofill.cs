using TMPro;
using UnityEngine;

namespace Server.Unity {
    [ExecuteInEditMode]
    internal sealed class PlaceholderAutofill: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text placeholderTmp;

        [SerializeField]
        private TMP_Text myTmpRef;

        [SerializeField]
        private string frontText;

        [SerializeField]
        private string backText;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal PlaceholderAutofill(): base() {
            placeholderTmp = null;
            myTmpRef = null;
            frontText = string.Empty;
            backText = string.Empty;
        }

        static PlaceholderAutofill() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Update() {
            placeholderTmp.text = frontText + myTmpRef.text + backText;
        }

        #endregion
    }
}