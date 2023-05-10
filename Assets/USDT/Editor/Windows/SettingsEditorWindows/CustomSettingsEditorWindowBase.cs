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
                settings.label = "×Ô¶¨ÒåProjectSetting";
                AssetDatabase.CreateAsset(settings, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static SerializedObject GetSerializedSettings() {
            return new SerializedObject(GetOrCreateSettings());
        }
    }


    public class CustomSettingsEditorWindowBase : SettingsProvider {
        public CustomSettingsEditorWindowBase(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        private SerializedObject _serializedObjectRoot;
        public static bool IsSettingsAvailable() {
            return true;
            //return File.Exists(CustomSettingsBaseSO.AssetPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            _serializedObjectRoot = CustomSettingsBaseSO.GetSerializedSettings();
            this.label = _serializedObjectRoot.FindProperty("label").stringValue;
        }

        public override void OnGUI(string searchContext) {
            //EditorGUILayout.PropertyField(_serializedObjectRoot.FindProperty("label"), EditorUtils.TempContent("Title"));
            //_serializedObjectRoot.ApplyModifiedPropertiesWithoutUndo();
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
