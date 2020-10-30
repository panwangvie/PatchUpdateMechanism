using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile.Interface
{
    /// <summary>
    /// 开始更新
    /// </summary>
    public interface IStartUpdate
    {
        /// <summary>
        /// 
        /// </summary>
        void StartUpdate();

        void StartUpdate<T>(T updateInfo);

        /// <summary>
        /// 开始更新
        /// </summary>
        /// <param name="ftpInfo">ftp信息</param>
        /// <param name="version">版本信息</param>
        void StartUpdate(FtpInfo ftpInfo, VersionInfo version);
    }
}
