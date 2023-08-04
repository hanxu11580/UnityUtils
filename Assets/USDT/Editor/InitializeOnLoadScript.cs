using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor {
	[InitializeOnLoad]
    public class InitializeOnLoadScript {
		static InitializeOnLoadScript() {
			ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
		}

		static void OnLeftToolbarGUI() {
			GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("1", "1 Button"))) {
				LogUtils.Log("click topbar 1 button");
			}

			if (GUILayout.Button(new GUIContent("2", "2  Button"))) {
				LogUtils.Log("click topbar 2 button");
			}
		}
	}
}