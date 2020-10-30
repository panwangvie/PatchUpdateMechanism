using PatchUpdate.ftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateFile;
using UpdateFile.Interface;
using UpdateFile.Log;

namespace PatchUpdate
{
    public delegate void DownloadInvoker(string szMessage);

    /// <summary>
    /// 开始下载补丁
    /// </summary>
    public class StartUpdatePatch
    {
        //下载情况
        public event DownloadInvoker DownloadEvent;

        /// <summary>
        /// 操作日志事件
        /// </summary>
        public event MessageInvoker MessageEvent;
        /// <summary>
        /// 进度事件
        /// </summary>
        public event FileTranProgress OnFileTranProgress;

        /// <summary>
        /// 解压进度
        /// </summary>
        public event UnZipInvoker UnZipEvent;

        /// <summary>
        /// 补丁信息事件
        /// </summary>
        public event VersionInvoker VersionEvent;

        /// <summary>
        /// 下载补丁文件并更新
        /// </summary>
        /// <param name="ftpInfo"></param>
        /// <param name="version">本地的补丁版本</param>
        public void DownLoadPatch(FtpInfo ftpInfo,string strExeName, VersionInfo version = null)
        {
            CSharpFtpClient ftp = new CSharpFtpClient(ftpInfo.Host, ftpInfo.Port, ftpInfo.UserName, ftpInfo.Passwd);

            ftp.MessageEvent += MessageEvent;
            ftp.OnFileTranProgress += OnFileTranProgress;
            var isconnect = ftp.Connect();
            if (isconnect)
            {

                string[] names = ftp.ListDirectory(PathConfig.FtpPatchPath+"/"+ strExeName);

                List<string> patchList = Sorting(names, version?.PatchName);

                foreach (string patch in patchList)
                {
                    DownloadEvent?.Invoke($"开始更新补丁{patch}");
                    if (ftp.DownLoadFile(PathConfig.LoadPatchPath, patch))
                    {
                        try
                        {
                            IOZipHelper zipHelper = new IOZipHelper();

                            zipHelper.UnZipEvent += UnZipEvent;
                            zipHelper.VersionEvent += VersionEvent;

                            string patchName = PathConfig.LoadPatchPath + patch;
                            zipHelper.PatchUnZip(patchName, PathConfig.Load);

                            //解压完之后删除
                            if (File.Exists(patchName))
                            {
                                File.Delete(patchName);
                                Logs.WriteLog($"Delete:{patchName} ", PathConfig.UpdateLog);
                            }

                            zipHelper.UnZipEvent -= UnZipEvent;
                            zipHelper.VersionEvent -= VersionEvent;

                        }
                        catch (Exception ex)
                        {
                            Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");

                        }
                    }
                    else
                    {
                        Logs.WriteLog($"下载补丁{patch}失败");

                        DownloadEvent?.Invoke("下载补丁{patchList[i]}失败");
                    }

                }

                DownloadEvent?.Invoke("补丁下载更新完成");

            }
            else
            {
                Logs.WriteLog($"连接FTP服务失败 ");
                DownloadEvent?.Invoke("连接服务失败");
            }
            
            ftp.MessageEvent -= MessageEvent;
            ftp.OnFileTranProgress -= OnFileTranProgress;
            //注销
            ftp.LogOut();
            ftp.Dispose();

            if (isconnect)
            {
                DelPatchPath();
            }
        }

        /// <summary>
        /// 更新完之后进行更新补丁程序相关文件
        /// </summary>
        public void DelPatchPath()
        {
            try
            {
                //下载更新完之后生成bat和vbs文件用于跟新生成
                if (!Directory.Exists(PathConfig.LoadPatchPath))
                {
                    return;
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(PathConfig.LoadPatchPath);
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                if (fileInfos.Length > 0)
                {
                    string[] copys = new string[fileInfos.Length];
                    string[] dels = new string[fileInfos.Length];
                    for (var i = 0; i < fileInfos.Length; i++)
                    {
                        copys[i] = $"{fileInfos[i].FullName} {PathConfig.Load + fileInfos[i].Name}";
                        dels[i] = fileInfos[i].FullName;
                    }
                    bool isBat = MathBatVbs.MathBat(copys, dels);
                    if (isBat)
                    {
                        try
                        {
                            string batPath = AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.bat";
                            string r = File.ReadAllText(batPath);
                            ProcessInfo.Start(batPath);
                            //关掉此程序
                            Process.GetCurrentProcess().Kill();
                            //Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public List<string> Sorting(string[] vers,string ver)
        {
         
            List<string> vs = new List<string>();
            if (vers != null)
            {
                for (var i = 0; i < vers.Length; i++)
                {
                    if (new ComVersionConfig().PatchRule(vers[i], ver)&& (vers[i].ToLower().EndsWith(".zip")|| vers[i].ToLower().EndsWith(".ph")))
                    {
                        if (vs.Count == 0)
                        {
                            vs.Add(vers[i]);
                        }
                        else
                        {
                            for (var j = 0; j < vs.Count; j++)
                            {
                                if (new ComVersionConfig().PatchRule(vers[i], vs[j]))
                                {

                                    if (j + 1 == vs.Count)
                                    {
                                        vs.Add(vers[i]);
                                        break;
                                    }
                                    else if (!new ComVersionConfig().PatchRule(vers[i], vs[j + 1]))
                                    {
                                        vs.Insert(j + 1, vers[i]);
                                        break;
                                    }
                                }
                                else
                                {

                                    vs.Insert(0, vers[i]);
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            return vs;
        }
    }
}
