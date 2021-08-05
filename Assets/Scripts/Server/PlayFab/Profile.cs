using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Profile: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text displayNameTextTmp;

        [SerializeField]
        private TMP_Text contactEmailTextTmp;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Profile(): base() {
            displayNameTextTmp = null;
            contactEmailTextTmp = null;
        }

        static Profile() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            if(PlayFabClientAPI.IsClientLoggedIn()) {
                PlayFabClientAPI.GetAccountInfo(
                    new GetAccountInfoRequest (),
                    OnGetAcctInfoSuccess,
                    OnGetAcctInfoFailure
                );
            }
        }

        #endregion

        private void OnGetAcctInfoSuccess(GetAccountInfoResult result) {
            PlayFabClientAPI.GetPlayerProfile(
                new GetPlayerProfileRequest {
                    PlayFabId = result.AccountInfo.PlayFabId,
                    ProfileConstraints = new PlayerProfileViewConstraints {
                        ShowContactEmailAddresses = true,
                        ShowDisplayName = true
                    }
                },
                OnGetPlayerProfileSuccess,
                OnGetPlayerProfileFailure
            );
        }

        private void OnGetAcctInfoFailure(PlayFabError _) {
            Console.LogError("GetAcctInfoFailure!");
        }

        private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result) {
            Console.Log("GetPlayerProfileSuccess!");

            displayNameTextTmp.text = result.PlayerProfile.DisplayName;
            contactEmailTextTmp.text = result.PlayerProfile.ContactEmailAddresses[0].EmailAddress;
        }

        private void OnGetPlayerProfileFailure(PlayFabError _) {
            Console.LogError("GetPlayerProfileFailure!");
        }
    }
}