using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Server.Ref {
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

        public void OnAddFriendClicked() {
            //2nd parameter should be another player account in your title
            AddFriend(FriendIdType.Email, "abc@test.com");
        }

        enum FriendIdType { PlayFabId, Username, Email, DisplayName };

        void AddFriend(FriendIdType idType, string friendId) {
            var request = new AddFriendRequest();
            switch(idType) {
                case FriendIdType.PlayFabId:
                    request.FriendPlayFabId = friendId;
                    break;
                case FriendIdType.Username:
                    request.FriendUsername = friendId;
                    break;
                case FriendIdType.Email:
                    request.FriendEmail = friendId;
                    break;
                case FriendIdType.DisplayName:
                    request.FriendTitleDisplayName = friendId;
                    break;
            }
            // Execute request and update friends when we are done
            PlayFabClientAPI.AddFriend(request, result => {
                Debug.Log("Friend added successfully!");
            }, DisplayPlayFabError);
        }
    }
}