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
        // ���ŷ�������
        public int openRoomCount;
        // Ա���б�
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

        // ��������һֱ���ڣ������ڹرգ�����ֱ�ӷų�Ա��������
        private BgPanelUIData _bujuexiaoBgPanelData;
        private AppData_Save _bujuexiaoAppData;

        public static string BujuexiaoAppDataSavePath;
        public static EventManager EventManager { get; private set; }

        // ʹ��UIFrameʱҪ��ȷ��UIFrame��Awake�Ѿ�ִ�й���
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

            // �¼�
            EventManager = GameEntry.GetManager<EventManager>();
            EventManager.Subscribe<SaveAppDataEventArgs>(OnSaveAppDataEvent);
            EventManager.Subscribe<ClearAppDataEventArgs>(OnClearAppDataEvent);
            EventManager.Subscribe<DeleteEmployeeEventArgs>(OnDeleteEmployeeEvent);
            EventManager.Subscribe<AddEmployeeEventArgs>(OnAddEmployeeEvent);

            // ע����Դ�����ͷ��¼�
            UIFrame.OnAssetRequest += OnAssetRequest;
            UIFrame.OnAssetRelease += OnAssetRelease;
            // ע��UI��ס�¼�
            // ����ʱ�䳬��0.5s�󴥷�UI��ס�¼�
            //UIFrame.StuckTime = 0.5f;
            //UIFrame.OnStuckStart += OnStuckStart;
            //UIFrame.OnStuckEnd += OnStuckEnd;

            // ��������
            BujuexiaoAppDataSavePath = Path.Combine(Application.persistentDataPath, "BJX.json");
            _bujuexiaoAppData = UniversalUtils.LitJsonToObject<AppData_Save>(BujuexiaoAppDataSavePath);
            if(_bujuexiaoAppData == null) {
                _bujuexiaoAppData = new AppData_Save();
            }
        }

        // ��Դ�����¼���typeΪUI�ű�������
        // ����ʹ��Addressables��YooAssets�ȵ�������Դ����ϵͳ
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

        // ��Դ�ͷ��¼�
        private void OnAssetRelease(Type type) {
            // TODO
        }

        private void OnApplicationQuit() {
            if(_bujuexiaoAppData != null) {
                UniversalUtils.LitJsonToJson(BujuexiaoAppDataSavePath, _bujuexiaoAppData);
                lg.i($"App�رգ��洢����");
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
                lg.e($"�洢appData Event����Ϊ��", true);
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
                lg.e($"���appData Event����Ϊ��", true);
                return;
            }
            PopupErrorTips("����������ڸ�Σ�������Ƿ�ȷ�����?", ()=> ClearAppData(clearData.hideUI), null);
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
                lg.e($"ɾ��Ա�� Event����Ϊ��", true);
                return;
            }
            if (data.deleteEmployee == null) {
                lg.e($"ɾ��Ա�� Employee����Ϊ��", true);
                return;
            }
            if(_bujuexiaoAppData.employees == null || _bujuexiaoAppData.employees.Count  == 0) {
                lg.e($"����Ա���б�employees����Ϊ��", true);
                return;
            }
            if (!_bujuexiaoAppData.employees.Contains(data.deleteEmployee)) {
                lg.e($"ɾ��Ա��:{data.deleteEmployee.name}������Ա���б���", true);
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
                lg.e($"���Ա�� Event����Ϊ��", true);
                return;
            }
            if (_bujuexiaoAppData.employees == null) {
                _bujuexiaoAppData.employees = new List<Employee_Save>();
            }
            // ����
            foreach (var emp in _bujuexiaoAppData.employees) {
                if(emp.name == data.name) {
                    lg.e($"���Ա��{emp.name}�Ѵ���", true);
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