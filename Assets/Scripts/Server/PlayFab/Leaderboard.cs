using IWP.General;
using PlayFab.ClientModels;
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
        }

        #endregion

        public void OnClick() {
            updateLeaderboardButton.enabled = false;
        }
    }
}