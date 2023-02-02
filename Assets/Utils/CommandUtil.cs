using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility {
    public static class CommandUtil {
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
    }
}
