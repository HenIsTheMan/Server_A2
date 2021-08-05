using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace Server.PlayFab {
    internal sealed class SignOut: MonoBehaviour {
        #region Fields

        [SerializeField]
        private UnityEvent myUnityEvent;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal SignOut(): base() {
            myUnityEvent = null;
        }

        static SignOut() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.ForgetAllCredentials();

            myUnityEvent?.Invoke();
        }
    }
}