using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateFile
{
    public class UpdateService
    {
        //// Token: 0x14000001 RID: 1
        //// (add) Token: 0x06000055 RID: 85 RVA: 0x00004EF8 File Offset: 0x000030F8
        //// (remove) Token: 0x06000056 RID: 86 RVA: 0x00004F30 File Offset: 0x00003130
        //public event UpdateMsg OnUpdateMsg;

        //// Token: 0x14000002 RID: 2
        //// (add) Token: 0x06000057 RID: 87 RVA: 0x00004F68 File Offset: 0x00003168
        //// (remove) Token: 0x06000058 RID: 88 RVA: 0x00004FA0 File Offset: 0x000031A0
        //public event UpdateResult OnUpdateResult;

        //// Token: 0x14000003 RID: 3
        //// (add) Token: 0x06000059 RID: 89 RVA: 0x00004FD8 File Offset: 0x000031D8
        //// (remove) Token: 0x0600005A RID: 90 RVA: 0x00005010 File Offset: 0x00003210
        //public event UpdateProgress OnUpdateProgress;

        //// Token: 0x14000004 RID: 4
        //// (add) Token: 0x0600005B RID: 91 RVA: 0x00005048 File Offset: 0x00003248
        //// (remove) Token: 0x0600005C RID: 92 RVA: 0x00005080 File Offset: 0x00003280
        //public event UpdateRequest OnUpdateRequest;

        // Token: 0x0600005D RID: 93 RVA: 0x000050B8 File Offset: 0x000032B8
        //public bool CheckVersion(string server, string client, out int updateModel)
        //{
        //    updateModel = 0;
        //    if (string.IsNullOrEmpty(client))
        //    {
        //        VersionInfo localVersionInfo = this.GetLocalVersionInfo();
        //        if (localVersionInfo == null)
        //        {
        //            return false;
        //        }
        //        client = localVersionInfo.App;
        //    }
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    bool result;
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        string text = streamReader.ReadLine();
        //        string value = string.Format("c-update?server={0}&client={1}", server, client);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        for (; ; )
        //        {
        //            text = streamReader.ReadLine();
        //            if (text.StartsWith("c-update?update=true"))
        //            {
        //                break;
        //            }
        //            if (text.StartsWith("c-update?update=false"))
        //            {
        //                goto Block_7;
        //            }
        //        }
        //        string text2 = text;
        //        updateModel = int.Parse(text2.Substring(text2.Length - 1));
        //        streamWriter.WriteLine("n");
        //        streamWriter.Flush();
        //        return true;
        //    Block_7:
        //        result = false;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //    return result;
        //}

        //// Token: 0x0600005E RID: 94 RVA: 0x00005264 File Offset: 0x00003464
        //public void Update(string server, string client)
        //{
        //    if (string.IsNullOrEmpty(client))
        //    {
        //        VersionInfo localVersionInfo = this.GetLocalVersionInfo();
        //        if (localVersionInfo == null)
        //        {
        //            return;
        //        }
        //        client = localVersionInfo.App;
        //    }
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        string text = streamReader.ReadLine();
        //        string value = string.Format("u-update?server={0}&client={1}", server, client);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        for (; ; )
        //        {
        //            text = streamReader.ReadLine();
        //            if (text.StartsWith("[I]:"))
        //            {
        //                if (this.OnUpdateMsg != null)
        //                {
        //                    this.OnUpdateMsg(text.Remove(0, 4));
        //                }
        //            }
        //            else if (text.StartsWith("[E]:"))
        //            {
        //                if (this.OnUpdateMsg != null)
        //                {
        //                    this.OnUpdateMsg(text.Remove(0, 4));
        //                }
        //            }
        //            else if (text.StartsWith("[P]:"))
        //            {
        //                if (this.OnUpdateProgress != null)
        //                {
        //                    this.OnUpdateProgress(int.Parse(text.Substring(4)));
        //                }
        //            }
        //            else
        //            {
        //                if (text == "u-update?appupdate=false")
        //                {
        //                    break;
        //                }
        //                if (text == "u-update?appupdate=true")
        //                {
        //                    goto Block_14;
        //                }
        //                if (text.StartsWith("close app") && this.OnUpdateRequest != null)
        //                {
        //                    string msg = "需要关闭程序[" + text.Replace("close app", "").Replace("continue update?", "").Trim() + "]";
        //                    if (this.OnUpdateRequest(msg) == "y")
        //                    {
        //                        streamWriter.WriteLine("y");
        //                        streamWriter.Flush();
        //                    }
        //                    else
        //                    {
        //                        streamWriter.WriteLine("n");
        //                        streamWriter.WriteLine("quit");
        //                        streamWriter.Flush();
        //                    }
        //                }
        //            }
        //        }
        //        if (this.OnUpdateResult != null)
        //        {
        //            this.OnUpdateResult("false");
        //            goto IL_274;
        //        }
        //        goto IL_274;
        //    Block_14:
        //        if (this.OnUpdateResult != null)
        //        {
        //            this.OnUpdateResult("true");
        //        }
        //    IL_274:;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //}

        //// Token: 0x0600005F RID: 95 RVA: 0x0000555C File Offset: 0x0000375C
        //public bool MakePatch(string src, string dest)
        //{
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    bool result;
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        string a = streamReader.ReadLine();
        //        string value = string.Format("c-compress?src={0}&dest={1}", src, dest);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        for (; ; )
        //        {
        //            a = streamReader.ReadLine();
        //            if (a == "c-compress?compress=true")
        //            {
        //                break;
        //            }
        //            if (a == "c-compress?compress=false")
        //            {
        //                goto Block_5;
        //            }
        //        }
        //        return true;
        //    Block_5:
        //        result = false;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //    return result;
        //}

        //// Token: 0x06000060 RID: 96 RVA: 0x000056BC File Offset: 0x000038BC
        //public bool UploadPatch(string patchName, string server)
        //{
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    bool result;
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        string a = streamReader.ReadLine();
        //        string value = string.Format("u-uploadpatch?server={0}&filename={1}", server, patchName);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        for (; ; )
        //        {
        //            a = streamReader.ReadLine();
        //            if (a == "u-uploadpatch?upload=true")
        //            {
        //                break;
        //            }
        //            if (a == "u-uploadpatch?upload=false")
        //            {
        //                goto Block_5;
        //            }
        //        }
        //        return true;
        //    Block_5:
        //        result = false;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //    return result;
        //}

        //// Token: 0x06000061 RID: 97 RVA: 0x0000581C File Offset: 0x00003A1C
        //public string GetLastPatchInfo(string server, string client)
        //{
        //    if (string.IsNullOrEmpty(client))
        //    {
        //        VersionInfo localVersionInfo = this.GetLocalVersionInfo();
        //        if (localVersionInfo == null)
        //        {
        //            return "";
        //        }
        //        client = localVersionInfo.App;
        //    }
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    string result;
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        streamReader.ReadLine();
        //        string value = string.Format("l-patchinfo?server={0}&client={1}", server, client);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        result = streamReader.ReadLine();
        //    }
        //    catch
        //    {
        //        result = "";
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //    return result;
        //}

        //// Token: 0x06000062 RID: 98 RVA: 0x00005980 File Offset: 0x00003B80
        //public VersionInfo GetLocalVersionInfo()
        //{
        //    VersionInfo result = null;
        //    if (!File.Exists("update.json"))
        //    {
        //        return result;
        //    }
        //    using (StreamReader streamReader = new StreamReader("update.json", Encoding.UTF8))
        //    {
        //        result = (ParseJson.DeserializeObject(streamReader.ReadToEnd(), typeof(VersionInfo)) as VersionInfo);
        //    }
        //    return result;
        //}

        //// Token: 0x06000063 RID: 99 RVA: 0x000059E8 File Offset: 0x00003BE8
        //public bool DownloadHotfix(string server)
        //{
        //    Process process = new Process();
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardInput = true;
        //    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
        //    process.StartInfo.Arguments = "External call";
        //    process.Start();
        //    StreamReader streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
        //    bool result;
        //    try
        //    {
        //        streamWriter.WriteLine("hello");
        //        streamWriter.Flush();
        //        string a = streamReader.ReadLine();
        //        string value = string.Format("d-downloadhotfix?server={0}", server);
        //        streamWriter.WriteLine(value);
        //        streamWriter.Flush();
        //        for (; ; )
        //        {
        //            a = streamReader.ReadLine();
        //            if (a == "d-downloadhotfix?hotfix=true")
        //            {
        //                break;
        //            }
        //            if (a == "d-downloadhotfix?hotfix=false")
        //            {
        //                goto Block_5;
        //            }
        //        }
        //        return true;
        //    Block_5:
        //        result = false;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        streamWriter.WriteLine("quit");
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //        streamWriter.Dispose();
        //        streamReader.Close();
        //        streamReader.Dispose();
        //        process.WaitForExit();
        //        process.Close();
        //        process.Dispose();
        //    }
        //    return result;
        //}

        //// Token: 0x0400004C RID: 76
        //private const string CheckFalse = "c-update?update=false";

        //// Token: 0x0400004D RID: 77
        //private const string CheckTrue = "c-update?update=true";

        //// Token: 0x0400004E RID: 78
        //private const string CompressTrue = "c-compress?compress=true";

        //// Token: 0x0400004F RID: 79
        //private const string CompressFalse = "c-compress?compress=false";

        //// Token: 0x04000050 RID: 80
        //private const string UpdateFalse = "u-update?appupdate=false";

        //// Token: 0x04000051 RID: 81
        //private const string UpdateTrue = "u-update?appupdate=true";

        //// Token: 0x04000052 RID: 82
        //private const string UploadFalse = "u-uploadpatch?upload=false";

        //// Token: 0x04000053 RID: 83
        //private const string UploadTrue = "u-uploadpatch?upload=true";
    }
}
