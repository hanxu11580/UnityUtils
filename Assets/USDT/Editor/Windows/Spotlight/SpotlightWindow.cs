//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine.UIElements;
using USDT.CustomEditor;

//-----------------------------------------------------------------------------

namespace USDT.CustomEditor {

    public struct ActionData {
        public string searchPattrnValue;
        public Action action;
    }

    public partial class SpotlightWindow : EditorWindow {

        [MenuItem(EditorMenuConst.Spotlight_MenuItemName_OpenWindow)]
        public static void OpenSpotlightAll() {
            try {
                _FillActionData(true);
                _CloseSpotlight();
                _Win = CreateInstance<SpotlightWindow>();

                int w = Screen.currentResolution.width;
                int h = Screen.currentResolution.height;
                if (h > w) {
                    w = Screen.currentResolution.height;
                    h = Screen.currentResolution.width;
                }

                _Win.position = new Rect(w / 2 - WIDTH_WINDOW * 0.5f, h / 2 - HEIGHT_WINDOW * 0.5f, WIDTH_WINDOW, HEIGHT_WINDOW);
                _Background = GrabScreen(_Win.position);
                Blur b = new Blur(WIDTH_WINDOW, HEIGHT_WINDOW, 16);
                _Background = b.BlurTexture(_Background);

                _Win.ShowPopup();
                _Win.Focus();
                _Win.Init();
            }catch(Exception e) {
                Debug.LogError(e);
            }
        }

