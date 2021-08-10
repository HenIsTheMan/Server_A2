using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class Search: MonoBehaviour {
        #region Fields

        private Dictionary<string, GameObject> mySelectionLinks;

        [SerializeField]
        private TMP_InputField searchInputField;

        [SerializeField]
        private TMP_Text friendsMsg;

        [SerializeField]
        private string foundFrontText;

        [SerializeField]
        private string foundBackText;

        [SerializeField]
        private Color foundColor;

        [SerializeField]
        private string notFoundText;

        [SerializeField]
        private Color notFoundColor;

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

            friendsMsg = null;

            foundFrontText = string.Empty;
            foundBackText = string.Empty;
            foundColor = Color.white;

            notFoundText = string.Empty;
            notFoundColor = Color.white;
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

                int amtFound = 0;

                foreach(string key in mySelectionLinks.Keys) {
                    if(searchTextLen <= key.Length && searchText == key.Substring(0, searchTextLen)) {
                        ++amtFound;
                        mySelectionLinks[key].SetActive(true);
                    } else {
                        mySelectionLinks[key].SetActive(false);
                    }
                }

                if(amtFound == 0) {
                    friendsMsg.text = notFoundText;
                    friendsMsg.color = notFoundColor;
                } else {
                    friendsMsg.text = foundFrontText + amtFound.ToString() + foundBackText;
                    friendsMsg.color = foundColor;
                }
            });
        }

        #endregion
    }
}