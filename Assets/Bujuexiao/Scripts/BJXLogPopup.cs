using Feif.UIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Bujuexiao {

    public class LogPopupUIData : UIData {
        public string content;
        public Action clickEnsureCallback;
        public Action clickCloseCallback;
    }

    [UIWindow]
    public class BJXLogPopup : UIComponent<LogPopupUIData> {

        [SerializeField] Button _ensureButton;
        [SerializeField] Button _closeButton;
        [SerializeField] Text _contentText;

        protected override Task OnCreate() {
            return Task.CompletedTask;
        }

        protected override Task OnRefresh() {
            _contentText.text = this.Data.content;
            return base.OnRefresh();
        }

        protected override void OnBind() {
            _ensureButton.onClick.AddListener(OnClickEnsure);
            _closeButton.onClick.AddListener(OnClickClose);
        }

        protected override void OnUnbind() {
            _ensureButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        protected override void OnShow() {
        }

        private void OnClickEnsure() {
            this.Data.clickEnsureCallback?.Invoke();
            UIFrame.Hide(this);
        }

        private void OnClickClose() {
            this.Data.clickCloseCallback?.Invoke();
            UIFrame.Hide(this);
        }
    }
}