using PlayFab;
using PlayFab.ClientModels;
using Server.General;
using System.Collections.Generic;
using System.Linq;
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

        private void OnEnable() {
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest() {
                    StatisticNames = new List<string> {
                        "score"
                    }
                },
                OnGetPlayerStatisticsSuccess,
                OnGetPlayerStatisticsFailure
            );
        }

        private void Update() { //Lame
            scoreTmp.text = frontText + score;
        }

        private void OnDisable() { //??
        }

        #endregion

        private void OnGetPlayerStatisticsSuccess(GetPlayerStatisticsResult result) {
            Console.Log("GetPlayerStatisticsSuccess!");

            score = result.Statistics.Any() ? result.Statistics[0].Value : 0;
        }

        private void OnGetPlayerStatisticsFailure(PlayFabError error) {
            Console.LogError("GetPlayerStatisticsFailure!");
        }

        public void AddToScore(int amt) {
            score += amt;
        }
    }
}