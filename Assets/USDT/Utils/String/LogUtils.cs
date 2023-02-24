using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace USDT.Utils {

    public class LogUtils {
        private static StringBuilder _SB = new StringBuilder();

        public static void Log(object msg, bool isLogUpperLayerMethod = false) {
            if (isLogUpperLayerMethod) {
                var UpperLayerMethod = ReflectionUtils.GetStackTraceUpperLayer();
                var prefixName = StringUtils.Format(ColorStringConst.CyanFormat, $"【{UpperLayerMethod.Name}】");
                msg = StringUtils.Concat(prefixName, "：", msg.ToString());
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
                var prefixName = StringUtils.Format(ColorStringConst.RedFormat, $"【{UpperLayerMethod.Name}】");
                msg = StringUtils.Concat(prefixName, "：", msg.ToString());
            }
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            UnityEngine.Debug.LogError(msg);
#else
        Console.WriteLine(msg);
#endif
        }
    }
}