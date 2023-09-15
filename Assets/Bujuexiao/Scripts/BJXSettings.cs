using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Feif.UIFramework;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using USDT.Core;
using USDT.Utils;
using UnityEngine.AddressableAssets;


namespace Bujuexiao {

    public class SettingsUIData :UIData{
        public int openRoomCount;
        public List<Employee_Save> employees = new List<Employee_Save>();
        public string selectWorkSaveFolderPath;
    }

    /// <summary>
    /// 存储appdata事件
    /// </summary>
    public class SaveAppDataEventArgs : GlobalEventArgs {
        public int openRoomCount;
        public string selectWorkSaveFolderPath;
    }

    /// <summary>
    /// 清空数据事件
    /// </summary>
    public class ClearAppDataEventArgs : GlobalEventArgs {
        public UIBase hideUI;
    }

    /// <summary>
    /// 删除员工事件
    /// </summary>
    public class DeleteEmployeeEventArgs : GlobalEventArgs {
        public Employee_Save deleteEmployee;
    }

    [UIWindow]
    public class BJXSettings :  UIComponent<SettingsUIData> {
        [SerializeField] Button _closeButton;
        [SerializeField] InputField _roomCountInputField;
        [SerializeField] Button _addEmployeeButton;
        [SerializeField] Button _saveButton;
        [SerializeField] Button _clearButton;
        [SerializeField] RectTransform _employesParent;
        [SerializeField] string _employeePrefabPath;
        // 选择数据存储路径
        [SerializeField] Button _selectWorkSaveFolderButton;
        // 显示当前数据存储路径
        [SerializeField] Text _selectWorkSaveFolderPath;

        private GameObject _employeePrefab;
        private List<BJXEmployee> _bJXEmployees;

        protected async override Task OnCreate() {
            var opt = Addressables.LoadAssetAsync<GameObject>(_employeePrefabPath);
            await opt.Task;
            _employeePrefab = opt.Task.Result;
        }

        protected override void OnBind() {
            _closeButton.onClick.AddListener(OnCloseSettings);
            _saveButton.onClick.AddListener(OnClickSave);
            _clearButton.onClick.AddListener(OnClickClear);
            _addEmployeeButton.onClick.AddListener(OnClickAddEmployee);
            _selectWorkSaveFolderButton.onClick.AddListener(OnSelectWorkSaveFolder);
            _roomCountInputField.onValueChanged.AddListener(OnRoomCountInputFieldChanged);
            BJXLauncher.EventManager.Subscribe<ChangeEmployeeStatusEventArgs>(OnChangeEmployeeInfo);
        }

        protected override void OnUnbind() {
            _closeButton.onClick.RemoveAllListeners();
            _saveButton.onClick.RemoveAllListeners();
            _clearButton.onClick.RemoveAllListeners();
            _addEmployeeButton.onClick.RemoveAllListeners();
            _roomCountInputField.onValueChanged.RemoveAllListeners();
            _selectWorkSaveFolderButton.onClick.RemoveAllListeners();
            BJXLauncher.EventManager.Unsubscribe<ChangeEmployeeStatusEventArgs>(OnChangeEmployeeInfo);
        }

        protected async override Task OnRefresh() {
            _roomCountInputField.text = this.Data.openRoomCount.ToString();
            _selectWorkSaveFolderPath.text = this.Data.selectWorkSaveFolderPath;
            if (_bJXEmployees == null) {
                _bJXEmployees = new List<BJXEmployee>(this.Data.employees.Count);
            }

            var haveData = this.Data.employees != null && this.Data.employees.Count != 0;

            // 初始化清空列表
            if (haveData) {
                this.Data.employees.Sort(Employee_Save.SortEmployees);
                var tasks = new List<Task<UIBase>>();
                for (int i = 0; i < this.Data.employees.Count; i++) {
                    var employeeData = this.Data.employees[i];
                    var data = new EmployeeUIData() {
                        name = employeeData.name,
                        status = employeeData.status,
                        deleteButtonCallback = () => { OnClickDeleteEmployee(employeeData); },
                    };
                    if (i < _bJXEmployees.Count) {
                        var getBjxEmployee = _bJXEmployees[i];
                        Task<UIBase> tUIbase = UIFrame.UnCreateInstantiateAsync(getBjxEmployee, _employesParent, data);
                        await tUIbase;
                    }
                    else {
                        // _bJXEmployees 不足需要创建，创建的需要加入
                        // 实例化UI元素
                        tasks.Add(UIFrame.Instantiate(_employeePrefab, _employesParent, data));
                    }
                }

                await Task.WhenAll(tasks);

                foreach (var task in tasks) {
                    _bJXEmployees.Add(task.Result.GetComponent<BJXEmployee>());
                }
            }

            // 用不到的gameObject全部清除掉
            var dataCount = haveData ? this.Data.employees.Count : 0;
            while (dataCount < _bJXEmployees.Count) {
                // 不能使用UIFrame.Destroy，会死循环
                UIFrame.DestroyImmediate(_bJXEmployees[dataCount].gameObject);
                _bJXEmployees.RemoveAt(dataCount);
            }
        }

        protected override void OnShow() {
        }

        private void OnCloseSettings() {
            UIFrame.Hide(this);
        }

        private void OnRoomCountInputFieldChanged(string arg0) {
            if (int.TryParse(arg0, out int parseInt)) {
                this.Data.openRoomCount = parseInt;
            }
        }

        private void OnClickSave() {
            UIFrame.Hide(this);
            var args = ReferencePool.Acquire<SaveAppDataEventArgs>();
            args.openRoomCount = this.Data.openRoomCount;
            args.selectWorkSaveFolderPath = _selectWorkSaveFolderPath.text;
            BJXLauncher.EventManager.Throw<SaveAppDataEventArgs>(null, args);
        }

        private void OnClickClear() {
            var args = ReferencePool.Acquire<ClearAppDataEventArgs>();
            args.hideUI = this;
            BJXLauncher.EventManager.Throw<ClearAppDataEventArgs>(null, args);
        }

        private void OnSelectWorkSaveFolder() {
            var path = FolderBrowserUtils.GetPathFromWindowsExplorer();
            if (!string.IsNullOrEmpty(path)) {
                this.Data.selectWorkSaveFolderPath = path;
                _selectWorkSaveFolderPath.text = path;
            }
        }

        private void OnClickDeleteEmployee(Employee_Save deleteEmployee) {
            var args = ReferencePool.Acquire<DeleteEmployeeEventArgs>();
            args.deleteEmployee = deleteEmployee;
            BJXLauncher.EventManager.Throw<DeleteEmployeeEventArgs>(null, args);
        }

        private void OnClickAddEmployee() {
            UIFrame.Show<BJXAddEmployee>(new AddEmployeeUIData());
        }

        private void OnChangeEmployeeInfo(object sender, GlobalEventArgs e) {
            var data = e as ChangeEmployeeStatusEventArgs;
            if (data == null) {
                lg.e($"改变员工状态 Event数据为空", true);
                return;
            }
            foreach (var emp in this.Data.employees) {
                if (emp.name == data.name) {
                    emp.status = data.status;
                    break;
                }
            }

            UIFrame.Refresh(this, this.Data);
        }
    }
}