using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace USDT.Utils {

    public class LogConst {
        public const string CyanFormat = "<b><color=cyan>{0}</color></b> ";
        public const string YellowFormat = "<b><color=yellow>{0}</color></b> ";
        public const string RedFormat = "<b><color=red>{0}</color></b> ";
    }


    public static class LogUtils {

        public static void Log(object msg, bool isLogUpperLayerMethod = false) {
            if (isLogUpperLayerMethod) {
                var UpperLayerMethod = ReflectionUtils.GetStackTraceUpperLayer();
                var prefixName = string.Format(LogConst.CyanFormat, $"【{UpperLayerMethod.Name}】");
                msg = $"{prefixName}：{msg}";
            }
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            UnityEngine.Debug.Log(msg);
#else
        Console.WriteLine(msg);
#endif
        }

        public static void LogError(object msg, bool isLogUpperLayerMethod = false) {
            if (isLogUpperLayerMethod) {
                var UpperLayerMethod = ReflectionUtils.GetStackTraceUpperLayer();
                var prefixName = string.Format(LogConst.RedFormat, $"【{UpperLayerMethod.Name}】");
                msg = $"{prefixName}：{msg}";
            }
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            UnityEngine.Debug.LogError(msg);
#else
        Console.WriteLine(msg);
#endif
        }
    }
}