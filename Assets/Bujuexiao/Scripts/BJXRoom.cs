using Feif.UIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using USDT.Utils;

namespace Bujuexiao {

    public class BJXRoom : UIBase {
        [SerializeField] Button _thisButton;
        // ״̬
        [SerializeField] Image _statusImg;

        [SerializeField] CanvasGroup _workMask;
        // 工作名字
        [SerializeField] Text _workingName;
        // 剩余时间
        [SerializeField] Text _remainTime;

        private BJXRoomStatus _thisStatus;

        public void Update() {
            if(_thisStatus == BJXRoomStatus.Working) {
                _statusImg.transform.Rotate(0, 0, -500 * Time.deltaTime);
            }
        }

        protected override Task OnRefresh() {
            return Task.CompletedTask;
        }

        protected override void OnBind() {
            
        }

        protected override void OnUnbind() {

        }

        public void SetRoomData(BJXRoomData data) {
            _statusImg.transform.rotation = Quaternion.identity;
            _thisStatus = data.status;
            _statusImg.sprite = data.statusSprite;
            _thisButton.onClick.RemoveAllListeners();
            _thisButton.onClick.AddListener(() => { data.roomButtonAction?.Invoke(); });
            if(data.status == BJXRoomStatus.Working) {
                _workMask.alpha = 1;
                _workingName.text = data.workEmployeeName;
                var remainTimeSpan = (data.endTime - DateTime.Now);
                UpdateRmainTime(remainTimeSpan);
            }
            else {
                _workMask.alpha = 0;
            }
        }


        public void UpdateRmainTime(TimeSpan timeSpan) {
            _remainTime.text = $"剩余{timeSpan:hh\\:mm\\:ss}";
        }
    }
}