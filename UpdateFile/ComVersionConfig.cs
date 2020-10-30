using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateFile.Interface;

namespace UpdateFile
{
    public class ComVersionConfig : IComparisonVersion
    {
        /// <summary>
        /// 版本分割符
        /// </summary>
        public static char strSplit = '-';

        /// <summary>
        /// 是否需要更新
        /// </summary>
        /// <returns></returns>
        public bool IsSameVersion(ref VersionInfo version)
        {
            try
            {
                VersionInfo loadVersion = VersionInfo.GetVersionByJson($"{PathConfig.Load}{PathConfig.ConfigName}");
                VersionInfo patchVersion = VersionInfo.GetVersionByJson($"{PathConfig.LoadPatchPath}\\{PathConfig.ConfigName}");
                version = patchVersion;
                bool res = VersionRule(patchVersion?.Version, loadVersion?.Version);
                return res;
            }
            catch(Exception ex)
            {
                Log.Logs.WriteLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 版本号比较规则,例如 V1.0-H01-D20200827 其中V1.0为大版本号，H01为补丁号，D20200827为发布时间，比较规则先比V，再比较H，再比较D
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="oldVersion"></param>
        /// <returns></returns>
        public bool VersionRule(string newVersion, string oldVersion)
        {
            if (string.IsNullOrEmpty(newVersion)) 
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldVersion))
            {
                return true;
            }
            try
            {

                string[] news = newVersion.Split(strSplit);
                string[] olds = oldVersion.Split(strSplit);
                if (news.Length == olds.Length)
                {
                    for (var i = 0; i < olds.Length; i++)
                    {
                        int intnew;
                        int intold;

                        StringBuilder strnew = new StringBuilder();
                        StringBuilder strold = new StringBuilder();
                        char[] newcs = news[i].ToCharArray();
                        char[] oldcs = olds[i].ToCharArray();

                        foreach (char n in newcs)
                        {

                            if ("0123456789".Contains(n))
                            {
                                strnew.Append(n.ToString());
                            }

                        }
                        foreach (char p in oldcs)
                        {
                            if ("0123456789".Contains(p))
                            {
                                strold.Append(p.ToString());
                            }

                        }
                        int.TryParse(strnew.ToString(), out intnew);
                        int.TryParse(strold.ToString(), out intold);
                        if (intnew > intold)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    Log.Logs.WriteLog($"版本号不正确,newVersion:{newVersion},oldVersion:{oldVersion}");
                }
            }
            catch(Exception ex)
            {
                Log.Logs.WriteLog($"版本比较错误：{ex.Message}");
            }
            return false;

        }

        public bool PatchRule(string newVersion, string oldVersion)
        {
            if (string.IsNullOrEmpty(newVersion))
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldVersion))
            {
                return true;
            }
            try
            {

                string[] news = newVersion.Split(strSplit);
                string[] olds = oldVersion.Split(strSplit);
                if (news.Length == olds.Length)
                {
                    for (var i = 0; i < olds.Length; i++)
                    {
                        int intnew;
                        int intold;

                        StringBuilder strnew = new StringBuilder();
                        StringBuilder strold = new StringBuilder();
                        char[] newcs = news[i].ToCharArray();
                        char[] oldcs = olds[i].ToCharArray();

                        foreach (char n in newcs)
                        {

                            if ("0123456789".Contains(n))
                            {
                                strnew.Append(n.ToString());
                            }

                        }
                        foreach (char o in oldcs)
                        {
                            if ("0123456789".Contains(o))
                            {
                                strold.Append(o.ToString());
                            }

                        }
                        int.TryParse(strnew.ToString(), out intnew);
                        int.TryParse(strold.ToString(), out intold);
                        if (intnew > intold)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    Log.Logs.WriteLog($"版本号不正确,newVersion:{newVersion},oldVersion:{oldVersion}");
                }
            }
            catch(Exception ex)
            {
                Log.Logs.WriteLog($"版本比较错误：{ex.Message}");
            }
            return false;

        }
    }
}
