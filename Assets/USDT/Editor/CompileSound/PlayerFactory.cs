using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace USDT.CustomEditor.CompileSound {
    public class PlayerFactory
    {
        private const string PlayMethodKey = "EDITOR_MusicPlayMethod";
        public static IPlayer GetPlayer()
        {
            //if (!EditorPrefs.HasKey(PlayMethodKey))
            //    EditorPrefs.SetInt(PlayMethodKey, 1);

            //if (EditorPrefs.GetInt(PlayMethodKey) == 1)
            //    return new EnginePlayer();

            //return new NativePlayer();

            return new EnginePlayer();
        }
    }
}