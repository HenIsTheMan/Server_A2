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

        [SerializeField]
        private TMP_Text profileMsgTmp;

        [SerializeField]
        private string sendingAcctRecoveryEmailText;

        [SerializeField]
        private Color sendingAcctRecoveryEmailTextColor;

        [SerializeField]
        private string sentAcctRecoveryEmailText;

        [SerializeField]
        private Color sentAcctRecoveryEmailTextColor;

        [SerializeField]
        private string failedToSendAcctRecoveryEmailText;

        [SerializeField]
        private Color failedToSendAcctRecoveryEmailTextColor;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ResetPassword(): base() {
            emailInputField = null;

            profileMsgTmp = null;

            sendingAcctRecoveryEmailText = string.Empty;
            sendingAcctRecoveryEmailTextColor = Color.white;

            sentAcctRecoveryEmailText = string.Empty;
            sentAcctRecoveryEmailTextColor = Color.white;

            failedToSendAcctRecoveryEmailText = string.Empty;
            failedToSendAcctRecoveryEmailTextColor = Color.white;
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

                profileMsgTmp.text = sendingAcctRecoveryEmailText;
                profileMsgTmp.color = sendingAcctRecoveryEmailTextColor;
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

            profileMsgTmp.text = sendingAcctRecoveryEmailText;
            profileMsgTmp.color = sendingAcctRecoveryEmailTextColor;
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.LogError("GetAccountInfoFailure!");

            profileMsgTmp.text = failedToSendAcctRecoveryEmailText;
            profileMsgTmp.color = failedToSendAcctRecoveryEmailTextColor;
        }

        private void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult _) {
            Console.Log("SendAccountRecoveryEmailSuccess!");

            profileMsgTmp.text = sentAcctRecoveryEmailText;
            profileMsgTmp.color = sentAcctRecoveryEmailTextColor;
        }

        private void OnSendAccountRecoveryEmailFailure(PlayFabError _) {
            Console.LogError("SendAccountRecoveryEmailFailure!");

            profileMsgTmp.text = failedToSendAcctRecoveryEmailText;
            profileMsgTmp.color = failedToSendAcctRecoveryEmailTextColor;
        }
    }
}