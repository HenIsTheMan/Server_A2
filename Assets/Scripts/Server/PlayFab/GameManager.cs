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

        [SerializeField]
        private string scoreStatisticName;

        private uint? scoreStatisticVer;

        private List<StatisticUpdate> myStatistics;

        private StatisticUpdate myStatisticUpdate;

        #endregion

        #region Properties
        #endregion

        #region Ctors and Dtor

        internal GameManager(): base() {
            score = 0;

            frontText = string.Empty;
            scoreTmp = null;

            scoreStatisticName = string.Empty;
            scoreStatisticVer = null;

            myStatistics = null;
            myStatisticUpdate = null;
        }

        static GameManager() {
        }

        #endregion

        #region Unity User Callback Event Funcs

        private void Awake() {
            myStatisticUpdate = new StatisticUpdate();

            myStatistics = new List<StatisticUpdate> {
                myStatisticUpdate
            };
        }

        private void OnEnable() {
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest() { //Lame
                    StatisticNames = new List<string> { //Lame
                        scoreStatisticName
                    }
                },
                OnGetPlayerStatisticsSuccess,
                OnGetPlayerStatisticsFailure
            );
        }

        private void Update() { //Lame
            scoreTmp.text = frontText + score;
        }

        private void OnDisable() {
            UpdateScoreStatistic();
        }

        #endregion

        private void UpdateScoreStatistic() {
            myStatisticUpdate.StatisticName = scoreStatisticName;
            myStatisticUpdate.Value = score;
            myStatisticUpdate.Version = scoreStatisticVer;

            PlayFabClientAPI.UpdatePlayerStatistics(
                new UpdatePlayerStatisticsRequest() {
                    Statistics = myStatistics
                },
                OnUpdatePlayerStatisticsSuccess,
                OnUpdatePlayerStatisticsFailure
            );
        }

        private void OnGetPlayerStatisticsSuccess(GetPlayerStatisticsResult result) {
            Console.Log("GetPlayerStatisticsSuccess!");

            if(result.Statistics.Any()) {
                score = result.Statistics[0].Value;
                scoreStatisticVer = result.Statistics[0].Version;
            } else {
                score = 0;
                scoreStatisticVer = null;
            }
        }

        private void OnGetPlayerStatisticsFailure(PlayFabError _) {
            Console.LogError("GetPlayerStatisticsFailure!");
        }

        private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult _) {
            Console.Log("UpdatePlayerStatisticsSuccess!");
        }

        private void OnUpdatePlayerStatisticsFailure(PlayFabError _) {
            Console.LogError("UpdatePlayerStatisticsFailure!");
        }

        public void AddToScore(int amt) {
            score += amt;
        }

        public void ResetScore() {
            score = 0;
        }
    }
}