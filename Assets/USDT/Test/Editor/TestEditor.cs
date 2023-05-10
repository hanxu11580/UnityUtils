using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace USDT.Test {

    [UnityEditor.CustomEditor(typeof(Test))]
    public class TestEditor:Editor{


        private void OnEnable() {

        }


        public override void OnInspectorGUI() {
            base.OnInspectorGUI();


        }
    }
}