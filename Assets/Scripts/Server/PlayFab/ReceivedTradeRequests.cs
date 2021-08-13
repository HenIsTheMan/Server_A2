using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ReceivedTradeRequests: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject selectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private int amtOfSelections;

        [SerializeField]
        private ObjPool selectionPool;

        private string myPlayerID;

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

        internal ReceivedTradeRequests(): base() {
            selectionPrefab = null;
            contentTransform = null;

            amtOfSelections = 0;
            selectionPool = null;

            myPlayerID = string.Empty;
        }

        static ReceivedTradeRequests() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            selectionPool.InitMe(
                amtOfSelections,
                selectionPrefab,
                contentTransform,
                false
            );
        }

        #endregion

        public void OnClick() {
            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest(),
                OnGetAccountInfo1stSuccess,
                OnGetAccountInfo1stFailure
            );
        }

        private void OnGetAccountInfo1stSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfo1stSuccess!");

            myPlayerID = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = myPlayerID,
                        Key = "TradeRequests"
                    },
                    GeneratePlayStreamEvent = true,
                },
                OnExecuteCloudScriptGetSuccess,
                OnExecuteCloudScriptGetFailure
            );
        }

        private void OnExecuteCloudScriptGetSuccess(ExecuteCloudScriptResult result) {
            Console.Log("ExecuteCloudScriptGetSuccess!");

            foreach(Transform childTransform in contentTransform) {
                selectionPool.DeactivateObj(childTransform.gameObject);
            }

            GameObject selectionGO;
            GiftTradeSelection selection;

            JSONArray tempArr;
            JSONNode.Enumerator myEnumerator;
            int index;

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            foreach(JSONArray node in resultArr) { //Oh, can just do this way
                selectionGO = selectionPool.ActivateObj();
                selection = selectionGO.GetComponent<GiftTradeSelection>();

                selection.selectionPool = selectionPool;

                selection.otherDisplayName = node[0];
                selection.playerID = myPlayerID;
                selection.tradeID = node[4];

                selection.displayNameText.text = selection.otherDisplayName;

                tempArr = (JSONArray)JSON.Parse(node[1]);
                myEnumerator = tempArr.GetEnumerator();
                index = 0;

                while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                    selection.itemImgs[index++].enabled = myEnumerator.Current.Value;
                }

                tempArr = (JSONArray)JSON.Parse(node[2]);
                myEnumerator = tempArr.GetEnumerator();
                index = 0;

                while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                    selection.itemCountTexts[index++].text = ((int)myEnumerator.Current.Value).ToString();
                }
            }
        }

        private void OnExecuteCloudScriptGetFailure(PlayFabError _) {
            Console.LogError("ExecuteCloudScriptGetFailure!");
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");
        }
    }
}