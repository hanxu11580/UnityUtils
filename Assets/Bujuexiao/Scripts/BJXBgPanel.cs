using Feif.UIFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using USDT.Utils;
using USDT.Core.Table;
using System;
using USDT.Core;
using System.IO;

namespace Bujuexiao {
    public class BgPanelUIData : UIData {
        public AppData_Save appData;
    }

    public enum BJXRoomStatus {
        None = 0, // 闲置
        Working,  // 工作
        Disable, // 禁用
    }

    public class BJXRoomData {
        public BJXRoom roomUnityInstance;
        public BJXRoomStatus status;
        public Sprite statusSprite;
        public Action roomButtonAction;

        public string workEmployeeName;
        public List<int> projcets = new List<int>();
        public DateTime startTime;
        public DateTime endTime;
    }

    [UIPanel]
    public class BJXBgPanel : UIComponent<BgPanelUIData> {

        [SerializeField] Button _openSettingsButton;
        [SerializeField] List<BJXRoom> _rooms = new List<BJXRoom>();
        [Header("房间状态图")]
        [SerializeField] Sprite _noneRoomStatusSprite;
        [SerializeField] Sprite _workingRoomStatusSprite;
        [SerializeField] Sprite _disableRoomStatusSprite;

        private Dictionary<int, BJXRoomData> _roomsDataLut = new Dictionary<int, BJXRoomData>();

        protected override Task OnRefresh() {
            for (int i = 0; i < _rooms.Count; i++) {
                var tmepIndex = i;
                var openRoom = i < this.Data.appData.openRoomCount;
                if (!_roomsDataLut.ContainsKey(i)) {
                    BJXRoomData newRoomData = new BJXRoomData();
                    newRoomData.roomUnityInstance = _rooms[i];
                    _roomsDataLut.Add(i, newRoomData);
                }
                var bujuexiaoRoomData = _roomsDataLut[i];
                if (openRoom) {
                    bujuexiaoRoomData.status = BJXRoomStatus.None;
                    bujuexiaoRoomData.statusSprite = _noneRoomStatusSprite;
                    bujuexiaoRoomData.roomButtonAction = () => ClickRoom(tmepIndex);
                }
                else {
                    bujuexiaoRoomData.status = BJXRoomStatus.Disable;
                    bujuexiaoRoomData.statusSprite = _disableRoomStatusSprite;
                }

                _rooms[i].SetRoomData(_roomsDataLut[i]);
            }
            return Task.CompletedTask;
        }

        protected override void OnBind() {
            _openSettingsButton.onClick.AddListener(() => {
                var settingsData = new SettingsUIData() {
                    openRoomCount = this.Data.appData.openRoomCount,
                    employees = this.Data.appData.employees,
                    selectWorkSaveFolderPath = this.Data.appData.selectWorkSaveFolderPath,
                };
                UIFrame.Show<BJXSettings>(settingsData);
            });

            BJXLauncher.EventManager.Subscribe<ChangeRoomStatusEventArgs>(OnChangeRoomStatusEvent);
        }

        protected override void OnUnbind() {
            _openSettingsButton.onClick.RemoveAllListeners();
            BJXLauncher.EventManager.Unsubscribe<ChangeRoomStatusEventArgs>(OnChangeRoomStatusEvent);
        }

        private void ClickRoom(int index) {
            // 确保设置过工作数据存储路径
            if (string.IsNullOrEmpty(this.Data.appData.selectWorkSaveFolderPath)) {
                lg.e("请先设置工作数据存储路径!", true);
                return;
            }

            BJXSetRoomWorkUIData bJXWorkRoom = new BJXSetRoomWorkUIData() {
                employees = this.Data.appData.employees,
                roomIndex = index,
                status = _roomsDataLut[index].status,
                workEmployeeName = _roomsDataLut[index].workEmployeeName,
            };
            UIFrame.Show<BJXSetRoomWork>(bJXWorkRoom);
        }

        private void OnChangeRoomStatusEvent(object sender, GlobalEventArgs e) {
            var data = e as ChangeRoomStatusEventArgs;
            if (data == null) {
                lg.e($"设置上钟 Event数据为空", true);
                return;
            }

            var roomData = _roomsDataLut[data.roomIndex];
            if(roomData.status == BJXRoomStatus.None) {
                // 只有第一次才设置开始时间
                roomData.startTime = data.startWorkTime;
                roomData.endTime = roomData.startTime;
            }
            // 设置技师忙碌状态
            roomData.workEmployeeName = data.selectEmployeeName;
            this.Data.appData.SetEmployeeBusy(data.selectEmployeeName, true);
            roomData.projcets.Add(data.selectProjectId);
            // 所有项目时间加起来
            foreach (var projcetId in roomData.projcets) {
                var projectTableItem = ProjectLutCategory.Instance.GetConfig(projcetId);
                roomData.endTime = roomData.endTime.AddMinutes(projectTableItem.duration);
            }
            roomData.status = BJXRoomStatus.Working;
            roomData.statusSprite = _workingRoomStatusSprite;
            roomData.roomUnityInstance.SetRoomData(roomData);
        }

        private void FixedUpdate() {
            foreach (var roomKV in _roomsDataLut) {
                if(roomKV.Value.status == BJXRoomStatus.Working) {
                    var remainTimeSpan = roomKV.Value.endTime - DateTime.Now;
                    if(remainTimeSpan.TotalSeconds <= 0) {
                        // 结束
                        foreach (var projcetId in roomKV.Value.projcets) {
                            var projectTableItem = ProjectLutCategory.Instance.GetConfig(projcetId);
                            SaveEmployeeWorkData(roomKV.Value.endTime, roomKV.Value.workEmployeeName, projectTableItem.name);
                        }
                        this.Data.appData.SetEmployeeBusy(roomKV.Value.workEmployeeName, false);
                        roomKV.Value.status = BJXRoomStatus.None;
                        roomKV.Value.statusSprite = _noneRoomStatusSprite;
                        roomKV.Value.projcets.Clear();
                        roomKV.Value.roomUnityInstance.SetRoomData(roomKV.Value);
                    }
                    else {
                        // 更新时间
                        roomKV.Value.roomUnityInstance.UpdateRmainTime(remainTimeSpan);
                    }
                }
            }
        }

        private void SaveEmployeeWorkData(DateTime endTime, string employeeName, string projectName) {
            var saveFolder = this.Data.appData.selectWorkSaveFolderPath;
            var yyyymmdd = TimeUtils.ToYYYY_MM_DD(DateTime.Now);
            var savePath = Path.Combine(saveFolder, yyyymmdd);
            //if (!File.Exists(savePath)) {
            //    File.AppendAllText(savePath, "abc\n");
            //}

            var append = $"{endTime},{employeeName},{projectName}\n";
            File.AppendAllText(savePath, append);
        }
    }
}