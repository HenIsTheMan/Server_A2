using TMPro;
using UnityEngine;

namespace Server.Unity {
    [ExecuteInEditMode]
    internal sealed class TextAutofill: MonoBehaviour {
        #region Fields

        [SerializeField]
        private bool shldRunInEditor;

        [SerializeField]
        private TMP_Text myTmp;

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

        internal TextAutofill(): base() {
            shldRunInEditor = false;
            myTmp = null;
            myTmpRef = null;
            frontText = string.Empty;
            backText = string.Empty;
        }

        static TextAutofill() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Update() {
            if(shldRunInEditor) {
                myTmp.text = frontText + myTmpRef.text + backText;
            }
        }

        #endregion
    }
}