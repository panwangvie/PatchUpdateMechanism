using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile.Log
{
    public class Logs
    {

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="strFileFullPath">输出日志的完整路径（包含文件夹及文件名）</param>
        /// <param name="msg">日志输出的信息</param>
        public static void WriteLog(string msg, string strFileFullPath= "")
        {
            try
            {
                //强制文件名后缀为.log
                if (string.IsNullOrEmpty(strFileFullPath))
                {
                    strFileFullPath = PathConfig.LogPath; 
                }
                if (Path.GetExtension(strFileFullPath)!=".log")
                {
                    strFileFullPath += ".log";
                }

                //判断路径文件夹是否存在
                if (!Directory.Exists(Path.GetDirectoryName(strFileFullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strFileFullPath));
                }

                //判断文件名称是否存在
                if (!File.Exists(strFileFullPath))
                {
                    //不存在文件
                    File.Create(strFileFullPath).Close();//创建该文件
                }
                StreamWriter writer = File.AppendText(strFileFullPath);//文件中添加文件流  

                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                try
                {
                    writer.WriteLine($"{sf.GetMethod().DeclaringType.FullName}---{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}---{msg}");

                }
                catch (Exception e)
                {
                    //throw new Exception("写入日志错误！" + e.ToString());
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strFileFullPath"></param>
        public static void WriteLog(Exception ex, string strFileFullPath = "")
        {
            try
            {
                //强制文件名后缀为.log
                if (string.IsNullOrEmpty(strFileFullPath))
                {
                    strFileFullPath = PathConfig.LogPath;
                }
                if (Path.GetExtension(strFileFullPath) != ".log")
                {
                    strFileFullPath += ".log";
                }

                //判断路径文件夹是否存在
                if (!Directory.Exists(Path.GetDirectoryName(strFileFullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strFileFullPath));
                }

                //判断文件名称是否存在
                if (!File.Exists(strFileFullPath))
                {
                    //不存在文件
                    File.Create(strFileFullPath).Close();//创建该文件
                }
                StreamWriter writer = File.AppendText(strFileFullPath);//文件中添加文件流  

                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                try
                {
                    writer.WriteLine($"{sf.GetMethod().DeclaringType.FullName}---{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}---Message:{ex.Message},StackTrace:{ex.StackTrace} ");
                }
                catch (Exception e)
                {
                    //throw new Exception("写入日志错误！" + e.ToString());
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
