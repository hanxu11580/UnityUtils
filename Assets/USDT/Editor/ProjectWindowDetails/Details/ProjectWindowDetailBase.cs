using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor.ProjectWindowDetails {

    public abstract class ProjectWindowDetailBase {
        private readonly static Dictionary<string, string> _labelMap = new Dictionary<string, string>();
        private const string ShowPrefsKey = "ProjectWindowDetails.Show.";
        public int ColumnWidth = 100;
        public string Name = "Base";
        public TextAlignment Alignment = TextAlignment.Left;
        public bool Visible
        {
            get {
                return EditorPrefs.GetBool(string.Concat(ShowPrefsKey, Name));
            }
            set {
                EditorPrefs.SetBool(string.Concat(ShowPrefsKey, Name), value);
            }
        }
        public abstract string GetLabel(string guid, string assetPath, Object asset);

        public string GetLableWithCache(string guid, string assetPath, Object asset) {
            string label = "";
            if (_labelMap.TryGetValue(assetPath, out label)) {
                return label;
            }
            else {
                var tempSize = GetLabel(guid, assetPath, asset);
                _labelMap.Add(assetPath, tempSize);
                label = tempSize;
            }
            return label;
        }


        public virtual void ClearLabelCache() {
            lg.i($"Çå¿ÕProjectWindowDetail.{Name} Label»º´æ");
            _labelMap.Clear();
        }
    }
}