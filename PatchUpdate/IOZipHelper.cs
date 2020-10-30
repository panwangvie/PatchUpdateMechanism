using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdateFile;
using UpdateFile.Log;

namespace PatchUpdate
{
    public delegate void UnZipInvoker(int speed);
    public delegate void VersionInvoker(VersionInfo version);

    public class IOZipHelper
    {
        /// <summary>
        /// 解压进度
        /// </summary>
        public event UnZipInvoker UnZipEvent;

        /// <summary>
        /// 补丁信息事件
        /// </summary>
        public event VersionInvoker VersionEvent;

        /// <summary>
        /// /更新程序依赖文件
        /// </summary>
        public static List<string> DependentFiles=new List<string> { "UpdateFile.dll" , AppDomain.CurrentDomain.SetupInformation.ApplicationName } ;
        /// <summary>
        /// 解压补丁包
        /// </summary>
        /// <param name="FileToUpZip">补丁完整路径</param>
        /// <param name="ZipedFolder">加压到的目录</param>
        public  void PatchUnZip(string FileToUpZip, string ZipedFolder)
        {
            if (!File.Exists(FileToUpZip))
            {
                return;
            }

            try
            {
                if (!Directory.Exists(ZipedFolder))
                {
                    Directory.CreateDirectory(ZipedFolder);
                }
                //文件跟新模式集合
                List<FileOperator> files = new List<FileOperator>();
                //版本信息
                VersionInfo versionInfo = new VersionInfo();
                //获取文件大小
                FileInfo fileInfo = new FileInfo(FileToUpZip);
                long totalLength = fileInfo.Length;

                using (ZipArchive archive = ZipFile.OpenRead(FileToUpZip))
                {

                   
                    string aa = $"{Path.GetFileNameWithoutExtension(FileToUpZip)}/{PathConfig.ConfigName}";
                    //获取VersionInfo.config来显示更新的信息
                    ZipArchiveEntry configEntry = archive.GetEntry($"{PathConfig.ConfigName}");
                    if (configEntry != null)
                    {
                        using (Stream stream = configEntry.Open())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string text = reader.ReadToEnd();
                                versionInfo = Json.Parse<VersionInfo>(text);
                                VersionEvent?.Invoke(versionInfo);
                            }
                        }
                    }

                    //获取fileinfo.json来判断补丁文件的更新模式
                    ZipArchiveEntry archiveEntry = archive.GetEntry(PathConfig.FileJson);
                    if (archiveEntry != null)
                    {
                        using (Stream stream = archiveEntry.Open())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string text = reader.ReadToEnd();
                                files = Json.Parse<List<FileOperator>>(text);

                            }
                        }
                    }

                    long unZip = 0;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.FullName))
                        {
                            string fullName = entry.FullName;
                            ///去掉路径前的压缩文件名
                         

                            //复制的新路径
                            string newFile = ZipedFolder + fullName;

                            try
                            {
                                if (string.IsNullOrEmpty(fullName))
                                {
                                    continue;
                                }

                                //若是不是文件则创建文件
                                if (!Path.HasExtension(newFile))
                                {
                                    Directory.CreateDirectory(newFile);
                                    continue;
                                }

                                //获取该文件的更新模式,若是获取为空则默认为覆盖模式
                                FileOperator fileOperator = files.Find(x => { return fullName == x.FileName; });

                                //若是更新程序相关文件则跳过
                                if (DependentFiles.Contains(Path.GetFileName(newFile)))
                                {

                                    //若是更新程序的依赖库则复制到PathConfig.LoadPatchPath ，再使用bat文件更新
                                    using (Stream open = entry.Open())
                                    {
                                        using (FileStream fs = new FileStream(PathConfig.LoadPatchPath + Path.GetFileName(newFile), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            open.CopyTo(fs);
                                            Logs.WriteLog($"CopyTo:{PathConfig.LoadPatchPath + Path.GetFileName(newFile)} ", PathConfig.UpdateLog);
                                        }
                                    }
                                    Logs.WriteLog($"continue :{newFile} ", PathConfig.UpdateLog);
                                    continue;
                                }

                                if (fileOperator?.OperatorType == OperatorTypeEnum.Delete)
                                {

                                    //若是文件则删除文件
                                    if (File.Exists(newFile))
                                    {
                                        File.Delete(newFile);
                                        Logs.WriteLog($"Delete:{newFile} ", PathConfig.UpdateLog);
                                    }
                                  

                                }
                                else if (fileOperator?.OperatorType == OperatorTypeEnum.Ignore)
                                {
                                    Logs.WriteLog($"Ignore :{newFile} ", PathConfig.UpdateLog);
                                }
                                else
                                {
                                    ///无文件夹则创建文件夹
                                    if (!Directory.Exists(Path.GetPathRoot(newFile)))
                                    {
                                        Directory.CreateDirectory(Path.GetPathRoot(newFile));
                                    }
                                    //覆盖文件
                                    using (Stream open = entry.Open())
                                    {
                                        using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                        {
                                            open.CopyTo(fs);
                                            Logs.WriteLog($"CopyTo:{newFile} ", PathConfig.UpdateLog);
                                        }
                                    }
                                }
                                unZip += entry.CompressedLength;
                                UnZipEvent?.Invoke((int)(unZip * 100 / totalLength));

                            }
                            catch (IOException ioex)
                            {
                                Logs.WriteLog($"补丁文件{newFile}更新异常,Message:{ioex.Message},StackTrace:{ioex.StackTrace}", PathConfig.UpdateLog);

                                var res = MessageBox.Show($"{newFile}被其他进程占用，正在关闭该进程重新更新", "意外", MessageBoxButton.OK);

                                bool isKill = KillFile(newFile);
                                try
                                {
                                    if (isKill)
                                    {
                                        Logs.WriteLog($"Kill:{isKill} ", PathConfig.UpdateLog);

                                        using (Stream open = entry.Open())
                                        {
                                            using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                            {
                                                open.CopyTo(fs);
                                                Logs.WriteLog($"CopyTo:{newFile} ", PathConfig.UpdateLog);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Logs.WriteLog($"补丁文件{newFile}更新失败,Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);

                                }


                            }
                            catch (UnauthorizedAccessException ex)
                            {

                                Logs.WriteLog($"补丁文件更新异常,Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
                                bool isKill = KillFile(newFile);
                                try
                                {
                                    if (isKill)
                                    {
                                        if (File.Exists(newFile))
                                        {
                                            File.Delete(newFile);
                                            Logs.WriteLog($"Delete:{newFile} ", PathConfig.UpdateLog);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logs.WriteLog($"补丁文件{newFile}更新失败,Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
                                }
                            }
                            catch (Exception ex)
                            {

                                Logs.WriteLog($"补丁文件{newFile}更新失败,Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
                            }

                        }

                    }

                    //完成
                    UnZipEvent?.Invoke(1);
                }

            }
            catch (Exception ex)
            {
                 Logs.WriteLog($"补丁文件更新失败,Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);

            }
        }

        /// <summary>
        /// 结束某个进程
        /// </summary>
        /// <param name="newFile"></param>
        /// <returns></returns>
        private  bool KillFile(string newFile)
        {
            ProcessInfo process = new ProcessInfo();
            bool isKill = false;
            if (newFile.ToLower().EndsWith(".exe"))
            {
                isKill = process.KissByProcess(Path.GetFileNameWithoutExtension(newFile));
            }
            else
            {
                isKill = process.KillProcess(newFile);
            }
            return isKill;
        }
    }
}

 