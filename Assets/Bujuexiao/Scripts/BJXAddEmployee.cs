using Feif.UIFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using USDT.Core;
using USDT.Utils;

namespace Bujuexiao {

    public class AddEmployeeUIData : UIData {

    }

    /// <summary>
    /// 添加员工事件
    /// </summary>
    public class AddEmployeeEventArgs : GlobalEventArgs {
        public string name;
    }

    [UIWindow]
    public class BJXAddEmployee : UIComponent<AddEmployeeUIData> {
        [SerializeField] Button _closeButton;
        [SerializeField] InputField _employeeNameInputField;
        [SerializeField] Button _addButton;

        private string _inputName;

        protected override Task OnCreate() {
            return Task.CompletedTask;
        }

        protected override void OnBind() {
            _closeButton.onClick.AddListener(OnCloseSettings);
            _addButton.onClick.AddListener(OnClickAdd);
            _employeeNameInputField.onValueChanged.AddListener(OnRoomCountInputFieldChanged);
        }

        protected override void OnUnbind() {
            _closeButton.onClick.RemoveAllListeners();
            _addButton.onClick.RemoveAllListeners();
            _employeeNameInputField.onValueChanged.RemoveAllListeners();
        }

        protected override void OnShow() {
        }

        private void OnCloseSettings() {
            UIFrame.Hide(this);
        }

        private void OnRoomCountInputFieldChanged(string arg0) {
            _inputName = arg0;
        }

        private void OnClickAdd() {
            if (string.IsNullOrEmpty(_inputName)) {
                LogUtils.LogError($"未输入员工名称，请输入", true);
                return;
            }
            var args = ReferencePool.Acquire<AddEmployeeEventArgs>();
            args.name = _inputName;
            BJXLauncher.EventManager.Throw<AddEmployeeEventArgs>(null, args);
            UIFrame.Hide(this);
        }
    }
}