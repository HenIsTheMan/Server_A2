using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using TMPro;
using UnityEngine;

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

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal FriendRequests() : base() {
            friendRequestSelectionPrefab = null;
            contentTransform = null;

            amtOfFriendRequestSelections = 0;
            friendRequestSelectionPool = null;
            friendSelectionPool = null;

            searchInputField = null;
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

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
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

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            JSONNode.Enumerator myEnumerator = resultArr.GetEnumerator();

            GameObject friendRequestSelectionGO;
            while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                friendRequestSelectionGO = friendRequestSelectionPool.ActivateObj();
                friendRequestSelectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = myEnumerator.Current.Value;
                friendRequestSelectionGO.transform.GetChild(1)
                    .GetComponent<AcceptFriendRequest>().friendRequestSelectionPool = friendRequestSelectionPool;
                friendRequestSelectionGO.transform.GetChild(2)
                    .GetComponent<RemoveFriendRequest>().friendRequestSelectionPool = friendRequestSelectionPool;
            }
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");
        }

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.Log("GetAccountInfoFailure!");
        }
    }
}