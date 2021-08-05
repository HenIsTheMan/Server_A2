using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Server.PlayFab {
    internal sealed class SignOut: MonoBehaviour {
        #region Fields

        private bool canClick;

        [SerializeField]
        private UnityEvent myUnityEvent;

        [SerializeField]
        private TMP_Text profileMsgTmp;

        [SerializeField]
        private string signedOutText;

        [SerializeField]
        private Color signedOutTextColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignOut(): base() {
            canClick = true;

            myUnityEvent = null;

            profileMsgTmp = null;

            signedOutText = string.Empty;

            signedOutTextColor = Color.white;
        }

        static SignOut() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            if(!canClick) {
                return;
            }
            canClick = false;

            PlayFabClientAPI.ForgetAllCredentials();

            profileMsgTmp.text = signedOutText;
            profileMsgTmp.color = signedOutTextColor;

            myUnityEvent?.Invoke();
        }
    }
}