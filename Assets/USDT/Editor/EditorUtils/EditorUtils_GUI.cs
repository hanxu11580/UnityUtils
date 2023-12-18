using UnityEditor;
using UnityEngine;
using USDT.Utils;
using static UnityEditor.GenericMenu;

namespace USDT.CustomEditor {

    public static partial class EditorUtils {
        #region 备忘录

        //改变背景颜色：GUI.backgroundColor

        //改变内容颜色： GUI.contentColor

        //改变内容改变内容和背景颜色： GUI.color

        //GUI的激活状态：GUI.enabled

        // 这玩意只有在重绘事件起作用 平常返回的永远是常量
        // 获取控件矩形 GUILayoutUtility.GetLastRect()

        // TextField逐字Undo
        // 记录操作前，Undo.IncrementCurrentGroup();

        // GUILayout是GUI的拓展 GUI忽略Layout
        // EditorGUILayout和EditorGUI是上面俩的扩展，但在运行时无法使用

        // GUILayout.FlexibleSpace()可以实现控件1在最左边，控件2在最右边

        // GUIUtility.GetStateObject可以实现将控件状态带入下一个OnGUI中

        // Tools.current记录当前是unity qwer哪种

        // 如果没有控件处于hotControl，则它基本上返回未经过滤的当前事件。
        // 这使每个控件都有机会使其首先变为hotControl。
        // 设置hotControl后，“GetTypeForControl”仅在传递的控件ID当前为hotControl或具有键盘焦点时才返回事件。
        // hotControl主要过滤鼠标事件，而KeyboardControl过滤按键事件

        // 下面可以实现：防止取消选择当前对象
        //int myID = GUIUtility.GetControlID(FocusType.Passive);
        //HandleUtility.AddDefaultControl(myID);
        #endregion

        public static GUIStyle Selection = "SelectionRect";

        public readonly static Color DefaultFontColor;

        static EditorUtils() {
        }

        static GUIContent tempContent;
        public static GUIContent TempContent(string text, Texture2D image = null, string tooltip = null) {
            if (tempContent == null) tempContent = new GUIContent();
            tempContent.text = text;
            tempContent.image = image;
            tempContent.tooltip = tooltip;
            return tempContent;
        }

        /// <summary>
        /// 画一个盒子
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="tint"></param>
        public static void DrawBoxColor(Rect rect, Color tint, string text = "", GUIStyle style = null) {
            Color c = GUI.color;
            GUI.color = tint;
            if(style != null) {
                GUI.Box(rect, text, style);
            }
            else {
                GUI.Box(rect, text);
            }
            GUI.color = c;
        }

        /// <summary>
        /// 折叠
        /// </summary>
        /// <param name="label"></param>
        /// <param name="uniqueFlag">唯一的标识</param>
        /// <returns></returns>
        public static bool DrawFoldout(string label, string uniqueFlag) {
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

        public static void TryOpenCSFile(string path, bool isAssetsPath = true) {
            if (!isAssetsPath) {
                path = PathUtils.GetAssetsPath(path);
            }

            var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            if (lScript != null) {
                AssetDatabase.OpenAsset(lScript);
            }
        }

        /// <summary>
        /// 触发组合键
        /// </summary>
        /// <param name="preKey"></param>
        /// <param name="postKey"></param>
        /// <param name="postKeyEvent"></param>
        /// <returns></returns>
        public static bool KeyCombinationsTick(EventModifiers preKey, KeyCode postKey, EventType postKeyEvent) {
            if (preKey != EventModifiers.None) {
                // 修饰键位和preKey相同则表示按下
                bool modifiersEventDown = (Event.current.modifiers & preKey) != 0;
                if (modifiersEventDown // 修饰键按下
                    && Event.current.rawType == postKeyEvent
                    && Event.current.keyCode == postKey) {
                    Event.current.Use();
                    return true;
                }
            }
            else {
                // 没有修饰符键位
                if (Event.current.rawType == postKeyEvent 
                    && Event.current.keyCode == postKey) {
                    Event.current.Use();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 在当前区域按下鼠标右键
        /// </summary>
        /// <returns></returns>
        public static bool PressRightMouseInCurrentArea() {
            var behaviorRect = GUILayoutUtility.GetLastRect();
            if (behaviorRect.Contains(Event.current.mousePosition)) {
                if (Event.current.type == EventType.MouseUp
                    && Event.current.button == 1) {
                    return true;
                }
            }
            return false;
        }

        public static Texture2D FindBuiltinTexture(string name) {
            var t = EditorGUIUtility.FindTexture(name);
            if (t != null)
                return t;

            var c = EditorGUIUtility.IconContent(name);
            if (c != null && c.image != null)
                return (Texture2D)c.image;

            return null;
        }

        public static void DisplayProgressBar(string title, string content, int index, int max) {
            UnityEditor.EditorUtility.DisplayProgressBar($"{title}({index}/{max}) ", content,
                index * 1f / max);
        }

        public static void ClearProgressBar() {
            UnityEditor.EditorUtility.ClearProgressBar();
        }

        public static Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
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