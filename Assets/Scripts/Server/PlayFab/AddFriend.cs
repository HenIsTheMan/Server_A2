using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using static Server.PlayFab.AddFriendTypes;

namespace Server.PlayFab {
    internal sealed class AddFriend: MonoBehaviour {
        #region Fields

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private TMP_InputField inputField;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal AddFriend(): base() {
        }

        static AddFriend() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            AddFriendRequest request = new AddFriendRequest();

            switch((AddFriendType)dropdown.value) {
                case AddFriendType.DisplayName:
                    request.FriendTitleDisplayName = inputField.text;
                    break;
                case AddFriendType.Username:
                    request.FriendUsername = inputField.text;
                    break;
                case AddFriendType.Email:
                    request.FriendEmail = inputField.text;
                    break;
                case AddFriendType.PlayFabID:
                    request.FriendPlayFabId = inputField.text;
                    break;
            }

            PlayFabClientAPI.AddFriend(
                request,
                OnAddFriendSuccess,
                OnAddFriendFailure
            );
        }

		private void OnAddFriendSuccess(AddFriendResult result) {
            Console.Log("AddFriendSuccess!");
        }

        private void OnAddFriendFailure(PlayFabError error) {
            Console.Log("AddFriendFailure!");
        }
    }
}