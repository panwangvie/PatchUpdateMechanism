using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchUpdate.ftp
{
    public class FtpTool
    {


        #region 文件、目录名称有效性判断

        /// <summary>
        /// 判断目录名中字符是否合法
        /// </summary>
        /// <param name="DirectoryName">目录名称</param>
        public bool IsValidPathChars(string DirectoryName)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            char[] DirChar = DirectoryName.ToCharArray();
            foreach (char C in DirChar)
            {
                if (Array.BinarySearch(invalidPathChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断文件名中字符是否合法
        /// </summary>
        /// <param name="FileName">文件名称</param>
        public bool IsValidFileChars(string FileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            char[] NameChar = FileName.ToCharArray();
            foreach (char C in NameChar)
            {
                if (Array.BinarySearch(invalidFileChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}
