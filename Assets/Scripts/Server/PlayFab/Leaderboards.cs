using IWP.General;
using UnityEngine;

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

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal Leaderboards() : base() {
            selectionPrefab = null;
            contentTransform = null;
            amtOfSelections = 0;
            selectionPool = null;
        }

        static Leaderboards() {
        }

        #endregion

        #region Unity User Callback Event Funcs
        #endregion

        public void OnClick() {
        }
    }
}