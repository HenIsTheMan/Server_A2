using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ResetPassword: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_InputField emailInputField;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ResetPassword(): base() {
            emailInputField = null;
        }

        static ResetPassword() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            if(emailInputField == null) {
                if(PlayFabClientAPI.IsClientLoggedIn()) {
                    PlayFabClientAPI.GetAccountInfo(
                        new GetAccountInfoRequest(),
                        OnGetAccountInfoSuccess,
                        OnGetAccountInfoFailure
                    );
                }
            } else {
                PlayFabClientAPI.SendAccountRecoveryEmail(
                    new SendAccountRecoveryEmailRequest {
                        Email = emailInputField.text,
                        TitleId = PlayFabSettings.staticSettings.TitleId
                    },
                    OnSendAccountRecoveryEmailSuccess,
                    OnSendAccountRecoveryEmailFailure
                );
            }
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            PlayFabClientAPI.SendAccountRecoveryEmail(
                new SendAccountRecoveryEmailRequest {
                    Email = result.AccountInfo.PrivateInfo.Email,
                    TitleId = PlayFabSettings.staticSettings.TitleId
                },
                OnSendAccountRecoveryEmailSuccess,
                OnSendAccountRecoveryEmailFailure
            );
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.LogError("GetAccountInfoFailure!");
        }

        private void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult _) {
            Console.Log("SendAccountRecoveryEmailSuccess!");
        }

        private void OnSendAccountRecoveryEmailFailure(PlayFabError _) {
            Console.LogError("SendAccountRecoveryEmailFailure!");
        }
    }
}