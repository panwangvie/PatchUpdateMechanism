using PatchUpdate.ftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdateFile.Control;
using UpdateFile.Interface;
using UpdateFile.Log;
using UpdateFile.ViewModel;

namespace UpdateFile
{
    /// <summary>
    /// 通过ftp的方式检测
    /// </summary>
    public class FtpTestMethod 
    {
        FtpInfo  ftpInfo;


        /// <summary>
        ///检查更新,需要使用更新机制的程序调用此方法。
        /// </summary>
        /// <param name="ftpInfo"></param>
        public static void Update(FtpInfo ftpInfo)
        {
            DetectUpdates(ftpInfo);
        }

        /// <summary>
        /// 检测更新方法
        /// </summary>
        /// <param name="ftpInfo"></param>
        private static void DetectUpdates(FtpInfo ftpInfo)
        {
            if (ftpInfo == null)
            {
                Logs.WriteLog("ftp信息为空");
                return;
            }
            IUpdate update = new FtpIsUpdate();

            CSharpFtpClient ftp = new CSharpFtpClient(ftpInfo.Host, ftpInfo.Port, ftpInfo.UserName, ftpInfo.Passwd);
           
            //在ftp中的补丁路径
            string path = PathConfig.FtpPatchPath + "/" + AppDomain.CurrentDomain.FriendlyName.Replace(".exe","");
            ftp.MessageEvent += (msg) =>
            {
                Logs.WriteLog(msg,PathConfig.LoadFtpLogPath);
            };

            if (ftp.Connect())
            {
            
                if (ftp.ChangeDir(path))
                {
                    string[] names = ftp.ListDirectory(path);
                    
                    foreach (var name in names)
                    {
                        if (name.ToLower() == PathConfig.FtpConfigName.ToLower())
                        {
                            bool res = ftp.DownLoadFile(PathConfig.LoadPatchPath, name);
                            if (res)
                            {
                                //注销
                                ftp.LogOut();
                                ftp.Dispose();

                                IComparisonVersion version = new ComVersionConfig();
                                VersionInfo versionInfo = new VersionInfo();
                                if (version.IsSameVersion(ref versionInfo))
                                {
                                    try
                                    {
                                        string verName = PathConfig.LoadPatchPath + name;
                                        if (File.Exists(verName))
                                        {
                                            File.Delete(verName);
                                            Logs.WriteLog($"Delete:{verName} ", PathConfig.UpdateLog);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");
                                    }

                                    UpdateMag(versionInfo, ftpInfo);
                                }
                                else
                                {
                                    try
                                    {
                                        string verName = PathConfig.LoadPatchPath + name;
                                        if (File.Exists(verName))
                                        {
                                            File.Delete(verName);
                                            Logs.WriteLog($"Delete:{verName} ", PathConfig.UpdateLog);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace} ");
                                    }
                                    return;
                                }
                            }

                        }
                       
                    }
                }
               
            }
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">版本信息</param>
        /// <param name="info">登录信息</param>
        private static void UpdateMag(VersionInfo version, FtpInfo info)
        {
            var yes = MessageBox.Show($"检测到有新的版本，是否要更新\n更新内容：\n{version.UpdateDesc}", "更新", MessageBoxButton.YesNo);
            MessgerInfo messgerInfo = null;
            VmMsgInfo vmInfo = new VmMsgInfo();
            vmInfo.UpdateInfo = version.UpdateDesc;
            vmInfo.VerInfo = version.Version;

            if (version.App.Contains("SEngine"))
            {
                messgerInfo=new MessgerInfo("ArStyle");
            }
            else
            {
                messgerInfo = new MessgerInfo("MeetStyle");
            }
          

            messgerInfo.YesAction += () => {
                Log.Logs.WriteLog($"开始更新", PathConfig.UpdateLog);
                ///开始更新
                IStartUpdate start = new StartUpdateClient();
                start.StartUpdate(info, version);
            };
            messgerInfo.NoAction += () => {

                if (version.Model == 1)
                {
                    ///强制更新
                    Environment.Exit(0);
                }

            };
            messgerInfo.Topmost = true;
            messgerInfo.ShowDialog();



        }
    }
}
