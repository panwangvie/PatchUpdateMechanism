using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile
{
    public  class PathConfig
    {
        #region ftp服务路径

        /// <summary>
        /// ftp根路径, 需要前面带/
        /// </summary>
        public readonly static string FtpUser = "/home/ftpuser";
        /// <summary>
        /// 补丁路径
        /// </summary>
        public readonly static string FtpPatchPath = FtpUser+"/client/patch";

        /// <summary>
        /// 版本信息文件名
        /// </summary>
        public readonly static string FtpConfigName = "VersionInfo.config";

        #endregion


        #region 本地路径

        /// <summary>
        /// 
        /// </summary>
        public readonly static string Load = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 更新程序的完整路径
        /// </summary>
        public readonly static string UpdateClient = Load+"PatchUpdate.exe";
        /// <summary>
        /// 本地版本信息文件名
        /// </summary>
        public readonly static string ConfigName = "VersionInfo.config";

        /// <summary>
        /// 本地补丁下载保存的位置
        /// </summary>
        public readonly static string  LoadPatchPath= Load+"data\\patch\\";

        /// <summary>
        /// 本地调用ftp日志位置
        /// </summary>
        public readonly static string LoadFtpLogPath =$"{ Load}data\\ftpLog\\{DateTime.Now.ToString("yyyMMdd")}.log";
         
        /// <summary>
        /// 本地日志
        /// </summary>
        public readonly static string LogPath = $"{LoadPatchPath}Logs\\Patch_{DateTime.Now.ToString("yyyyMMdd")}.log";

        /// <summary>
        /// 补丁文件更新信息
        /// </summary>
        public readonly static string FileJson = "fileinfo.json";

        /// <summary>
        /// 补丁更新的日志
        /// </summary>
        public readonly static string UpdateLog = $"{LoadPatchPath}Logs\\update_{DateTime.Now.ToString("yyyyMMdd")}.log";

        #endregion

    }
}
