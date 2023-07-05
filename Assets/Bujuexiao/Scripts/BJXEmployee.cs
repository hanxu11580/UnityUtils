using Feif.UIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using USDT.Core;

namespace Bujuexiao {

    public class EmployeeUIData : UIData {
        public string name;
        public BJXEmployeeStatus status;
        public Action deleteButtonCallback;
    }

    public enum BJXEmployeeStatus {
        Working,  // 工作
        AskForLeave, // 请假
    }

    [SerializeField]
    public class Employee_Save {
        public string name;
        public BJXEmployeeStatus status;

        private bool _isBusy;
        public bool GetIsBusy() {
            return _isBusy;
        }
        public void SetIsBusy(bool isBusy) {
            _isBusy = isBusy;
        }

        /// <summary>
        /// 根据状态排序
        /// </summary>
        public static int SortEmployees(Employee_Save l, Employee_Save r) {
            if (l == null) return 1;
            if (r == null) return -1;
            var intLstatus = (int)l.status;
            var intRstatus = (int)r.status;
            if (intLstatus < intRstatus) {
                return -1;
            }
            else if (intLstatus > intRstatus) {
                return 1;
            }
            else {
                // 相同
                var lNameLen = l.name.Length;
                var rNameLen = r.name.Length;
                if (lNameLen < rNameLen) {
                    return -1;
                }
                else if (lNameLen > rNameLen) {
                    return 1;
                }
                return 0;
            }
        }
    }


    /// <summary>
    /// 改变员工状态事件
    /// </summary>
    public class ChangeEmployeeStatusEventArgs : GlobalEventArgs {
        public string name;
        public BJXEmployeeStatus status;
    }

    public class BJXEmployee : UIComponent<EmployeeUIData> {

        [SerializeField] Text _name;
        [SerializeField] Dropdown _statusDropdown;
        [SerializeField] Button _deleteButton;
        [SerializeField] Image _bgImage;

        [SerializeField] Sprite _workSprite;
        [SerializeField] Sprite _askForLeaveprite;


        protected override Task OnCreate() {
            return base.OnCreate();
        }

        protected override void OnBind() {
            base.OnBind();
            _statusDropdown.onValueChanged.AddListener(OnStatusDropdownChanged);
        }

        protected override void OnUnbind() {
            base.OnUnbind();
            _statusDropdown.onValueChanged.RemoveAllListeners();
        }

        protected override Task OnRefresh() {
            _name.text = this.Data.name;
            if(_statusDropdown.options == null) {
                _statusDropdown.options = new List<Dropdown.OptionData>();
            }
            else {
                _statusDropdown.options.Clear();
            }
            // 和BJXEmployeeStatus枚举一样
            _statusDropdown.options.Add(new Dropdown.OptionData() { text = "正常" });
            _statusDropdown.options.Add(new Dropdown.OptionData() { text = "请假" });
            _statusDropdown.value = (int)this.Data.status;
            switch (this.Data.status) {
                case BJXEmployeeStatus.Working: {
                        _bgImage.sprite = _workSprite;
                        break;
                    }
                case BJXEmployeeStatus.AskForLeave: {
                        _bgImage.sprite = _askForLeaveprite;
                        break;
                    }
            }
            _deleteButton.onClick.RemoveAllListeners();
            _deleteButton.onClick.AddListener(() => { this.Data.deleteButtonCallback?.Invoke(); });
            return base.OnRefresh();
        }

        private void OnStatusDropdownChanged(int index) {
            // 相同不改变
            if (index == (int)this.Data.status) {
                return;
            }

            switch (index) {
                case (int)BJXEmployeeStatus.Working: {
                        this.Data.status = BJXEmployeeStatus.Working;
                        _bgImage.sprite = _workSprite;
                        break;
                    }
                case (int)BJXEmployeeStatus.AskForLeave: {
                        this.Data.status = BJXEmployeeStatus.AskForLeave;
                        _bgImage.sprite = _askForLeaveprite;
                        break;
                    }
            }

            var args = ReferencePool.Acquire<ChangeEmployeeStatusEventArgs>();
            args.name = this.Data.name;
            args.status = this.Data.status;
            BJXLauncher.EventManager.Throw<ChangeEmployeeStatusEventArgs>(null, args);
        }
    }
}