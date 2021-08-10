using System.Collections.Generic;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Search: MonoBehaviour {
        #region Fields

        private Dictionary<string, GameObject> mySelectionLinks;

        #endregion

        #region Properties

        internal Dictionary<string, GameObject> MySelectionLinks {
            get;
            private set;
        }

        #endregion

        #region Ctors and Dtor

        internal Search(): base() {
            mySelectionLinks = null;
        }

        static Search() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            mySelectionLinks = new Dictionary<string, GameObject>();
        }

        #endregion
    }
}