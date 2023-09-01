using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.CustomEditor {

    public class EditorMenuConst{
        /// <summary>
        /// ÆÁÄ»ÖÐÐÄÎ»ÖÃ
        /// </summary>
        public readonly static Rect ScreenCenterRect
            = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.5f, Screen.height * 0.5f);


        public const string CompileSound_MenuItemName_Active = "EditorUtils/Compilation/CompileSound/Active";
        public const string CompileSound_MenuItemName_Shuffle = "EditorUtils/Compilation/CompileSound/Shuffle";
    }
}