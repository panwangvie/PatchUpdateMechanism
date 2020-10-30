using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateFile;
using UpdateFile.Log;

namespace PatchUpdate
{
    public class MathBatVbs
    {
        /// <summary>
        /// 生成Vbs文件
        /// </summary>
        public static bool MathVbs(ref string vbsPath ,string batPath="")
        {
            batPath = AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.bat";
            vbsPath = AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.vbs";
            try
            {
                 StreamWriter stream = new StreamWriter(vbsPath, false);
                string text1 = "set ws = WScript.CreateObject(\"WScript.Shell\")";
                string text2 = $"ws.Run \"{batPath}\",0";
                stream.WriteLine(text1);
                stream.WriteLine(text2);
               

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();

                }
                return true;
            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");

            }
            return false;

        }

        /// <summary>
        /// 生成bat文件
        /// </summary>
        public static bool MathBat(string newPath, string oldPath)
        {
            try
            {
                System.IO.StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.bat", false);

                string text = $"@echo off\nchoice / t 1 / d y / n > nul \ncopy {newPath} {oldPath}\ndel {AppDomain.CurrentDomain.BaseDirectory}UpdateThis.vbs\ndel {AppDomain.CurrentDomain.BaseDirectory}UpdateThis.bat\nexit";
                stream.Write(text);
                 
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();

                }
                return true;

            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");

            }
            return false;

        }

        /// <summary>
        /// copy语句
        /// </summary>
        /// <param name="copyPaths"></param>
        /// <param name="delPath"></param>
        /// <returns></returns>
        public static bool MathBat(string[] copyPaths, string[] delPaths)
        {
            try
            {
                StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.bat", false);
                string copy = string.Empty;
                string batText = "@echo off\nchoice / t 2 / d y / n > nul";
                for (var i = 0; i < copyPaths.Length; i++)
                {
                    batText += $"\ncopy {copyPaths[i]}";
                }
              
                for (var i = 0; i < delPaths.Length; i++)
                {
                    batText += $"\ndel {delPaths[i]}";
                }
                    
                batText += $"\ndel {AppDomain.CurrentDomain.BaseDirectory}UpdateThis.bat";
                batText += "\nexit";
                stream.WriteLine(batText);
                Logs.WriteLog(batText);

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
                return true;

            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");

            }
            return false;

        }
    }
}
