using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Server.PlayFab {
    internal sealed class FriendRequests: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject friendRequestSelectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private int amtOfFriendRequestSelections;

        [SerializeField]
        private ObjPool friendRequestSelectionPool;

        [SerializeField]
        private ObjPool friendSelectionPool;

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

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FriendRequests(): base() {
            friendRequestSelectionPrefab = null;
            contentTransform = null;

            amtOfFriendRequestSelections = 0;
            friendRequestSelectionPool = null;
            friendSelectionPool = null;

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
        }

        static FriendRequests() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            friendRequestSelectionPool.InitMe(
                amtOfFriendRequestSelections,
                friendRequestSelectionPrefab,
                contentTransform,
                false
            );
        }

        #endregion

        public void OnClick() {
            searchInputField.text = string.Empty;

            otherButton.enabled = false;

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );

            MyProcessingFunc();
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = result.AccountInfo.PlayFabId,
                        Key = "FriendRequests"
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        private void OnExecuteCloudScriptGetSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptGetSuccess!");

            foreach(Transform child in contentTransform) {
                friendRequestSelectionPool.DeactivateObj(child.gameObject);
                friendSelectionPool.DeactivateObj(child.gameObject);
            }
            search.MySelectionLinks.Clear();

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            JSONNode.Enumerator myEnumerator = resultArr.GetEnumerator();

            GameObject friendRequestSelectionGO;
            while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                friendRequestSelectionGO = friendRequestSelectionPool.ActivateObj();

                friendRequestSelectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = myEnumerator.Current.Value;

                AcceptFriendRequest acceptFriendRequest = friendRequestSelectionGO.transform.GetChild(1)
                    .GetComponent<AcceptFriendRequest>();
                acceptFriendRequest.friendRequestSelectionPool = friendRequestSelectionPool;

                acceptFriendRequest.friendsMsg = friendsMsg;
                acceptFriendRequest.friendsEllipsesControl = friendsEllipsesControl;

                acceptFriendRequest.processingText = processingText;
                acceptFriendRequest.processingColor = processingColor;

                acceptFriendRequest.successText = successText;
                acceptFriendRequest.successColor = successColor;

                acceptFriendRequest.failureText = failureText;
                acceptFriendRequest.failureColor = failureColor;

                RemoveFriendRequest removeFriendRequest = friendRequestSelectionGO.transform.GetChild(2)
                    .GetComponent<RemoveFriendRequest>();
                removeFriendRequest.friendRequestSelectionPool = friendRequestSelectionPool;

                removeFriendRequest.friendsMsg = friendsMsg;
                removeFriendRequest.friendsEllipsesControl = friendsEllipsesControl;

                removeFriendRequest.processingText = processingText;
                removeFriendRequest.processingColor = processingColor;

                removeFriendRequest.successText = successText;
                removeFriendRequest.successColor = successColor;

                removeFriendRequest.failureText = failureText;
                removeFriendRequest.failureColor = failureColor;

                search.MySelectionLinks.Add(myEnumerator.Current.Value, friendRequestSelectionGO);
            }

            otherButton.enabled = true;

            MySuccessFunc();
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");

            otherButton.enabled = true;

            MyFailureFunc();
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");

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