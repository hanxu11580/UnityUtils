using PureMVC.Patterns.Proxy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PureMVC.MyTest {

    public class DataProxy : Proxy {
        public new const string NAME = "DataProxy";

        public DataProxy(ScoreData data = null) : base(NAME, data) {
        }

        public void AddScore(int addScore) {
            var scoreData = this.Data as ScoreData;
            scoreData.Score += addScore;
            SendNotification(PureMVCStart.Msg_AddScore, scoreData);
        }
    }
}