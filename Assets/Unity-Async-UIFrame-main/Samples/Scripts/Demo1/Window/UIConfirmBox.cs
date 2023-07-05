using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Feif.UIFramework;
using System;

namespace Feif.UI
{
    // 这个UI所需要的参数（数据）
    public class UIConfirmBoxData : UIData
    {
        public string Content;
        public Action ConfirmAction;
        public Action CancelAction;
    }

    // 这是一个Window，不需要UIData则继承UIBase，需要UIData则继承UIComponent
    [UIWindow]
    public class UIConfirmBox : UIComponent<UIConfirmBoxData>
    {
        [SerializeField] private Text txtContent;
        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnCancel;

        private UIWindowEffect effect;

        protected override Task OnCreate()
        {
            effect = GetComponent<UIWindowEffect>();
            return Task.CompletedTask;
        }

        protected override void OnBind()
        {
            btnConfirm.onClick.AddListener(OnBtnConfirm);
            btnCancel.onClick.AddListener(OnBtnCancel);
        }

        protected override void OnUnbind()
        {
            btnConfirm.onClick.RemoveListener(OnBtnConfirm);
            btnCancel.onClick.RemoveListener(OnBtnCancel);
        }

        protected override Task OnRefresh()
        {
            txtContent.text = this.Data.Content;
            return Task.CompletedTask;
        }

        protected override void OnShow()
        {
            // 播放打开Window动效
            effect.PlayOpen();
        }

        private async void OnBtnConfirm()
        {
            this.Data.ConfirmAction?.Invoke();
            // 播放关闭Window动效
            await effect.PlayClose();
            await UIFrame.Hide(this);
        }

        private async void OnBtnCancel()
        {
            this.Data.CancelAction?.Invoke();
            // 播放关闭Window动效
            await effect.PlayClose();
            await UIFrame.Hide(this);
        }
    }
}