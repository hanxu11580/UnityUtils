using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.GenericMenu;

namespace Utility.Editor {

    public static class UnityEditorUtility {
        #region 备忘录

        //改变背景颜色：GUI.backgroundColor

        //改变内容颜色： GUI.contentColor

        //改变内容改变内容和背景颜色： GUI.color

        //GUI的激活状态：GUI.enabled

        // 这玩意只有在重绘事件起作用 平常返回的永远是常量
        // 获取控件矩形 GUILayoutUtility.GetLastRect()

        // TextField逐字Undo
        // 记录操作前，Undo.IncrementCurrentGroup();
        #endregion


        public static GUIStyle Selection = "SelectionRect";
        /// <summary>
        /// 画一个盒子
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="tint"></param>
        public static void DrawRectColor(Rect rect, Color tint, string text = "") {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, text, Selection);
            GUI.color = c;
        }

        /// <summary>
        /// 折叠
        /// </summary>
        /// <param name="label"></param>
        /// <param name="uniqueFlag">唯一的标识</param>
        /// <returns></returns>
        public static bool DrawFoldout(string label, string uniqueFlag) {
            //string key = uniqueFlag.GetHashCode().ToString();
            bool foldout = EditorPrefs.GetBool(uniqueFlag, true);
            bool flag = EditorGUILayout.Foldout(foldout, label);
            if (flag != foldout)
                EditorPrefs.SetBool(uniqueFlag, flag);
            return flag;
        }

        /// <summary>
        /// 画一个菜单
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="callbacks"></param>
        /// <param name="haveSeparator">是否有分隔线</param>
        public static void DrawMenu(string[] itemNames, MenuFunction[] callbacks, bool haveSeparator = false) {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < itemNames.Length; i++) {
                menu.AddItem(new GUIContent(itemNames[i]), false, callbacks[i]);
                if(haveSeparator)
                    menu.AddSeparator("");
            }
            menu.ShowAsContext();
        }


        #region 通用画List代码模板
        //public static IList DrawListProperty(string label, object list, Type contentType, out bool isDirty, string foldoutUniqueFlag = "") {
        //    isDirty = false;
        //    var oldList = list as IList;
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label("", GUILayout.ExpandWidth(true));
        //    if (GUILayout.Button(AddIcon, GUILayout.Width(20), GUILayout.Height(20))) {
        //        var createObj = GetDefaultValue(contentType);
        //        oldList.Add(createObj);
        //        isDirty = true;
        //    }
        //    GUILayout.EndHorizontal();
        //    GUILayout.Space(-20);

        //    if (DrawFoldout($"{label}   [{oldList?.Count}]", foldoutUniqueFlag)) {
        //        if (oldList != null && oldList.Count > 0) {
        //            EditorGUI.indentLevel++;
        //            for (int i = 0; i < oldList.Count; i++) {
        //                foldoutUniqueFlag = $"foldoutUniqueFlag_{i}";
        //                var item = oldList[i];
        //                Type itemType = item.GetType();
        //                SirenixEditorGUI.BeginBox();
        //                EditorGUILayout.BeginHorizontal();
        //                var newItem = DrawValue(itemType, item, itemType.Name, ref isDirty, foldoutUniqueFlag);
        //                if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(20))) {
        //                    // 删除操作，直接跳，后面的没必要操作了
        //                    oldList.RemoveAt(i);
        //                    isDirty = true;
        //                    break;
        //                }
        //                EditorGUILayout.EndHorizontal();
        //                SirenixEditorGUI.EndBox();
        //                // TODO Drag

        //                oldList[i] = newItem;
        //            }
        //            EditorGUI.indentLevel--;
        //        }
        //    }

        //    return oldList;
        //}

        #endregion
    }


}