using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ResetPassword: MonoBehaviour {
        #region Fields

        private bool canClick;

        [SerializeField]
        private TMP_InputField emailInputField;

        [SerializeField]
        private TMP_Text userFeedbackTmp;

        [SerializeField]
        private EllipsesControl ellipsesControl;

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
            canClick = true;

            emailInputField = null;

            userFeedbackTmp = null;

            ellipsesControl = null;

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

        private void Awake() {
            userFeedbackTmp.text = string.Empty;
        }

        #endregion

        public void OnClick() {
            if(!canClick) {
                return;
            }
            canClick = false;

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

                ellipsesControl.enabled = true;
                userFeedbackTmp.text = sendingAcctRecoveryEmailText;
                userFeedbackTmp.color = sendingAcctRecoveryEmailTextColor;
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

            ellipsesControl.enabled = true;
            userFeedbackTmp.text = sendingAcctRecoveryEmailText;
            userFeedbackTmp.color = sendingAcctRecoveryEmailTextColor;
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.LogError("GetAccountInfoFailure!");

            ellipsesControl.enabled = false;
            userFeedbackTmp.text = failedToSendAcctRecoveryEmailText;
            userFeedbackTmp.color = failedToSendAcctRecoveryEmailTextColor;

            canClick = true;
        }

        private void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult _) {
            Console.Log("SendAccountRecoveryEmailSuccess!");

            ellipsesControl.enabled = false;
            userFeedbackTmp.text = sentAcctRecoveryEmailText;
            userFeedbackTmp.color = sentAcctRecoveryEmailTextColor;

            canClick = true;
        }

        private void OnSendAccountRecoveryEmailFailure(PlayFabError error) {
            Console.LogError("SendAccountRecoveryEmailFailure!");

            ellipsesControl.enabled = false;
            userFeedbackTmp.text = sentAcctRecoveryEmailText + ' ' + error.ErrorMessage;
            userFeedbackTmp.color = failedToSendAcctRecoveryEmailTextColor;

            canClick = true;
        }
    }
}