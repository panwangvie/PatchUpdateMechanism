using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace MathHotfix
{
    public delegate void UpdateMsg(string msg);
    public delegate void UpdateProgress(int progress);
    public delegate string UpdateRequest(string msg);
    public delegate void UpdateResult(string result);

    /// <summary>
    /// 制作补丁的
    /// </summary>
    public class UpdateService
    {
        private const string CheckFalse = "c-update?update=false";
        private const string CheckTrue = "c-update?update=true";
        private const string CompressTrue = "c-compress?compress=true";
        private const string CompressFalse = "c-compress?compress=false";
        private const string UpdateFalse = "u-update?appupdate=false";
        private const string UpdateTrue = "u-update?appupdate=true";
        private const string UploadFalse = "u-uploadpatch?upload=false";
        private const string UploadTrue = "u-uploadpatch?upload=true";
        public event UpdateMsg OnUpdateMsg;

        public event UpdateResult OnUpdateResult;

        public event UpdateProgress OnUpdateProgress;

        public event UpdateRequest OnUpdateRequest;
       
        public bool CheckVersion(string server, string client, out int updateModel)
        {
            updateModel = 0;
            if (string.IsNullOrEmpty(client))
            {
                VersionInfo localVersionInfo = this.GetLocalVersionInfo();
                if (localVersionInfo == null)
                {
                    return false;
                }
                client = localVersionInfo.App;
            }
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            bool result;
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                string text = streamReader.ReadLine();
                string value = string.Format("c-update?server={0}&client={1}", server, client);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                while (true)
                {
                    text = streamReader.ReadLine();
                    if (text.StartsWith("c-update?update=true"))
                    {
                        break;
                    }
                    if (text.StartsWith("c-update?update=false"))
                    {
                        goto Block_7;
                    }
                }
                string expr_100 = text;
                updateModel = int.Parse(expr_100.Substring(expr_100.Length - 1));
                streamWriter.WriteLine("n");
                streamWriter.Flush();
                result = true;
                return result;
            Block_7:
                result = false;
            }
            catch
            {
                result = false;
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            return result;
        }
        public void Update(string server, string client)
        {
            if (string.IsNullOrEmpty(client))
            {
                VersionInfo localVersionInfo = this.GetLocalVersionInfo();
                if (localVersionInfo == null)
                {
                    return;
                }
                client = localVersionInfo.App;
            }
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                string text = streamReader.ReadLine();
                string value = string.Format("u-update?server={0}&client={1}", server, client);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                while (true)
                {
                    text = streamReader.ReadLine();
                    if (text.StartsWith("[I]:"))
                    {
                        if (this.OnUpdateMsg != null)
                        {
                            this.OnUpdateMsg(text.Remove(0, 4));
                        }
                    }
                    else
                    {
                        if (text.StartsWith("[E]:"))
                        {
                            if (this.OnUpdateMsg != null)
                            {
                                this.OnUpdateMsg(text.Remove(0, 4));
                            }
                        }
                        else
                        {
                            if (text.StartsWith("[P]:"))
                            {
                                if (this.OnUpdateProgress != null)
                                {
                                    this.OnUpdateProgress(int.Parse(text.Substring(4)));
                                }
                            }
                            else
                            {
                                if (text == "u-update?appupdate=false")
                                {
                                    break;
                                }
                                if (text == "u-update?appupdate=true")
                                {
                                    goto Block_14;
                                }
                                if (text.StartsWith("close app") && this.OnUpdateRequest != null)
                                {
                                    string msg = "需要关闭程序[" + text.Replace("close app", "").Replace("continue update?", "").Trim() + "]";
                                    if (this.OnUpdateRequest(msg) == "y")
                                    {
                                        streamWriter.WriteLine("y");
                                        streamWriter.Flush();
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("n");
                                        streamWriter.WriteLine("quit");
                                        streamWriter.Flush();
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.OnUpdateResult != null)
                {
                    this.OnUpdateResult("false");
                    goto IL_274;
                }
                goto IL_274;
            Block_14:
                if (this.OnUpdateResult != null)
                {
                    this.OnUpdateResult("true");
                }
            IL_274:;
            }
            catch (Exception)
            {
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
        }
        public bool MakePatch(string src, string dest)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            bool result;
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                string a = streamReader.ReadLine();
                string value = string.Format("c-compress?src={0}&dest={1}", src, dest);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                while (true)
                {
                    a = streamReader.ReadLine();
                    if (a == "c-compress?compress=true")
                    {
                        break;
                    }
                    if (a == "c-compress?compress=false")
                    {
                        goto Block_5;
                    }
                }
                result = true;
                return result;
            Block_5:
                result = false;
            }
            catch
            {
                result = false;
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            return result;
        }
        public bool UploadPatch(string patchName, string server)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            bool result;
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                string a = streamReader.ReadLine();
                string value = string.Format("u-uploadpatch?server={0}&filename={1}", server, patchName);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                while (true)
                {
                    a = streamReader.ReadLine();
                    if (a == "u-uploadpatch?upload=true")
                    {
                        break;
                    }
                    if (a == "u-uploadpatch?upload=false")
                    {
                        goto Block_5;
                    }
                }
                result = true;
                return result;
            Block_5:
                result = false;
            }
            catch
            {
                result = false;
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            return result;
        }
        public string GetLastPatchInfo(string server, string client)
        {
            if (string.IsNullOrEmpty(client))
            {
                VersionInfo localVersionInfo = this.GetLocalVersionInfo();
                if (localVersionInfo == null)
                {
                    return "";
                }
                client = localVersionInfo.App;
            }
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            string result;
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                streamReader.ReadLine();
                string value = string.Format("l-patchinfo?server={0}&client={1}", server, client);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                result = streamReader.ReadLine();
            }
            catch
            {
                result = "";
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            return result;
        }
        public VersionInfo GetLocalVersionInfo()
        {
            VersionInfo result = null;
            if (!File.Exists("update.json"))
            {
                return result;
            }
            using (StreamReader streamReader = new StreamReader("update.json", Encoding.UTF8))
            {
                result = (ParseJson.DeserializeObject(streamReader.ReadToEnd(), typeof(VersionInfo)) as VersionInfo);

                Json.Parse<VersionInfo>(streamReader.ReadToEnd());
            }
            return result;
        }
        public bool DownloadHotfix(string server)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = Application.StartupPath + "\\hotfix.exe";
            process.StartInfo.Arguments = "External call";
            process.Start();
            StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
            bool result;
            try
            {
                streamWriter.WriteLine("hello");
                streamWriter.Flush();
                string a = streamReader.ReadLine();
                string value = string.Format("d-downloadhotfix?server={0}", server);
                streamWriter.WriteLine(value);
                streamWriter.Flush();
                while (true)
                {
                    a = streamReader.ReadLine();
                    if (a == "d-downloadhotfix?hotfix=true")
                    {
                        break;
                    }
                    if (a == "d-downloadhotfix?hotfix=false")
                    {
                        goto Block_5;
                    }
                }
                result = true;
                return result;
            Block_5:
                result = false;
            }
            catch
            {
                result = false;
            }
            finally
            {
                streamWriter.WriteLine("quit");
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                streamReader.Close();
                streamReader.Dispose();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            return result;
        }
    }
}