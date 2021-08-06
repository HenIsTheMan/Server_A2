using PlayFab;
using PlayFab.AdminModels;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Server.PlayFab {
    internal sealed class DeleteAcct: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Text editAcctMsgTmp;

        [SerializeField]
        private EllipsesControl editAcctEllipsesControl;

        [SerializeField]
        private string deletingAcctText;

        [SerializeField]
        private Color deletingAcctTextColor;

        [SerializeField]
        private string acctDeletedText;

        [SerializeField]
        private Color acctDeletedTextColor;

        [SerializeField]
        private UnityEvent myUnityEvent;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal DeleteAcct(): base() {
            editAcctMsgTmp = null;
            editAcctEllipsesControl = null;

            deletingAcctText = string.Empty;
            deletingAcctTextColor = Color.white;

            acctDeletedText = string.Empty;
            acctDeletedTextColor = Color.white;

            myUnityEvent = null;
        }

        static DeleteAcct() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );

            editAcctEllipsesControl.enabled = true;
            editAcctMsgTmp.text = deletingAcctText;
            editAcctMsgTmp.color = deletingAcctTextColor;
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            PlayFabAdminAPI.DeleteMasterPlayerAccount(
                new DeleteMasterPlayerAccountRequest {
                    PlayFabId = result.AccountInfo.PlayFabId
                },
                OnDeleteMasterPlayerAccountSuccess,
                OnDeleteMasterPlayerAccountFailure
            );
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.LogError("GetAccountInfoFailure!");
        }

        private void OnDeleteMasterPlayerAccountSuccess(DeleteMasterPlayerAccountResult result) {
            Console.Log("DeleteMasterPlayerAccountSuccess!");

            PlayFabClientAPI.ForgetAllCredentials();

            editAcctEllipsesControl.enabled = false;
            editAcctMsgTmp.text = acctDeletedText;
            editAcctMsgTmp.color = acctDeletedTextColor;

            myUnityEvent?.Invoke();
        }

        private void OnDeleteMasterPlayerAccountFailure(PlayFabError _) {
            Console.LogError("DeleteMasterPlayerAccountFailure!");
        }
    }
}