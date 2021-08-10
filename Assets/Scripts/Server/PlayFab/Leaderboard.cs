using IWP.General;
using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Server.PlayFab {
    internal sealed class Leaderboard: MonoBehaviour {
        #region Fields

        [SerializeField]
        private GameObject selectionPrefab;

        [SerializeField]
        private Transform contentTransform;

        [SerializeField]
        private int amtOfSelections;

        [SerializeField]
        private ObjPool selectionPool;

        [SerializeField]
        private Button updateLeaderboardButton;

        [SerializeField]
        private int leaderboardMaxResultsCount;

        [SerializeField]
        private int leaderboardStartPos;

        [SerializeField]
        private string leaderboardStatisticName;

        private GetLeaderboardRequest getLeaderboardRequest;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Leaderboard() : base() {
            selectionPrefab = null;
            contentTransform = null;
            amtOfSelections = 0;
            selectionPool = null;
            updateLeaderboardButton = null;

            leaderboardMaxResultsCount = 0;
            leaderboardStartPos = 0;
            leaderboardStatisticName = string.Empty;

            getLeaderboardRequest = null;
        }

        static Leaderboard() {
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

            getLeaderboardRequest = new GetLeaderboardRequest();
        }

        #endregion

        public void OnClick() {
            updateLeaderboardButton.enabled = false;

            getLeaderboardRequest.MaxResultsCount = leaderboardMaxResultsCount;
            getLeaderboardRequest.StartPosition = leaderboardStartPos;
            getLeaderboardRequest.StatisticName = leaderboardStatisticName;

            PlayFabClientAPI.GetLeaderboard(
                getLeaderboardRequest,
                OnGetLeaderboardSuccess,
                OnGetLeaderboardFailure
            );
        }

        private void OnGetLeaderboardSuccess(GetLeaderboardResult result) {
			foreach(Transform child in contentTransform) {
				selectionPool.DeactivateObj(child.gameObject);
			}

			GameObject selectionGO;
			foreach(PlayerLeaderboardEntry entry in result.Leaderboard) {
				selectionGO = selectionPool.ActivateObj();
				selectionGO.transform.GetChild(0).GetComponent<TMP_Text>().text = entry.DisplayName;
				selectionGO.transform.GetChild(1).GetComponent<TMP_Text>().text = entry.StatValue.ToString();
			}

			updateLeaderboardButton.enabled = true;
        }

        private void OnGetLeaderboardFailure(PlayFabError error) {
            Console.Log("GetGetLeaderboardFailure! " + error.GenerateErrorReport());

            updateLeaderboardButton.enabled = true;
        }
    }
}