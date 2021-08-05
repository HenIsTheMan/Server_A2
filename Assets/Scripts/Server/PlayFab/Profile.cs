using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Profile: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text usernameTextTmp;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Profile(): base() {
            usernameTextTmp = null;
        }

        static Profile() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            if(PlayFabClientAPI.IsClientLoggedIn()) {
                PlayFabClientAPI.GetAccountInfo(
                    new GetAccountInfoRequest(),
                    OnGetAccountInfoSuccess,
                    OnGetAccountInfoFailure
                );
            }
        }

        #endregion

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            usernameTextTmp.text = result.AccountInfo.Username;
        }

        private void OnGetAccountInfoFailure(PlayFabError error) {
            Console.LogError("GetAccountInfoFailure!");
        }
    }
}