using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace USDT.Utils {
    public static class CommandlineUtils {
        //示例
        //CommandUtil.ExecuteInCmd("ipconfig");
        //CommandUtil.ExecuteOutCmd("-I http://www.baidu.com", @"C:\curl.exe");

        public static string ExecuteInCmd(string cmdline) {
            using (var process = new Process()) {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine(cmdline + "&exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                process.Close();
                return output;
            }
        }

        public static string ExecuteOutCmd(string argument, string applocaltion) {
            using (var process = new Process()) {
                process.StartInfo.Arguments = argument;
                process.StartInfo.FileName = applocaltion;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine("exit");

                //获取cmd窗口的输出信息  
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return output;
            }
        }

        public static void ExecuteCmd(string shellName, string workingDir) {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = shellName;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = workingDir;
            process.Start();
            UnityEngine.Debug.Log($"ExeCmd {process.StartInfo.FileName} {shellName}    ##workingDir={process.StartInfo.WorkingDirectory} ");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            UnityEngine.Debug.Log(output);
        }

    }

    public static class CommandlineArgsReader {
        static List<string> _arguments;
        static Dictionary<string, string> _arg2valDic;

        static CommandlineArgsReader() {
            _arguments = Environment.GetCommandLineArgs().ToList();
            _arg2valDic = new Dictionary<string, string>();
        }

        public static string GetArgValue(string key) {
            if(_arg2valDic.TryGetValue(key, out string value)) {
                return value;
            }
            var _key = $"-{key}";
            int index = -1;
            index = _arguments.FindIndex(arg => arg.Equals(_key, StringComparison.OrdinalIgnoreCase));
            if (index > 0) {
                _arg2valDic.Add(key, _arguments[index + 1]);
            }
            return _arguments[index + 1];
        }
    }
}