        [MenuItem(EditorMenuConst.Spotlight_MenuItemName_RefreshData)]
        public static void ClearData() {
            try {
                _ActionData.Clear();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }


        private static SpotlightWindow _Win;

        private static Texture _Background;

        private static Dictionary<string, ActionData> _ActionData = new Dictionary<string, ActionData>();

        private const int WIDTH_WINDOW = 800;

        private const int HEIGHT_WINDOW = 500;

        private const int BASELAYOUT_HEIGHT = 35;

        private const int INFOPANEL_WIDTH = 50;

        private const int HITPANEL_WIDTH = 730;


        private InputEventWrapper inputWrapper;

        private StyleTextureWrapper styles;

        private string searchPattern = "";

        private List<string> hits = new List<string>();

        private List<Action> hitsAction = new List<Action>();

        private List<StyleTextureWrapper.IconType> hitsIcon = new List<StyleTextureWrapper.IconType>();

        private int currentSelection = 0;

        public void Init() {

            this.inputWrapper = new InputEventWrapper();
            this.styles = new StyleTextureWrapper();
        }

        private void OnGUI() {
            try {
                GUI.DrawTexture(new Rect(0, 0, WIDTH_WINDOW, HEIGHT_WINDOW), _Background);

                Event cur = Event.current;
                if (cur != null && cur.isKey && cur.keyCode == KeyCode.Escape) {
                    _CloseSpotlight();
                    return;
                }
                this.inputWrapper.GatherInputs();
                this.DrawContent();
                this.ProcessInputs();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private void OnLostFocus() {
            // 脱离聚焦调试非常恶心
            //try {
            //    _CloseSpotlight();
            //}catch(Exception e) {
            //    Debug.LogError(e);
            //}
        }

        private Vector2 scrollPosition;
        private void DrawContent() {

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchPattern");
            this.searchPattern = EditorGUILayout.TextField(this.searchPattern, this.styles.SearchStyle, GUILayout.Height(BASELAYOUT_HEIGHT));
            EditorGUI.FocusTextInControl("SearchPattern");

            if (EditorGUI.EndChangeCheck()) {


                this.currentSelection = 0;

                this.hits.Clear();
                this.hitsAction.Clear();
                this.hitsIcon.Clear();

                string searchFlag = "";
                if (!string.IsNullOrEmpty(this.searchPattern)) {
                    string searchPatternTmp = this.searchPattern;
                    // 添加标签查找
                    if (searchPatternTmp.Contains(":")) {
                        var splitSearchPattern = searchPatternTmp.Split(':');
                        // 取:后面的字符串
                        searchPatternTmp = splitSearchPattern[splitSearchPattern.Length - 1];
                        searchFlag = SearchFlagTextConvert(splitSearchPattern[0]);
                    }

                    searchPatternTmp = searchPatternTmp.Replace(" ", "");
                    if (string.IsNullOrEmpty(searchPatternTmp)) {
                        return;
                    }
                    // 不让 / 搞那么复杂没屌用
                    //s2 = s2.Replace("/", "");
                    int counter = 0;
                    foreach (var item in SpotlightWindow._ActionData) {
                        // 过滤标签
                        if (!string.IsNullOrEmpty(searchFlag)) {
                            if (!item.Key.EndsWith(searchFlag)) {
                                continue;
                            }
                        }

                        int index = item.Value.searchPattrnValue.IndexOf(searchPatternTmp, StringComparison.InvariantCultureIgnoreCase);

                        if (index != -1) {
                            this.hits.Add(item.Key);
                            this.hitsAction.Add(item.Value.action);
                            this.hitsIcon.Add(this.styles.GetIconTypeFromString(item.Key));
                            counter++;
                        }

                        // 速度考虑最大显示50，不然就多写点精准的匹配字符
                        if (counter >= 50) {
                            break;
                        }
                    }
                }
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);


            using (new EditorGUILayout.VerticalScope(GUIStyle.none)) {
                // 只显示7个，但是这样就没有滑动条了
                // 因为超出显示界面才会有滑动条
                //int maxItems = 6;
                //int start = this.currentSelection;
                //int end = this.currentSelection + maxItems;

                //if (end >= this.hits.Count) {

                //    end = this.hits.Count - 1;
                //    start = end - maxItems;
                //    if (start < 0) {
                //        start = 0;
                //    }
                //}

                // 画全部匹配数据
                int start = 0;
                int end = this.hits.Count - 1;

                for (int i = start; i <= end; i++) {

                    string desc = this.hits[i];
                    if (desc.Length > 45) {
                        desc = "..." + desc.Substring(Mathf.Max(0, desc.Length - 45));
                    }

                    using (new EditorGUILayout.HorizontalScope(GUIStyle.none)) {

                        using (new EditorGUILayout.VerticalScope(GUIStyle.none, GUILayout.Width(INFOPANEL_WIDTH))) {
                            GUILayout.Box(this.styles.GetTexture(this.hitsIcon[i], i == this.currentSelection), this.styles.IconStyle, GUILayout.Height(BASELAYOUT_HEIGHT));
                        }

                        using (new EditorGUILayout.VerticalScope(GUIStyle.none, GUILayout.Width(HITPANEL_WIDTH))) {
                            if (GUILayout.Button(desc, i == this.currentSelection ? this.styles.HitStyleSelected : this.styles.HitStyle, GUILayout.Height(30))) {
                                this.ExecuteItemAtIndex(i);
                                Debug.Log(i);
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void ExecuteItemAtIndex(int i) {

            try {
                switch (this.hitsIcon[i]) {

                }

                _ActionData[this.hits[i]].action();
            }
            catch (Exception e) {
                Debug.LogError($"{e}");
            }
        }


        private int _pageStart = 0;
        private int _pageEnd = 12;
        private int _pageItemCount = 13;
        private void ProcessInputs() {
            if (this.inputWrapper.DownArrow) {

                this.currentSelection++;
                this.Repaint();
            }
            else if (this.inputWrapper.UpArrow) {
                this.currentSelection--;
                this.Repaint();
            }
            // 上下滚动不用换选择的项目
            else if (this.inputWrapper.ScrollPositive) {
                //this.currentSelection++;
                //this.Repaint();
            }
            else if (this.inputWrapper.ScrollNegative) {
                //this.currentSelection--;
                //this.Repaint();
            }

            if (this.hits.Count <= 0) {
                this.currentSelection = 0;
            }
            else if (this.currentSelection >= this.hits.Count) {
                this.currentSelection = 0;
                //this.currentSelection = this.hits.Count - 1;
            }
            else if (this.currentSelection < 0) {
                this.currentSelection = this.hits.Count - 1;
                //this.currentSelection = 0;
            }

            if (this.inputWrapper.Confirm) {

                if (this.hits.Count > 0) {
                    this.ExecuteItemAtIndex(this.currentSelection);
                }
            }


            // 按键处理
            if (this.inputWrapper.DownArrow) {
                if (this.currentSelection == 0) {
                    scrollPosition.y = 0;
                    this._pageStart = 0;
                    this._pageEnd = 12;
                }
                else {
                    if (this.currentSelection > this._pageEnd) {
                        this._pageStart = this.currentSelection;
                        this._pageEnd = this.currentSelection + this._pageItemCount - 1;
                        if (this._pageEnd > this.hits.Count) {
                            this._pageEnd = this.hits.Count;
                            // 此时窗口变小了，把窗口保持原来的大小
                            this._pageStart = this.hits.Count - this._pageItemCount;
                            scrollPosition.y = this.hits.Count * 35;
                        }
                        else {
                            scrollPosition.y += this._pageItemCount * 35;
                        }
                    }
                }
            }
            else if (this.inputWrapper.UpArrow) {
                if (this.currentSelection == this.hits.Count - 1) {
                    scrollPosition.y = this.hits.Count * 35;
                    this._pageStart = this.hits.Count - _pageItemCount;
                    this._pageEnd = this.hits.Count - 1;
                }
                else {
                    if (this.currentSelection < this._pageStart) {
                        this._pageStart = this.currentSelection - this._pageItemCount + 1;
                        this._pageEnd = this.currentSelection;
                        if (this._pageStart < 0) {
                            this._pageStart = 0;
                            this._pageEnd = this._pageItemCount;
                            scrollPosition.y = 0;
                        }
                        else {
                            scrollPosition.y -= this._pageItemCount * 35;
                        }
                    }
                }
            }
            this.inputWrapper.ClearInputs();
        }

        private static void _CloseSpotlight() {
            if (_Win != null) {
                _Win.Close();
            }
        }

        private static void _FillActionData(bool includeAssetdatabase = false) {

            //if (SpotlightWindow.actionData.Count == 0) {

            //    SpotlightWindow.actionData.Clear();
            //    SpotlightWindow.AddDefaultItems();
            //    SpotlightWindow.GetMenuItems();
            //    SpotlightWindow.GetComponentsItems();
            //    if (includeAssetdatabase) {
            //        SpotlightWindow.GetAssetItems();
            //    }
            //}
            if (includeAssetdatabase) {
                GetAssetItems();
            }
        }

        //private static void AddDefaultItems() {

        //    List<string> items = new List<string>();

        //    items.Add("Edit/Preferences...");
        //    items.Add("Edit/Modules...");
        //    items.Add("Edit/Project Settings/Input");
        //    items.Add("Edit/Project Settings/Tags and Layers");
        //    items.Add("Edit/Project Settings/Audio");
        //    items.Add("Edit/Project Settings/Time");
        //    items.Add("Edit/Project Settings/Player");
        //    items.Add("Edit/Project Settings/Physics");
        //    items.Add("Edit/Project Settings/Physics 2D");
        //    items.Add("Edit/Project Settings/Quality");
        //    items.Add("Edit/Project Settings/Graphics");
        //    items.Add("Edit/Project Settings/Network");
        //    items.Add("Edit/Project Settings/Editor");
        //    items.Add("Edit/Project Settings/Script Execution Order");


        //    items.Add("File/New Scene");
        //    items.Add("File/Open Scene");
        //    items.Add("File/Save Project");
        //    items.Add("File/Build Settings...");
        //    items.Add("File/Build % Run");
        //    items.Add("File/Exit");



        //    items.Add("GameObject/Create Empty");
        //    items.Add("GameObject/Create Empty Child");
        //    items.Add("GameObject/3D Object/Cube");
        //    items.Add("GameObject/3D Object/Sphere");
        //    items.Add("GameObject/3D Object/Capsule");
        //    items.Add("GameObject/3D Object/Cylinder");
        //    items.Add("GameObject/3D Object/Plane");
        //    items.Add("GameObject/3D Object/Quad");
        //    items.Add("GameObject/2D Object/Sprite");
        //    items.Add("GameObject/Light/Directional Light");
        //    items.Add("GameObject/Light/Point Light");
        //    items.Add("GameObject/Light/Spotlight");
        //    items.Add("GameObject/Light/Area Light");
        //    items.Add("GameObject/Light/Reflection Probe");
        //    items.Add("GameObject/Light/Light Probe Group");
        //    items.Add("GameObject/Audio/Audio Source");
        //    items.Add("GameObject/Audio/Audio Reverb Zone");
        //    items.Add("GameObject/Video/Video Player");
        //    items.Add("GameObject/Effects/Particle System");
        //    items.Add("GameObject/Effects/Trail");
        //    items.Add("GameObject/Effects/Line");
        //    items.Add("GameObject/Camera");

        //    items.Add("GameObject/Set as first sibling %=");
        //    items.Add("GameObject/Set as first sibling %");
        //    items.Add("GameObject/Set as last sibling");
        //    items.Add("GameObject/Set as last sibling %-");
        //    items.Add("GameObject/Move To View %&f");
        //    items.Add("GameObject/Move To View %&f");
        //    items.Add("GameObject/Align With View %#f");
        //    items.Add("GameObject/Align With View %#f");
        //    items.Add("GameObject/Align View to");
        //    items.Add("GameObject/Align View to Selected");
        //    items.Add("GameObject/Toggle Active State");
        //    items.Add("GameObject/Toggle Active State &#a");

        //    foreach (var item in items) {

        //        if (!SpotlightWindow.actionData.ContainsKey(item)) {

        //            SpotlightWindow.actionData.Add(item,
        //                () => {
        //                    try {
        //                        EditorApplication.ExecuteMenuItem(item);
        //                    }
        //                    catch (Exception e) {
        //                        Debug.LogError(e.Message);
        //                    }
        //                }
        //                );
        //        }
        //    }
        //}

        public static Type[] GetAllTypes() {

            List<Type> res = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                res.AddRange(assembly.GetTypes());
            }
            return res.ToArray();
        }

        //private static void GetComponentsItems() {

        //    Type[] types = GetAllTypes();

        //    for (int i = 0; i < types.Length; i++) {

        //        if (types[i].IsInterface) {
        //            continue;
        //        }
        //        if (types[i].IsAbstract) {
        //            continue;
        //        }

        //        if (!types[i].IsSubclassOf(typeof(Component))) {
        //            continue;
        //        }

        //        string desc = "Component/" + types[i].FullName;

        //        if (!SpotlightWindow.actionData.ContainsKey(desc)) {

        //            string tt = types[i].FullName + ", " + Assembly.GetAssembly(types[i]);
        //            SpotlightWindow.actionData.Add(desc, () => {
        //                ExecuteComponentAdd(tt);
        //            });
        //        }
        //    }
        //    //}
        //}


        private static void GetAssetItems() {
            if (_ActionData.Count > 0) {
                return;
            }

            // 这个很慢
            //string[] allAssets = AssetDatabase.GetAllAssetPaths();

            string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

            List<string> allAssets = new List<string>(allFiles.Length);

            foreach (string file in allFiles) {
                if (Path.GetExtension(file) == ".meta")
                    continue;
                string unityPath = "Assets" + file.Substring(Application.dataPath.Length);
                allAssets.Add(unityPath);
            }

            for (int i = 0; i < allAssets.Count; i++) {
                var path = allAssets[i];
                //if (Directory.Exists(path)) {
                //    // 目录过滤掉
                //    continue;
                //}
                var extension = Path.GetExtension(path);
                var iconType = GetActionDataIconType(extension);
                string desc = $"{iconType}/{path}";
                if (!_ActionData.ContainsKey(desc)) {
                    ActionData action = new ActionData();
                    string tmp = desc.Replace(" ", "");
                    var s1ArrTmp = tmp.Split('.');
                    tmp = s1ArrTmp[0];
                    tmp = tmp.Replace(@"\", "/");
                    if (tmp.Contains("/")) {
                        // 取最后一个名字
                        var arr = tmp.Split('/');
                        tmp = arr[arr.Length - 1];
                    }
                    action.searchPattrnValue = tmp;
                    action.action = () => {
                        SelectAndPingAsset(path, iconType);
                    };
                    _ActionData.Add(desc, action);
                }
            }
        }

        private static void SelectAndPingAsset(string path, StyleTextureWrapper.IconType iconType) {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(Selection.activeObject);

            if (Selection.activeObject != null) {
                if (iconType == StyleTextureWrapper.IconType.Component
                    || iconType == StyleTextureWrapper.IconType.File) {
                    if (obj != null) {
                        AssetDatabase.OpenAsset(obj);
                    }
                }
            }
        }

        //private static void ExecuteComponentAdd(string type) {


        //    Type t = Type.GetType(type);
        //    if (t != null) {

        //        if (Selection.gameObjects.Length != 0) {
        //            for (int i = 0; i < Selection.gameObjects.Length; i++) {
        //                Selection.gameObjects[i].AddComponent(t);
        //            }
        //        }
        //    }
        //    else {
        //        Debug.LogError(type);
        //    }

        //}


        //private static void GetMenuItems() {

        //    List<Type> classes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
        //        .Where(t =>
        //            t.Assembly.FullName.Contains("Assembly-CSharp-Editor")
        //            ).ToList();
        //    for (int i = 0; i < classes.Count; i++) {

        //        List<MethodInfo> methods = classes[i].GetMethods(BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        //            .Where(m => SpotlightWindow.GetCustomAttribute<MenuItem>(m) != null).ToList();

        //        foreach (var method in methods) {

        //            var attr = SpotlightWindow.GetCustomAttribute<MenuItem>(method);
        //            if (!attr.menuItem.StartsWith("CONTEXT") && !SpotlightWindow.actionData.ContainsKey(attr.menuItem)) {
        //                SpotlightWindow.actionData.Add(attr.menuItem, () => EditorApplication.ExecuteMenuItem(attr.menuItem));
        //            }
        //        }
        //    }
        //}


        private static Texture GrabScreen(Rect rect) {
            Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(rect.position, (int)rect.width, (int)rect.height);
            var tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        public static T GetCustomAttribute<T>(MemberInfo member) where T : Attribute {
            return GetCustomAttribute<T>(member, true);
        }


        public static T GetCustomAttribute<T>(MemberInfo member, bool inherit) where T : Attribute {
            return (T)((IEnumerable<object>)member.GetCustomAttributes(typeof(T), inherit)).FirstOrDefault<object>();
        }


        private static string SearchFlagTextConvert(string flag) {
            switch (flag) {
                case "cs": return ".cs";
                case "sh": return ".shader";
                case "p": return ".prefab";
                case "anim": return ".anim";
                case "sc": return ".unity";
                case "as":return ".asset";
                default: return null;
            }
        }

        private static StyleTextureWrapper.IconType GetActionDataIconType(string extension) {
            switch (extension) {
                case ".cs":
                case ".shader": {
                        return StyleTextureWrapper.IconType.Component;
                    }
                case ".prefab": return StyleTextureWrapper.IconType.GameObject;
                case ".anim":
                case ".asset": {
                        return StyleTextureWrapper.IconType.Tools;
                    }
                case ".unity": return StyleTextureWrapper.IconType.Scene;
                case ".txt":
                case ".json":
                case ".md": {
                        return StyleTextureWrapper.IconType.File;
                    }
                default: return StyleTextureWrapper.IconType.Unknown;
            }
        }
    }
}
