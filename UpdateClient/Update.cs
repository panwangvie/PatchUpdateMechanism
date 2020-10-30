using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateClient
{
    public class Update
    {
        public bool GetInfo()
        {
            return true;
        }

        public bool DelFile(string delFile)
        {
            if (File.Exists(delFile))
            {
                try
                {
                    File.Delete(delFile);
                    return true;

                }
                catch (Exception ex)
                {
                    System.IO.StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "aa.txt");
                    stream.WriteLine($"Message:{ex.Message},StackTrace:{ex.StackTrace}");
                    stream.Close();

                }

            }
            return false;
        }

        public bool ResFile(string old,string newFile)
        {
            string va;
            try
            {
                File.Copy(newFile, old, true);
                return true;
            }catch(Exception ex)
            {
                System.IO.StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "aa.txt");
                stream.WriteLine($"Message:{ex.Message},StackTrace:{ex.StackTrace}");
                stream.Close();
            }
            return false;
        }

    }
}
