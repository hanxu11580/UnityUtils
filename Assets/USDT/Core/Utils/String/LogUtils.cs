using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace USDT.Utils {

    public class LogUtils {
        private static StringBuilder _SB = new StringBuilder();

        public static Action<string> LogCallback { get; set; }
        public static Action<string> LogErrorCallback { get; set; }

        public static void Log(object msg, bool isCallback = false) {
            if(msg == null) {
                return;
            }
#if UNITY_EDITOR
            var UpperLayerMethod = ReflectionUtils.GetStackTraceUpperLayer();
            var prefixName = StringUtils.Format(ColorStringConst.CyanFormat, $"[{UpperLayerMethod.DeclaringType.Name}.{UpperLayerMethod.Name}]");
            msg = StringUtils.Concat(prefixName, ":", msg.ToString());
#endif
            if (isCallback) {
                LogCallback?.Invoke(msg.ToString());
            }
            _Log(msg);
        }

        public static void LogError(object msg, bool isCallback = false) {
            if (msg == null) {
                return;
            }
#if UNITY_EDITOR
            var UpperLayerMethod = ReflectionUtils.GetStackTraceUpperLayer();
            var prefixName = StringUtils.Format(ColorStringConst.RedFormat, $"[{UpperLayerMethod.DeclaringType.Name}.{UpperLayerMethod.Name}]");
            msg = StringUtils.Concat(prefixName, ":", msg.ToString());
#endif
            if (isCallback) {
                LogErrorCallback?.Invoke(msg.ToString());
            }
            _LogError(msg);
        }

        private static void _Log(object msg) {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            UnityEngine.Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        private static void _LogError(object msg) {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS
            UnityEngine.Debug.LogError(msg);
#else
            Console.WriteLine(msg);
#endif
        }
    }
}