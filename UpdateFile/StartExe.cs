using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateFile.Log;

namespace UpdateFile
{
    public class StartExe
    {
        public EventHandler ExitedAction;

        /// <summary>
        /// 启动EXE
        /// </summary>
        /// <param name="exePath">程序路径</param>
        /// <param name="args">启动时带的参数</param>
        public void Start(string exePath, string[] args)
        {

            if (File.Exists(exePath))
            {
                string s = "";
                foreach (string arg in args)
                {
                    s += arg + " ";
                }
                s = s.Trim();
                ProcessStartInfo startInfo = new ProcessStartInfo(exePath, s);
                //不创建窗口
                startInfo.UseShellExecute = true;
                //设置启动动作,确保以管理员身份运行
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.CreateNoWindow = true;
                startInfo.Verb = "runas";

                Process process = new Process();
                process.StartInfo = startInfo;
                ////当EnableRaisingEvents为true，进程退出时Process会调用下面的委托函数
                process.Exited += ExitedAction;
                process.EnableRaisingEvents = false;
                process.Start();
                process.Dispose();
            }
            else
            {
                Logs.WriteLog($"启动程序{exePath}失败");
            }
        }
    }
}
