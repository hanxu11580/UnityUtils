using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor {
    /// <summary>
    /// A popup window that displays a list of options and may use a search
    /// </summary>
    public class SearchablePopup : PopupWindowContent {

        #region GUI Styles

        private static GUIStyle SearchBox = "ToolbarSeachTextField";
        private static GUIStyle CancelButton = "ToolbarSeachCancelButton"; //这是一个×
        private static GUIStyle DisabledCancelButton = "ToolbarSeachCancelButtonEmpty";
        private static GUIStyle Selection = "SelectionRect";

        #endregion


        public int Width;

        private const float ROW_HEIGHT = 16.0f;
        private const float ROW_INDENT = 8.0f;
        private const string SEARCH_CONTROL_NAME = "EnumSearchText";

        public static void Show(Rect activatorRect, string[] options, int current, Action<int> onSelectionMade) {
            SearchablePopup win =
                new SearchablePopup(options, current, onSelectionMade);
            PopupWindow.Show(activatorRect, win);
        }

        /// <summary>
        /// 默认鼠标当前位置
        /// </summary>
        public static void Show(string[] options, int current, Action<int> onSelectionMade, int width = 0) {
            SearchablePopup win =
                new SearchablePopup(options, current, onSelectionMade);

            Rect rect = new Rect(
                x: Event.current.mousePosition.x,
                y: Event.current.mousePosition.y+10, 0, 0);
            win.Width = width;
            PopupWindow.Show(rect, win);
        }


        /// <summary>
        /// Force the focused window to redraw. This can be used to make the
        /// popup more responsive to mouse movement.
        /// </summary>
        private static void Repaint() { EditorWindow.focusedWindow.Repaint(); }

        private class FilteredList {
            public struct Entry {
                public int Index;
                public string Text;
            }

            // 所有项
            readonly string[] allItems;
            // 过滤后项
            public List<Entry> Entries { get; private set; }

            public string Filter { get; private set; }


            public int MaxLength
            { get { return allItems.Length; } }

            public FilteredList(string[] items) {
                allItems = items;
                Entries = new List<Entry>();
                UpdateFilter("");
            }

            public bool UpdateFilter(string filter) {
                if (Filter == filter)
                    return false;

                Filter = filter;
                Entries.Clear();

                for (int i = 0; i < allItems.Length; i++) {
                    if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower())) {
                        Entry entry = new Entry {
                            Index = i,
                            Text = allItems[i]
                        };
                        if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                            Entries.Insert(0, entry);
                        else
                            Entries.Add(entry);
                    }
                }
                return true;
            }
        }

        readonly Action<int> onSelectionMade;

        readonly int currentIndex;

        readonly FilteredList list;

        Vector2 scroll;
        // 鼠标悬停Index
        int hoverIndex;

        int scrollToIndex;
        // 滑动到scrollToIndex的偏移量
        float scrollOffset;

        private SearchablePopup(string[] names, int currentIndex, Action<int> onSelectionMade) {
            list = new FilteredList(names);
            this.currentIndex = currentIndex;
            this.onSelectionMade = onSelectionMade;

            hoverIndex = currentIndex;
            scrollToIndex = currentIndex;
            scrollOffset = GetWindowSize().y - ROW_HEIGHT * 2;
        }

        #region life
        public override void OnOpen() {
            base.OnOpen();
            // Force a repaint every frame to be responsive to mouse hover
            EditorApplication.update += Repaint;
        }

        public override void OnClose() {
            base.OnClose();
            EditorApplication.update -= Repaint;
        }

        /// <summary>
        /// 宽固定600， 高动态计算
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetWindowSize() {
            var width = Width != 0 ? Width : base.GetWindowSize().x;
            return new Vector2(width,
                Mathf.Min(600, list.MaxLength * ROW_HEIGHT + EditorStyles.toolbar.fixedHeight));
        }

        public override void OnGUI(Rect rect) {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            // 从search区域下面开始画搜索区域
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }
        #endregion

        void DrawSearch(Rect rect) {
            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(rect, false, false, false, false);

            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= CancelButton.fixedWidth;

            // 将下面的TextField设置名字SEARCH_CONTROL_NAME，并将焦点聚焦到搜索框
            GUI.FocusControl(SEARCH_CONTROL_NAME);
            GUI.SetNextControlName(SEARCH_CONTROL_NAME);
            string newText = GUI.TextField(searchRect, list.Filter, SearchBox);

            if (list.UpdateFilter(newText)) {
                hoverIndex = 0;
                scroll = Vector2.zero;
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = CancelButton.fixedWidth;

            if (string.IsNullOrEmpty(list.Filter))
                GUI.Box(searchRect, GUIContent.none, DisabledCancelButton);
            else if (GUI.Button(searchRect, "x", CancelButton)) {
                list.UpdateFilter("");
                scroll = Vector2.zero;
            }
        }

        void DrawSelectionArea(Rect scrollRect) {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                list.Entries.Count * ROW_HEIGHT);

            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);

            for (int i = 0; i < list.Entries.Count; i++) {
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout)) {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition)) {
                    if (Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;
                    if (Event.current.type == EventType.MouseDown) {
                        if(onSelectionMade != null) {
                            onSelectionMade(list.Entries[i].Index);
                        }
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        static void DrawRectColor(Rect rect, Color tint, string text = "") {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, text, Selection);
            GUI.color = c;
        }


        void DrawRow(Rect rowRect, int i) {
            if (list.Entries[i].Index == currentIndex)
                DrawRectColor(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawRectColor(rowRect, Color.white);

            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += ROW_INDENT;

            GUI.Label(labelRect, list.Entries[i].Text);
        }
        
        void HandleKeyboard() {
            if (Event.current.type == EventType.KeyDown) {
                // 滚轮
                if (Event.current.keyCode == KeyCode.DownArrow) {
                    hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.UpArrow) {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -ROW_HEIGHT;
                }

                if (Event.current.keyCode == KeyCode.Return) {
                    if (hoverIndex >= 0 && hoverIndex < list.Entries.Count) {
                        if(onSelectionMade != null) {
                            onSelectionMade(list.Entries[hoverIndex].Index);
                        }
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape) {
                    EditorWindow.focusedWindow.Close();
                }
            }
        }
    }
}