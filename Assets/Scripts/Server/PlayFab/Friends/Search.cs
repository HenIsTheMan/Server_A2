using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Search: MonoBehaviour {
        #region Fields

        private Dictionary<string, GameObject> mySelectionLinks;

        [SerializeField]
        private TMP_InputField searchInputField;

        #endregion

        #region Properties

        internal Dictionary<string, GameObject> MySelectionLinks {
            get => mySelectionLinks;
            private set => mySelectionLinks = value;
        }

        #endregion

        #region Ctors and Dtor

        internal Search(): base() {
            mySelectionLinks = null;
            searchInputField = null;
        }

        static Search() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            mySelectionLinks = new Dictionary<string, GameObject>();

            searchInputField.onValueChanged.AddListener(delegate {
                string searchText = searchInputField.text;
                int searchTextLen = searchText.Length;

                foreach(string key in mySelectionLinks.Keys) {
                    mySelectionLinks[key].SetActive(searchTextLen <= key.Length && searchText == key.Substring(0, searchTextLen));
                }
            });
        }

        #endregion
    }
}