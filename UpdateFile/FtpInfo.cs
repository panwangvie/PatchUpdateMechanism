using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile
{
    /// <summary>
    /// ftp信息
    /// </summary>
    public class FtpInfo
    {
        private string _userName = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get => _userName; set => _userName = value; }

        private string _passwd = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        public string Passwd { get => _passwd; set => _passwd = value; }

        private string _passwdMd5 = string.Empty;
        /// <summary>
        /// md5密码
        /// </summary>
        public string PasswdMd5 { get; set; }

        private string _host = string.Empty;
        /// <summary>
        ///ip地址 
        /// </summary>
        public string Host { get => _host; set => _host = value; }

        private int _port;
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get => _port; set => _port = value; }
      
    }
}
