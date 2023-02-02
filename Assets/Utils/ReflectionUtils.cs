using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility {
    public static class ReflectionUtils
    {
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
    }
}
