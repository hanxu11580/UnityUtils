using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace USDT.Utils {
    public static class ReflectionUtils
    {
        private static Type[] _types;

        public static Type[] GetTypes() {
            if (_types == null) {
                _types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where((Assembly assembly) => assembly.FullName.Contains("Assembly"))
                    .SelectMany((Assembly assembly) => assembly.GetTypes()).ToArray();
            }

            return _types;
        }

        public static Type FindTypeByFullName(string fullName) {
            foreach (var t in GetTypes()) {
                if(t.FullName == fullName) {
                    return t;
                }
            }
            return null;
        }

        public static Type[] GetAttriTypes(Type hasAttribute, bool inherit) {
            return GetTypes().Where((Type T) =>
                    (!T.IsAbstract)
                    && T.GetCustomAttributes(hasAttribute, inherit).Any())
                .ToArray();
        }

        public static Type[] GetInterfaces(Type iType) {
            return GetTypes().Where((Type T) => iType.IsAssignableFrom(T)).ToArray();
        }

        /// <summary>
        /// 继承baseType类型
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetSubTypes(Type baseType) {
            return GetTypes().Where((Type T) =>
                (T.IsClass && !T.IsAbstract)
                && T.IsSubclassOf(baseType))
                .ToArray();
        }

        /// <summary>
        /// 继承baseType类型、且有hasAttribute属性
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="hasAttribute"></param>
        /// <returns></returns>
        public static Type[] GetSubTypes(Type baseType, Type hasAttribute) {
            return GetTypes().Where((Type T) =>
                    (T.IsClass && !T.IsAbstract)
                    && T.IsSubclassOf(baseType)
                    && T.GetCustomAttributes(hasAttribute, false).Any())
                .ToArray();
        }


        #region Mono.Cecil
        // 需要Mono.Cecil的支持
        // 根据Type得到CS文件路径
        //private const string UnityDllPath = "D:\\Lockstep_dragon\\Library\\ScriptAssemblies\\Assembly-CSharp.dll";
        //public static string TypeToFilePath(string typeName)
        //{
        //    if (string.IsNullOrEmpty(typeName)) return string.Empty;
        //    AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(UnityDllPath, new ReaderParameters { ReadSymbols = true });
        //    TypeDefinition tr = ad.MainModule.GetType(typeName);
        //    foreach (var method in tr.Methods)
        //    {
        //        if (method.HasBody)
        //        {
        //            var body = method.Body;
        //            foreach (var seq in method.DebugInformation.SequencePoints)
        //            {
        //                if (seq != null && seq.Document != null)
        //                {
        //                    if (!string.IsNullOrEmpty(seq.Document.Url))
        //                    {
        //                        return seq.Document.Url;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return string.Empty;
        //}
        #endregion

        /// <summary>
        /// 可以获得当前正在调用的函数信息
        /// </summary>
        /// <returns></returns>
        public static MethodBase GetCurrentMethod() {
            return System.Reflection.MethodBase.GetCurrentMethod();
        }

        /// <summary>
        /// 获取上一层调用函数
        /// </summary>
        /// <returns></returns>
        public static MethodBase GetStackTraceUpperLayer() {
            var stacktrace = new StackTrace();
            if (stacktrace.FrameCount > 1) {
                var method = stacktrace.GetFrame(2).GetMethod();
                return method;
            }
            return null;
        }
    }
}
