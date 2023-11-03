using UnityEditor;
using UnityEngine;
using USDT.Components;
using USDT.CustomEditor.CompileSound;
using USDT.Utils;

namespace USDT.CustomEditor {
    public class InitializeOnLoadScript {

		static void OnLeftToolbarGUI() {
			GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("1", "1 Button"))) {
				lg.i("click topbar 1 button");
			}

			if (GUILayout.Button(new GUIContent("2", "2  Button"))) {
				lg.i("click topbar 2 button");
			}
		}

		[InitializeOnLoadMethod]
		private static void OnInitializeOnLoadMethod() {
			ProjectWindowDetails.ProjectWindowDetails.Init();
			CompileSoundListener.Init();
			IPhoneXSafeAreaDrawerEditor.IPhoneXSafeAreaDrawer.Init();
		}
	}
}