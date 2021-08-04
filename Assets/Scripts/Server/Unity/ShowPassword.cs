using TMPro;
using UnityEngine;
using static TMPro.TMP_InputField;

namespace Server.Unity {
    [ExecuteAlways]
    internal sealed class ShowPassword: MonoBehaviour {
        #region Fields

        private bool isPasswordHidden;

        [SerializeField]
        private TextMeshProUGUI showHidePasswordTmp;

        [SerializeField]
        private string showPasswordText;

        [SerializeField]
        private string hidePasswordText;

        [SerializeField]
        private TMP_InputField inputField;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ShowPassword(): base() {
            isPasswordHidden = true;

            showHidePasswordTmp = null;

            showPasswordText = string.Empty;
            hidePasswordText = string.Empty;

            inputField = null;
        }

        static ShowPassword() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            UnityEngine.Assertions.Assert.IsTrue(
                inputField.contentType == ContentType.Password
                || inputField.contentType == ContentType.Standard
            );

            isPasswordHidden = inputField.contentType == ContentType.Password;

            if(isPasswordHidden) {
                showHidePasswordTmp.SetText(showPasswordText);
            } else {
                showHidePasswordTmp.SetText(hidePasswordText);
            }
        }

        #if UNITY_EDITOR

        private void Update() {
            if(Application.isPlaying) {
                return;
            }
            
            Awake();
        }

        #endif

        #endregion

        public void OnClick() {
            isPasswordHidden = !isPasswordHidden;

            if(isPasswordHidden) {
                showHidePasswordTmp.SetText(showPasswordText);

                inputField.contentType = ContentType.Password;
            } else {
                showHidePasswordTmp.SetText(hidePasswordText);

                inputField.contentType = ContentType.Standard;
            }

            inputField.textComponent.SetAllDirty();
        }
    }
}