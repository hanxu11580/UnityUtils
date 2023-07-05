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
    /// �ı�Ա��״̬�¼�
    /// </summary>
    public class ChangeRoomStatusEventArgs : GlobalEventArgs {
        public int roomIndex;
        // ѡ��ļ�ʦ
        public string selectEmployeeName;
        // ѡ�����Ŀid
        public int selectProjectId;
        // ��ʼ�¼�
        public DateTime startWorkTime;
    }


    [UIWindow]
    public class BJXSetRoomWork : UIComponent<BJXSetRoomWorkUIData> {

        [SerializeField] Text _workStatusText;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _workButton; // ����
        [SerializeField] Button _addWorkButton; // ����
        [SerializeField] Dropdown _employeeDropdown;
        [SerializeField] Dropdown _projectDropdown;

        // ��Ա������
        private bool _haveEmployeeUse;

        protected override Task OnCreate() {
            return Task.CompletedTask;
        }

        protected override Task OnRefresh() {

            // Ա��ö�����
            if (_employeeDropdown.options == null) {
                _employeeDropdown.options = new List<Dropdown.OptionData>();
            }
            else {
                _employeeDropdown.options.Clear();
            }
            // ��Ŀö�����
            if (_projectDropdown.options == null) {
                _projectDropdown.options = new List<Dropdown.OptionData>();
            }
            else {
                _projectDropdown.options.Clear();
            }


            // ����ʹ��״̬
            if (this.Data.status == BJXRoomStatus.None) {
                _workStatusText.text = "����";
                _addWorkButton.interactable = false;
                _employeeDropdown.interactable = true;
                _workButton.interactable = true;

                // ����Ա��ö��
                _employeeDropdown.value = 0;
                if (this.Data.employees != null && this.Data.employees.Count != 0) {
                    foreach (var employee in this.Data.employees) {
                        // �����ʦ����ˣ�Ȼ��æµ״̬���޷�ѡ��
                        if (employee.status == BJXEmployeeStatus.AskForLeave || employee.GetIsBusy()) {
                            continue;
                        }
                        _employeeDropdown.options.Add(new Dropdown.OptionData() { text = employee.name });
                        _haveEmployeeUse = true;
                    }
                }
            }
            else {
                _workStatusText.text = "������";
                _employeeDropdown.captionText.text = $"{this.Data.workEmployeeName} ������";
                _addWorkButton.interactable = true;
                _employeeDropdown.enabled = false;
                _workButton.interactable = false;
            }

            // ������Ŀö��
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
                LogUtils.LogError($"���޿��м�ʦ!", true);
                return false;
            }
        }
    }
}