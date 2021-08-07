using IWP.General;
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

        [SerializeField]
        private int amtOfFriendSelections;

        [SerializeField]
        private ObjPool friendSelectionPool;

        [SerializeField]
        private ObjPool friendRequestSelectionPool;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FriendList(): base() {
            friendSelectionPrefab = null;
            contentTransform = null;

            amtOfFriendSelections = 0;
            friendSelectionPool = null;
            friendRequestSelectionPool = null;
        }

        static FriendList() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            friendSelectionPool.InitMe(
                amtOfFriendSelections,
                friendSelectionPrefab,
                contentTransform,
                false
            );
        }

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
                friendSelectionPool.DeactivateObj(child.gameObject);
                friendRequestSelectionPool.DeactivateObj(child.gameObject);
            }

            GameObject friendSelectionGO;
            foreach(FriendInfo friendInfo in result.Friends) {
                friendSelectionGO = friendSelectionPool.ActivateObj();
                friendSelectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = friendInfo.TitleDisplayName;
            }
        }

        private void OnGetFriendsListFailure(PlayFabError _) {
            Console.Log("GetFriendsListFailure!");
        }
    }
}