using UnityEditor;
using UnityEngine;
using USDT.Components;
using USDT.CustomEditor.CompileSound;
using USDT.Utils;

namespace USDT.CustomEditor {
    [InitializeOnLoad]
    public class InitializeOnLoadScript {
		static InitializeOnLoadScript() {
			//ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
			ProjectWindowDetails.ProjectWindowDetails.Init();
			CompileSoundListener.Init();
			IPhoneXSafeAreaDrawerEditor.IPhoneXSafeAreaDrawer.Init();
        }

		static void OnLeftToolbarGUI() {
			GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("1", "1 Button"))) {
				lg.i("click topbar 1 button");
			}

			if (GUILayout.Button(new GUIContent("2", "2  Button"))) {
				lg.i("click topbar 2 button");
			}
		}
	}
}