using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PureMVC.MyTest {
    public class DataMediator : Mediator {
        public new const string NAME = "DataMediator";

        private UIPanelView _view;

        public DataMediator(object viewComponent = null) : base(NAME, viewComponent) {
            _view = viewComponent as UIPanelView;
            _view.scoreText.text = $"分数:0";
            _view.addScoreBtn.onClick.AddListener(() => SendNotification(PureMVCStart.Msg_ClickAddScoreButton));
        }

        public override string[] ListNotificationInterests() {
            string[] arr = new string[1];
            arr[0] = PureMVCStart.Msg_AddScore;
            return arr;
        }

        public override void HandleNotification(INotification notification) {
            switch (notification.Name) {
                case PureMVCStart.Msg_AddScore: {
                        var scoreData = notification.Body as ScoreData;
                        _view.scoreText.text = $"分数:{scoreData.Score}";
                        break;
                    }
            }
        }
    }
}