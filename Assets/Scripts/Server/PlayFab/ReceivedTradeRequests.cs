using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using SimpleJSON;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class ReceivedTradeRequests: MonoBehaviour {
        #region Fields

        private string myPlayFabID;
        private string otherPlayFabID;

        [SerializeField]
        private GameObject selectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private int amtOfSelections;

        [SerializeField]
        private ObjPool selectionPool;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal ReceivedTradeRequests(): base() {
            myPlayFabID = string.Empty;
            otherPlayFabID = string.Empty;

            selectionPrefab = null;
            contentTransform = null;

            amtOfSelections = 0;
            selectionPool = null;
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

            myPlayFabID = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.GetAccountInfo(
                new GetAccountInfoRequest() {
                    PlayFabId = otherPlayFabID
                },
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure
            );
        }

        private void OnGetAccountInfoSuccess(GetAccountInfoResult result) {
            Console.Log("GetAccountInfoSuccess!");

            otherPlayFabID = result.AccountInfo.PlayFabId;

            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest() {
                    FunctionName = "GetUserReadOnlyData",
                    FunctionParameter = new {
                        PlayFabID = myPlayFabID,
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

            GameObject selectionGO;
            SendTradeRequestSelection selection;

            JSONArray tempArr;
            JSONNode.Enumerator myEnumerator;
            int index;

            JSONArray resultArr = (JSONArray)JSON.Parse((string)result.FunctionResult);
            foreach(JSONArray node in resultArr) { //Oh, can just do this way
                selectionGO = selectionPool.ActivateObj();
                selection = selectionGO.GetComponent<SendTradeRequestSelection>();

                selection.selectionPool = selectionPool;

               selection.playerID = otherPlayFabID;
                selection.tradeID = node[3];

                selection.displayNameText.text = node[0];

                tempArr = (JSONArray)JSON.Parse(node[1]);
                myEnumerator = tempArr.GetEnumerator();
                index = 0;

                while(myEnumerator.MoveNext()) { //Iterate through JSONArray
                    if(myEnumerator.Current.Value) {
                        selection.itemImgs[index++].enabled = true;
                    }
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

        private void OnGetAccountInfoFailure(PlayFabError _) {
            Console.LogError("GetAccountInfoFailure!");
        }

        private void OnGetAccountInfo1stFailure(PlayFabError _) {
            Console.LogError("GetAccountInfo1stFailure!");
        }
    }
}