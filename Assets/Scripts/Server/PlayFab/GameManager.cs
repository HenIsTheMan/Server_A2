using TMPro;
using UnityEngine;

namespace Server.PlayFab {
    internal sealed class GameManager: MonoBehaviour {
        #region Fields

        private int score;

        [SerializeField]
        private string frontText;

        [SerializeField]
        private TMP_Text scoreTmp;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal GameManager(): base() {
            score = 0;

            frontText = string.Empty;
            scoreTmp = null;
        }

        static GameManager() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void OnEnable() { //??
            score = 0;
        }

        private void Update() { //Lame
            scoreTmp.text = frontText + score;
        }

        private void OnDisable() { //??
        }

        #endregion

        public void AddToScore(int amt) {
            score += amt;
        }
    }
}