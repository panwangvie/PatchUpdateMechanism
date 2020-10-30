using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchUpdate
{
    public class HotfixMath
    {
        public bool MakePatch(string src, string dest)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\hotfix.exe";
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
                for (; ; )
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
                return true;
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
