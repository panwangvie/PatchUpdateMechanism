using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UpdateFile.Interface;

namespace UpdateFile
{
    /// <summary>
    /// 
    /// </summary>
    public class StartUpdateClient : IStartUpdate
    {
        public void StartUpdate()
        {
           
        }

        /// <summary>
        /// 关闭当前客户端，启动PatchUpdate.exe更新程序，且将参数传入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateInfo"></param>
        public void StartUpdate<T>(T updateInfo)
        {
            FtpInfo ftpInfo = updateInfo as FtpInfo;
            if (ftpInfo == null)
            {
                Log.Logs.WriteLog("启动程序参数错误为空");
                return;
            }
            StartExe startExe = new StartExe();
            string[] args = new string[6];
            args[0] = ftpInfo.Host;
            args[1] = ftpInfo.Port+"";
            args[2] = ftpInfo.UserName;
            args[3] = ftpInfo.Passwd;
            args[4] = ftpInfo.PasswdMd5;
            args[5] = Json.Stringify(ftpInfo); //最后一个参数采用字符串格式

            startExe.Start(PathConfig.UpdateClient, args); 

            //关掉此程序
            Environment.Exit(0);
        }


        /// <summary>
        /// 启动更新
        /// </summary>
        /// <param name="ftpInfo"></param>
        /// <param name="version"></param>
        public void StartUpdate(FtpInfo ftpInfo, VersionInfo version)
        {

            StartExe startExe = new StartExe();
            string[] args = new string[7];
            args[0] = ftpInfo.Host;
            args[1] = ftpInfo.Port + "";
            args[2] = ftpInfo.UserName;
            args[3] = ftpInfo.Passwd;
            args[4] = ftpInfo.PasswdMd5;
            args[5] = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            args[6] = Json.Stringify(ftpInfo); //最后一个参数采用字符串格式
            args[7] = Json.Stringify(version);
 
            //启动更新程序
            startExe.Start(PathConfig.UpdateClient, args);
            //关掉此程序
            Environment.Exit(0);
        }

        
    }
}
