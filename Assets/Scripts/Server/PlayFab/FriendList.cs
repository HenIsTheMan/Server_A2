using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class FriendList: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject friendSelectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FriendList(): base() {
            friendSelectionPrefab = null;
            contentTransform = null;
        }

        static FriendList() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetFriendsList(
                new GetFriendsListRequest {
                    IncludeSteamFriends = false,
                    IncludeFacebookFriends = true,
                    XboxToken = null
                },
                OnGetFriendsListSuccess,
                OnGetFriendsListFailure
            );
        }

        private void OnGetFriendsListSuccess(GetFriendsListResult result) {
            Console.Log("GetFriendsListSuccess!");

            foreach(Transform child in contentTransform) {
                Destroy(child.gameObject);
            }

            GameObject friendSelectionGO;
            foreach(FriendInfo friendInfo in result.Friends) {
                friendSelectionGO = Instantiate(friendSelectionPrefab, contentTransform);
                friendSelectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = friendInfo.TitleDisplayName;
            }
        }

        private void OnGetFriendsListFailure(PlayFabError _) {
            Console.Log("GetFriendsListFailure!");
        }
    }
}