using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UpdateFile;
using UpdateFile.Log;

namespace PatchUpdate
{
    public class ProcessInfo
    {

        /// <summary>
        /// 关闭某个应用
        /// </summary>
        /// <param name="processName"></param>
        public bool KissByProcess(string processName)
        {
            bool res = false;
            try
            {
                Process[] processesByName = Process.GetProcessesByName(processName);
                Array.ForEach<Process>(processesByName, delegate (Process pro)
                {
                    pro.Kill();
                    res = true;
                });
            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace: {ex.StackTrace}");
            }
            return res;
        }
 
        /// <summary>
        /// 结束被使用的进程
        /// </summary>
        /// <param name="filePath">正在使用的文件具体路径</param>
        public bool KillProcess(string fileName)
        {
            bool res = false;
            try
            {
                Process tool = new Process();
                tool.StartInfo.FileName = "handle64.exe";
                tool.StartInfo.Arguments = fileName + " /accepteula";
                tool.StartInfo.UseShellExecute = false;
                tool.StartInfo.RedirectStandardOutput = true;
                tool.Start();
                string outputTool = tool.StandardOutput.ReadToEnd();
                string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
                foreach (Match match in Regex.Matches(outputTool, matchPattern))
                {
                    Process.GetProcessById(int.Parse(match.Value)).Kill();
                    res = true;
                }
            }catch(Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace: {ex.StackTrace}");
            }
            return res;
        }

        /// <summary>
        /// 关闭某个应用
        /// </summary>
        /// <param name="processName"></param>
        public void CloseSoundApp(string processName)
        {
            Process[] pProcess;
            pProcess = Process.GetProcesses();
            for (int i = 1; i <= pProcess.Length - 1; i++)
            {
                if (pProcess[i].ProcessName == processName)
                {
                    pProcess[i].Kill();
                    break;
                }
            }

        }

        public static void Start(string exePath)
        {

            if (File.Exists(exePath))
            {
              
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
                //不创建窗口
                startInfo.UseShellExecute = true;
                //设置启动动作,确保以管理员身份运行
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.Verb = "runas";
               

                Process process = new Process();
                process.StartInfo = startInfo;
                ////当EnableRaisingEvents为true，进程退出时Process会调用下面的委托函数
                process.EnableRaisingEvents = false;
                process.Start();
                process.Dispose();
            }
        }
    }
}
