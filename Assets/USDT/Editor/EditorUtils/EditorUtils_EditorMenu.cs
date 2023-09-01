using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using USDT.CustomEditor.CompileSound;
using USDT.CustomEditor.Excel;

namespace USDT.CustomEditor {

    // Editor Menu
    public static partial class EditorUtils {

        [MenuItem("EditorUtils/测试")]
        private static void Test() {
            //ReflectionUtils.GetSetFieldPtrValue();
        }

        [MenuItem("EditorUtils/Path/OpenPersistentDataPath")]
        private static void OpenPersistentDataPath() {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("EditorUtils/Windows/SearchTexture")]
        private static void OpenSearchTextureWindow() {
            EditorWindow.GetWindow<SearchImageWindow>();
        }

        [MenuItem("EditorUtils/Windows/EditorStyleViewer")]
        private static void OpenEditorStyleViewer() {
            EditorWindow.GetWindow<EditorStyleViewer>().Close();
            EditorWindow.GetWindowWithRect<EditorStyleViewer>(EditorMenuConst.ScreenCenterRect);
        }

        #region Excel

        [MenuItem("EditorUtils/Excels/导出表类CS、数据Json")]
        private static void ExportALL() {
            ExcelExportUtil.ExportScriptALL();
            ExcelExportUtil.ExportDataALL();
        }
        [MenuItem("EditorUtils/Excels/导出表类CS")]
        private static void ExportScript() {
            ExcelExportUtil.ExportScriptALL();
        }
        [MenuItem("EditorUtils/Excels/导出表数据")]
        private static void ExportData() {
            ExcelExportUtil.ExportDataALL();
        }
        #endregion

        #region 脚本编译

        [MenuItem("EditorUtils/Compilation/强制请求编译脚本")]
        public static void RequestScriptReload() {
            CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active)]
        public static void ToggleWaitMusic() {
            CompileSoundSettings.UseWaitMusic ^= true;
            CompileSoundListener.UpdateToggle();
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active, true)]
        public static bool CheckToggleWaitMusic() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Active, CompileSoundSettings.UseWaitMusic);
            return true;
        }


        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle)]
        public static void ChooseShuffle() {
            CompileSoundSettings.Shuffle ^= true;
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle, true)]
        public static bool CheckChooseShuffle() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Shuffle, CompileSoundSettings.Shuffle);
            return true;
        }
        #endregion
    }
}