using Feif.UIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using USDT.Core;
using USDT.Core.Table;
using USDT.Utils;

namespace Bujuexiao {

    public class BJXSetRoomWorkUIData : UIData {
        public List<Employee_Save> employees;
        public int roomIndex;
        public BJXRoomStatus status;
        public string workEmployeeName;
    }

    /// <summary>
    /// 改变员工状态事件
    /// </summary>
    public class ChangeRoomStatusEventArgs : GlobalEventArgs {
        public int roomIndex;
        // 选择的技师
        public string selectEmployeeName;
        // 选择的项目id
        public int selectProjectId;
        // 开始事件
        public DateTime startWorkTime;
    }


    [UIWindow]
    public class BJXSetRoomWork : UIComponent<BJXSetRoomWorkUIData> {

        [SerializeField] Text _workStatusText;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _workButton; // 工作
        [SerializeField] Button _addWorkButton; // 加钟
        [SerializeField] Dropdown _employeeDropdown;
        [SerializeField] Dropdown _projectDropdown;

        // 有员工可用
        private bool _haveEmployeeUse;

        protected override Task OnCreate() {
            return Task.CompletedTask;
        }

        protected override Task OnRefresh() {

            // 员工枚举清空
            if (_employeeDropdown.options == null) {
                _employeeDropdown.options = new List<Dropdown.OptionData>();
            }
            else {
                _employeeDropdown.options.Clear();
            }
            // 项目枚举清空
            if (_projectDropdown.options == null) {
                _projectDropdown.options = new List<Dropdown.OptionData>();
            }
            else {
                _projectDropdown.options.Clear();
            }


            // 房间使用状态
            if (this.Data.status == BJXRoomStatus.None) {
                _workStatusText.text = "空闲";
                _addWorkButton.interactable = false;
                _employeeDropdown.interactable = true;
                _workButton.interactable = true;

                // 设置员工枚举
                _employeeDropdown.value = 0;
                if (this.Data.employees != null && this.Data.employees.Count != 0) {
                    foreach (var employee in this.Data.employees) {
                        // 如果技师请假了，然后忙碌状态将无法选择
                        if (employee.status == BJXEmployeeStatus.AskForLeave || employee.GetIsBusy()) {
                            continue;
                        }
                        _employeeDropdown.options.Add(new Dropdown.OptionData() { text = employee.name });
                        _haveEmployeeUse = true;
                    }
                }
            }
            else {
                _workStatusText.text = "上钟中";
                _employeeDropdown.captionText.text = $"{this.Data.workEmployeeName} 上钟中";
                _addWorkButton.interactable = true;
                _employeeDropdown.enabled = false;
                _workButton.interactable = false;
            }

            // 设置项目枚举
            _projectDropdown.value = 0;
            var lut = ProjectLutCategory.Instance.GetConfigs();
            foreach (var kv in lut) {
                _projectDropdown.options.Add(new Dropdown.OptionData() { text = kv.Value.name });
            }

            return base.OnRefresh();
        }

        protected override void OnBind() {
            _closeButton.onClick.AddListener(OnCloseSettings);
            _workButton.onClick.AddListener(OnClickWork);
            _addWorkButton.onClick.AddListener(OnClickAddWork);
        }

        protected override void OnUnbind() {
            _closeButton.onClick.RemoveAllListeners();
            _workButton.onClick.RemoveAllListeners();
            _addWorkButton.onClick.RemoveAllListeners();
        }

        protected override void OnShow() {
        }

        private void OnCloseSettings() {
            UIFrame.Hide(this);
        }

        private void OnClickWork() {
            if (!CheckHaveEmployeeUse()) {
                return;
            }
            var args = ReferencePool.Acquire<ChangeRoomStatusEventArgs>();
            args.selectEmployeeName = _employeeDropdown.captionText.text;
            foreach (var projectItem in ProjectLutCategory.Instance.GetConfigs().Values) {
                if(projectItem.name == _projectDropdown.captionText.text) {
                    args.selectProjectId = projectItem.Id;
                }
            }
            args.startWorkTime = DateTime.Now;
            args.roomIndex = this.Data.roomIndex;
            BJXLauncher.EventManager.Throw<ChangeRoomStatusEventArgs>(null, args);

            UIFrame.Hide(this);
        }

        private void OnClickAddWork() {
            if (!CheckHaveEmployeeUse()) {
                return;
            }
            var args = ReferencePool.Acquire<ChangeRoomStatusEventArgs>();
            args.selectEmployeeName = this.Data.workEmployeeName;
            foreach (var projectItem in ProjectLutCategory.Instance.GetConfigs().Values) {
                if (projectItem.name == _projectDropdown.captionText.text) {
                    args.selectProjectId = projectItem.Id;
                }
            }
            args.startWorkTime = DateTime.Now;
            args.roomIndex = this.Data.roomIndex;
            BJXLauncher.EventManager.Throw<ChangeRoomStatusEventArgs>(null, args);

            UIFrame.Hide(this);
        }

        private bool CheckHaveEmployeeUse() {
            if (_haveEmployeeUse) {
                return true;
            }
            else {
                LogUtils.LogError($"暂无空闲技师!", true);
                return false;
            }
        }
    }
}