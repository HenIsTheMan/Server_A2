using Server.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private TMP_InputField searchInputField;

        [SerializeField]
        private Button otherButton;

        [SerializeField]
        private Search search;

        [SerializeField]
        private TMP_Text friendsMsg;

        [SerializeField]
        private EllipsesControl friendsEllipsesControl;

        [SerializeField]
        private string processingText;

        [SerializeField]
        private Color processingColor;

        [SerializeField]
        private string successText;

        [SerializeField]
        private Color successColor;

        [SerializeField]
        private string failureText;

        [SerializeField]
        private Color failureColor;

        [SerializeField]
        private string removeProcessingText;

        [SerializeField]
        private string removeSuccessText;

        [SerializeField]
        private string removeFailureText;

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

            searchInputField = null;

            otherButton = null;

            search = null;

            friendsMsg = null;
            friendsEllipsesControl = null;

            processingText = string.Empty;
            processingColor = Color.white;

            successText = string.Empty;
            successColor = Color.white;

            failureText = string.Empty;
            failureColor = Color.white;

            removeProcessingText = string.Empty;
            removeSuccessText = string.Empty;
            removeFailureText = string.Empty;
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
            searchInputField.text = string.Empty;

            otherButton.enabled = false;

            PlayFabClientAPI.GetFriendsList(
                new GetFriendsListRequest {
                    IncludeSteamFriends = false,
                    IncludeFacebookFriends = true,
                    XboxToken = null
                },
                OnGetFriendsListSuccess,
                OnGetFriendsListFailure
            );

            MyProcessingFunc();
        }

        private void OnGetFriendsListSuccess(GetFriendsListResult result) {
            Console.Log("GetFriendsListSuccess!");

            foreach(Transform child in contentTransform) {
                friendSelectionPool.DeactivateObj(child.gameObject);
                friendRequestSelectionPool.DeactivateObj(child.gameObject);
            }
            search.MySelectionLinks.Clear();

            GameObject friendSelectionGO;
            foreach(FriendInfo friendInfo in result.Friends) {
                friendSelectionGO = friendSelectionPool.ActivateObj();

                friendSelectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = friendInfo.TitleDisplayName;

                RemoveFriend removeFriend = friendSelectionGO.transform.GetChild(2).GetComponent<RemoveFriend>();
                removeFriend.friendSelectionPool = friendSelectionPool;

                removeFriend.friendsMsg = friendsMsg;
                removeFriend.friendsEllipsesControl = friendsEllipsesControl;

                removeFriend.processingText = removeProcessingText;
                removeFriend.processingColor = processingColor;

                removeFriend.successText = removeSuccessText;
                removeFriend.successColor = successColor;

                removeFriend.failureText = removeFailureText;
                removeFriend.failureColor = failureColor;

                search.MySelectionLinks.Add(friendInfo.TitleDisplayName, friendSelectionGO);
            }

            otherButton.enabled = true;

            MySuccessFunc();
        }

        private void OnGetFriendsListFailure(PlayFabError _) {
            Console.Log("GetFriendsListFailure!");

            otherButton.enabled = true;

            MyFailureFunc();
        }

        private void MyProcessingFunc() {
            friendsEllipsesControl.enabled = true;
            friendsMsg.text = processingText;
            friendsMsg.color = processingColor;
        }

        private void MySuccessFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = successText;
            friendsMsg.color = successColor;
        }

        private void MyFailureFunc() {
            friendsEllipsesControl.enabled = false;
            friendsMsg.text = failureText;
            friendsMsg.color = failureColor;
        }
    }
}