using UnityEditor;
using UnityEngine;
using USDT.CustomEditor.CompileSound;
using USDT.Utils;

namespace USDT.CustomEditor {
    [InitializeOnLoad]
    public class InitializeOnLoadScript {
		static InitializeOnLoadScript() {
			//ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
			EditorApplication.projectWindowItemOnGUI += ProjectWindowDetails.ProjectWindowDetails.DrawAssetDetails;
			CompileSoundListener.Init();
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