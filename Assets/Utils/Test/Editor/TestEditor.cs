using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace EditorUtils {

    [CustomEditor(typeof(Utility.Test))]
    public class TestEditor:Editor{


        private void OnEnable() {
            
        }


        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }



    }
}