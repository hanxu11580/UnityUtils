using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utility.Editor{
    public class DataTableEditingWindow : EditorWindow
    {
        public Action<string[][]> OnSaveData;

        public List<DataTableRowData> RowDatas { get; private set; }

        string TitleStyle = "flow node hex 2";

        string ElementStyle = "ScriptText";

        List<DataTableRowData> RowDatasTemp;

        ReorderableList reorderableList;

        Vector2 _scrollViewPos;

        bool _draggable;
        bool _displayAddButton;
        bool _displayRemoveButton;

        [MenuItem("Tools/打开测试表格窗口")]
        public static void TestOpenTableWindow() {
            var titles = new string[] {
                "t1","t2","t3","t4",
            };

            var data = new string[][] {
                new string[]{ "1","2","3","4"},
                new string[]{ "1","2","3","4"},
                new string[]{ "1","2","3","4"},
                new string[]{ "1","2","3","4"},
                new string[]{ "1","2","3","4"},
            };
            Show(titles,data, true);
        }

        public static DataTableEditingWindow Show(string[] titles, string[][] sourceData, bool draggable = true, bool displayAddButton = true, bool displayRemoveButton = true) {
            var wnd = CreateInstance<DataTableEditingWindow>();
            wnd.Init(titles, sourceData, draggable, displayAddButton, displayRemoveButton);
            wnd.Show();
            return wnd;
        }

        public void Init(string[] titles, string[][] sourceData, bool draggable = true, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            if (sourceData == null)
                return;

            _draggable = draggable;
            _displayAddButton = displayAddButton;
            _displayRemoveButton = displayRemoveButton;

            RowDatas = new List<DataTableRowData>();
            RowDatasTemp = new List<DataTableRowData>();

            // 添加title
            RowDatas.Add(new DataTableRowData(titles));
            RowDatasTemp.Add(new DataTableRowData(titles));

            // r:行 c:列
            for (int r = 0; r < sourceData.Length; r++)
            {
                DataTableRowData tempData = new DataTableRowData();
                DataTableRowData data = new DataTableRowData();

                for (int c = 0; c < sourceData[r].Length; c++)
                {
                    tempData.Data.Add(sourceData[r][c].ToString());
                    data.Data.Add(sourceData[r][c].ToString());
                }

                RowDatasTemp.Add(tempData);
                RowDatas.Add(data);
            }

            if (RowDatas == null)
                return;
        }

        private void OnGUI()
        {
            _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos);
            if (RowDatas == null || RowDatas.Count == 0)
            {
                Close();
                GUILayout.EndScrollView();
                return;
            }
            
            CheckColumnCount();

            
            if (reorderableList == null)
            {
                reorderableList =
                   new ReorderableList(RowDatas, typeof(List<DataTableRowData>), _draggable, false, _displayAddButton, _displayRemoveButton);

                reorderableList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
                {
                    for (int i = 0; i < RowDatas[index].Data.Count; i++)
                    {
                        if (RowDatas[index].Data.Count > 10)
                        {
                            rect.width =
                                (this.position.width - 20) /
                                10;
                        }
                        else
                        {
                            rect.width =
                                (this.position.width - 20) /
                                RowDatas[index].Data.Count;
                        }
                        
                        rect.x = rect.width * i + 20;
                        RowDatas[index].Data[i] =
                            EditorGUI.TextField(rect, "", RowDatas[index].Data[i],
                                index == 0 ? this.TitleStyle : this.ElementStyle);
                    }
                };

                #region 自带的添加删除按钮
                reorderableList.onAddCallback = list =>
                {
                    DataTableRowData data = new DataTableRowData();
                    for (int i = 0; i < RowDatas[0].Data.Count - 1; i++) {
                        data.Data.Add("");
                    }
                    RowDatas.Add(data);
                };

                reorderableList.onRemoveCallback = list =>
                {
                    RowDatas.RemoveAt(list.index);
                };
                #endregion

                reorderableList.drawHeaderCallback = (Rect rect) => {
                    // rect.width就是总宽度
                    // rect.x = rect.width
                };
            }

            reorderableList.DoLayoutList();
            
            if (RowDatas != null && RowDatas.Count>0)
            {
                if (RowDatas[0].Data.Count > 10)
                {
                    float listItemWidth = 0f;
                    float listX = 0f;
                    listItemWidth = (position.width - 20) / 10;
                    listX = listItemWidth * (RowDatas[0].Data.Count-1) + 20;
                    GUILayout.Label("",new GUIStyle(){fixedWidth = listX});
                }
            }
            GUILayout.EndScrollView();

            // control + s
            if(UnityEditorUtility.KeyCombinationsTick(EventModifiers.Control, KeyCode.S, EventType.KeyDown)) {
                // 没有发生改变 则什么都不做
                if (!CheckDirty())
                    return;

                OnSaveData?.Invoke(RowDatasToArray());
            }

        }

        /// <summary>
        /// 检查列数一致性
        /// </summary>
        void CheckColumnCount()
        {
            if (RowDatas == null || RowDatas.Count == 0)
                return;

            int count = RowDatas[0].Data.Count;

            for (int i = 0; i < RowDatas.Count; i++)
            {
                int need = count - RowDatas[i].Data.Count;

                if (need > 0)
                    for (int j = 0; j < need; j++)
                        RowDatas[i].Data.Add("");
                else if (need < 0)
                    for (int j = 0; j < Mathf.Abs(need); j++)
                        RowDatas[i].Data.RemoveAt(RowDatas[i].Data.Count - 1);
            }
        }

        /// <summary>
        /// 检查表格是否进行更改
        /// </summary>
        /// <returns></returns>
        bool CheckDirty()
        {
            if (RowDatasTemp == null || RowDatas == null)
            {
                return false;
            }

            if (RowDatasTemp.Count != RowDatas.Count)
                return true;

            for (int i = 0; i < RowDatas.Count; i++)
            {
                if (RowDatasTemp[i].Data.Count != RowDatas[i].Data.Count)
                    return true;

                for (int j = 0; j < RowDatas[i].Data.Count; j++)
                {
                    if (RowDatas[i].Data[j] != RowDatasTemp[i].Data[j])
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// RowDatas转交错数组
        /// </summary>
        string[][] RowDatasToArray() {
            var rowCount = RowDatas.Count;
            var res = new string[rowCount][];
            for (int r = 0; r < rowCount; r++) {
                var colCount = RowDatas[r].Data.Count;
                res[r] = new string[colCount];
                for (int c = 0; c < colCount; c++) {
                    res[r][c] = RowDatas[r].Data[c];
                }
            }
            return res;
        }
    }
}
