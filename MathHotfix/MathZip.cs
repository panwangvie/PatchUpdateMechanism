using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathHotfix
{
    public class MathZip
    {

        /// <summary>
        /// 通过系统IO流制作ZIP
        /// </summary>
        /// <param name="path">文件夹目录</param>
        /// <param name="name"></param>
        public bool MathByIOZip(string path,string name)
        {
            try
            {
                if (System.IO.File.Exists(name))
                {
                    System.IO.File.Delete(name);
                }
                ZipFile.CreateFromDirectory(path, name);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
