using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Server {
    internal sealed class PlayFabFriendList: MonoBehaviour {
        #region Fields

        List<FriendInfo> _friends = null;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal PlayFabFriendList(): base() {
        }

        static PlayFabFriendList() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnGetFriendsListClicked() {
            GetFriends();
        }

        void GetFriends() {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest {
                IncludeSteamFriends = false,
                IncludeFacebookFriends = false,
                XboxToken = null
            }, result => {
                _friends = result.Friends;
                DisplayFriends(_friends); //triggers your UI
            }, DisplayPlayFabError);
        }

        void DisplayFriends(List<FriendInfo> friendsCache) {
            Debug.Log(string.Format("There are {0} friend(s)", friendsCache.Count));
            friendsCache.ForEach(f => Debug.Log(f.FriendPlayFabId));
        }

        void DisplayPlayFabError(PlayFabError error) {
            Debug.Log(error.GenerateErrorReport());
        }
    }
}