using Feif.UIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using USDT.Utils;
using USDT.Core;
using USDT.Core.Table;
using UnityEngine.AddressableAssets;

namespace Bujuexiao {

    [System.Serializable]
    public class AppData_Save {
        // 开放房间数量
        public int openRoomCount;
        // 员工列表
        public List<Employee_Save> employees = new List<Employee_Save>();

        public string selectWorkSaveFolderPath;

        public void SetEmployeeBusy(string employeeName, bool isBusy) {
            foreach (var employee in employees) {
                if (employee.name == employeeName) {
                    employee.SetIsBusy(isBusy);
                    break;
                }
            }
        }

        public void Clear() {
            openRoomCount = 0;
            employees.Clear();
            selectWorkSaveFolderPath = null;
        }
    }

    public class BJXLauncher : MonoBehaviour {
        [SerializeField] private GameObject stuckPanel;

        // 这个界面会一直存在，不存在关闭，所以直接放成员变量就行
        private BgPanelUIData _bujuexiaoBgPanelData;
        private AppData_Save _bujuexiaoAppData;

        public static string BujuexiaoAppDataSavePath;
        public static EventManager EventManager { get; private set; }

        // 使用UIFrame时要先确保UIFrame的Awake已经执行过了
        private void Start() {
            Init();
            _bujuexiaoBgPanelData = new BgPanelUIData() {
                appData = _bujuexiaoAppData
            };
            UIFrame.Show<BJXBgPanel>(_bujuexiaoBgPanelData);
        }

        private void Init() {

            // Log
            lg.LogErrorCallback += msg => PopupErrorTips(msg);

            // 事件
            EventManager = GameEntry.GetManager<EventManager>();
            EventManager.Subscribe<SaveAppDataEventArgs>(OnSaveAppDataEvent);
            EventManager.Subscribe<ClearAppDataEventArgs>(OnClearAppDataEvent);
            EventManager.Subscribe<DeleteEmployeeEventArgs>(OnDeleteEmployeeEvent);
            EventManager.Subscribe<AddEmployeeEventArgs>(OnAddEmployeeEvent);

            // 注册资源请求释放事件
            UIFrame.OnAssetRequest += OnAssetRequest;
            UIFrame.OnAssetRelease += OnAssetRelease;
            // 注册UI卡住事件
            // 加载时间超过0.5s后触发UI卡住事件
            //UIFrame.StuckTime = 0.5f;
            //UIFrame.OnStuckStart += OnStuckStart;
            //UIFrame.OnStuckEnd += OnStuckEnd;

            // 加载数据
            BujuexiaoAppDataSavePath = Path.Combine(Application.persistentDataPath, "BJX.json");
            _bujuexiaoAppData = UniversalUtils.LitJsonToObject<AppData_Save>(BujuexiaoAppDataSavePath);
            if(_bujuexiaoAppData == null) {
                _bujuexiaoAppData = new AppData_Save();
            }
        }

        // 资源请求事件，type为UI脚本的类型
        // 可以使用Addressables，YooAssets等第三方资源管理系统
        private Task<GameObject> OnAssetRequest(Type type) {
            string loadPath;
            if (UIFrame.IsPanel(type)) {
                loadPath = $"Assets/Bujuexiao/Bundles/UI/{type.Name}.prefab";
            }
            else {
                loadPath = $"Assets/Bujuexiao/Bundles/Window/{type.Name}.prefab";
            }
            var opt = Addressables.LoadAssetAsync<GameObject>(loadPath);
            return opt.Task;
        }

        // 资源释放事件
        private void OnAssetRelease(Type type) {
            // TODO
        }

        private void OnApplicationQuit() {
            if(_bujuexiaoAppData != null) {
                UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
                lg.i($"App关闭，存储数据");
            }
        }

        private void PopupErrorTips(
            string msg,
            Action clickEnsureCallback = null,
            Action clickCloseCallback = null) {
            UIFrame.Show<BJXLogPopup>(new LogPopupUIData() {
                content = msg,
                clickEnsureCallback = clickEnsureCallback,
                clickCloseCallback = clickCloseCallback,
            });
        }

        private void OnSaveAppDataEvent(object sender, GlobalEventArgs e) {
            var saveData = e as SaveAppDataEventArgs;
            if(saveData == null) {
                lg.e($"存储appData Event数据为空", true);
                return;
            }
            _bujuexiaoAppData.openRoomCount = saveData.openRoomCount;
            _bujuexiaoAppData.selectWorkSaveFolderPath = saveData.selectWorkSaveFolderPath;
            UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
            UIFrame.Refresh<BJXBgPanel>(_bujuexiaoBgPanelData);
        }

        private void OnClearAppDataEvent(object sender, GlobalEventArgs e) {
            var clearData = e as ClearAppDataEventArgs;
            if(clearData == null) {
                lg.e($"清空appData Event数据为空", true);
                return;
            }
            PopupErrorTips("清除数据属于高危操作，是否确定清除?", ()=> ClearAppData(clearData.hideUI), null);
        }

        private void ClearAppData(UIBase hideUI) {
            _bujuexiaoAppData.Clear();
            UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
            UIFrame.Hide(hideUI);
            UIFrame.Refresh<BJXBgPanel>(_bujuexiaoBgPanelData);
        }


        private void OnDeleteEmployeeEvent(object sender, GlobalEventArgs e) {
            var data = e as DeleteEmployeeEventArgs;
            if(data == null) {
                lg.e($"删除员工 Event数据为空", true);
                return;
            }
            if (data.deleteEmployee == null) {
                lg.e($"删除员工 Employee数据为空", true);
                return;
            }
            if(_bujuexiaoAppData.employees == null || _bujuexiaoAppData.employees.Count  == 0) {
                lg.e($"本地员工列表employees数据为空", true);
                return;
            }
            if (!_bujuexiaoAppData.employees.Contains(data.deleteEmployee)) {
                lg.e($"删除员工:{data.deleteEmployee.name}，不在员工列表内", true);
                return;
            }
            _bujuexiaoAppData.employees.Remove(data.deleteEmployee);
            UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
            var settingsData = new SettingsUIData() {
                openRoomCount = _bujuexiaoAppData.openRoomCount,
                employees = _bujuexiaoAppData.employees
            };
            UIFrame.Refresh<BJXSettings>(settingsData);
        }

        private void OnAddEmployeeEvent(object sender, GlobalEventArgs e) {
            var data = e as AddEmployeeEventArgs;
            if (data == null) {
                lg.e($"添加员工 Event数据为空", true);
                return;
            }
            if (_bujuexiaoAppData.employees == null) {
                _bujuexiaoAppData.employees = new List<Employee_Save>();
            }
            // 判重
            foreach (var emp in _bujuexiaoAppData.employees) {
                if(emp.name == data.name) {
                    lg.e($"添加员工{emp.name}已存在", true);
                    break;
                }
            }

            _bujuexiaoAppData.employees.Add(new Employee_Save() {
                name = data.name,
                status = BJXEmployeeStatus.Working,
            });
            UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
            var settingsData = new SettingsUIData() {
                openRoomCount = _bujuexiaoAppData.openRoomCount,
                employees = _bujuexiaoAppData.employees
            };
            UIFrame.Refresh<BJXSettings>(settingsData);
        }
    }
}