using IWP.General;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace Server.PlayFab {
    internal sealed class Leaderboards: MonoBehaviour {
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
        private Button updateLeaderboardsButton;

        private GetLeaderboardRequest getLeaderboardRequest;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Leaderboards() : base() {
            selectionPrefab = null;
            contentTransform = null;
            amtOfSelections = 0;
            selectionPool = null;
            updateLeaderboardsButton = null;
            getLeaderboardRequest = null;
        }

        static Leaderboards() {
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
            updateLeaderboardsButton.enabled = false;
        }
    }
}