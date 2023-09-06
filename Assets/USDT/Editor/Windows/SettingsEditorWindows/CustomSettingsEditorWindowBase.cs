using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace USDT.CustomEditor {

    public class CustomSettingsBaseSO : ScriptableObject {
        public const string AssetPath = "Assets/USDT/Editor/Windows/SettingsEditorWindows/CustomSettingsBase.asset";
        public string label;
        public static CustomSettingsBaseSO GetOrCreateSettings() {
            var settings = AssetDatabase.LoadAssetAtPath<CustomSettingsBaseSO>(AssetPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<CustomSettingsBaseSO>();
                settings.label = "自定义ProjectSetting";
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static SerializedObject GetSerializedSettings() {
            return new SerializedObject(GetOrCreateSettings());
        }

        #region 配置相关
        // 绘制iphoneX安全区域
        public bool drawIPhoneXSafeArea = false;

        #endregion
    }


    public class CustomSettingsEditorWindowBase : SettingsProvider {
        public CustomSettingsEditorWindowBase(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        private CustomSettingsBaseSO _so;

        public static bool IsSettingsAvailable() {
            return true;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            //_serializedObjectRoot = CustomSettingsBaseSO.GetSerializedSettings();
            //this.label = _serializedObjectRoot.FindProperty("label").stringValue;
            _so = CustomSettingsBaseSO.GetOrCreateSettings();
            this.label = _so.label;
        }

        public override void OnGUI(string searchContext) {
            EditorGUILayout.Space(25);

            EditorGUI.BeginChangeCheck();

            _so.drawIPhoneXSafeArea = EditorGUILayout.Toggle("drawIPhoneXSafeArea", _so.drawIPhoneXSafeArea);



            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(_so);
                AssetDatabase.Refresh();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() {
            if (IsSettingsAvailable()) {
                var provider = new CustomSettingsEditorWindowBase("Project/CustomSettingsProvider", SettingsScope.Project);
                return provider;
            }
            return null;
        }
    }
}
