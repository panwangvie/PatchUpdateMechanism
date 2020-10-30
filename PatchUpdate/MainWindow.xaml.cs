using PatchUpdate.ftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using UpdateFile;
using UpdateFile.Control;
using UpdateFile.Log;
using UpdateFile.ViewModel;

namespace PatchUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(StartupEventArgs e)
        {
            InitializeComponent();
            string[] args = e.Args;
            if (args?.Length > 0)
            {

                System.IO.File.WriteAllLines($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}my.txt", args);

            }
            else
            {
                System.IO.File.WriteAllLines($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}my.txt", new string[] { $"没有，{DateTime.Now.ToString("yyyyMMMdd:HH:mm:ss")}" });

            }
            FtpInfo ftpInfo = new FtpInfo();
            VersionInfo version;
            if (args.Length == 4)
            {
                ftpInfo.Host = args[0];
                int port;
                int.TryParse(args[1],out port);
                ftpInfo.Port = port;

                ftpInfo.UserName = args[2];
                ftpInfo.Passwd = args[3];
            }
            if (args.Length == 5)
            {
                ftpInfo.PasswdMd5 = args[4];
            }
            if (args.Length == 7)
            {
                FtpInfo info = Json.Parse<FtpInfo>(args[5]);
                version = Json.Parse<VersionInfo>(args[6]);
            }
        }


        CSharpFtpClient cSharpFtpClient;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string host = "192.168.1.85";
            string userName = "ftpuser";
            string passwd = "gktel123456";
            string post = "21";

            ///切换至
            string changeDir = "/home/ftpuser/PatchUpdate";

            string uploadPath = "E:/WindowsFormsApp1.zip";
            if (cSharpFtpClient == null)
            {
                cSharpFtpClient = new CSharpFtpClient(host, userName, passwd);
            }
            ///建立连接
            bool isConnect = cSharpFtpClient.Connect();
            if (!isConnect)
            {
                return;
            }

               bool isChange = cSharpFtpClient.ChangeDir(changeDir);
            if (isChange)
            {
                string[] vs=  cSharpFtpClient.ListDirectory(changeDir);
                cSharpFtpClient.UploadFile(uploadPath);
                cSharpFtpClient.DownLoadFile("E:\\Code", "SEngine.exe.config");
            }
                
            

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //上传图片
            OpenFileDialog ofg = new OpenFileDialog();
            ofg.Multiselect = false;

            //modify by j01776 for MPPD22566
            ofg.Filter = "图片文件(*.jpg,*.bmp,*.png)|*.jpg;*.bmp;*.png|所有文件|*.*";
            //ofg.Filter = "JPG图片|*.jpg|BMP图片|*.bmp|PNG图片|*.png|所有文件|*.*";
            //end

            if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                if (!string.IsNullOrEmpty(ofg.FileName))
                {

                }
                string code = DateTime.Now.ToString("yyyyMMddHHmmss");
                bool res = cSharpFtpClient.UploadFile(ofg.FileName, code);

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HotfixMath hotfixMath = new HotfixMath();
            string path = "E:\\Code\\MyARDemo\\WpfApp1\\PatchUpdate\\bin\\Debug";

            hotfixMath.MakePatch(path, path+@"\ISClient-B3323-B3323-y00554-20200826.ph");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FtpInfo ftpInfo = new FtpInfo()
            {
                Host = "192.168.1.85",
                UserName = "ftpuser",
                Passwd = "gktel123456",
                Port = 21
            };
            FtpTestMethod.Update(ftpInfo);

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            FtpTestMethod ftpTestMethod = new FtpTestMethod();
            FtpInfo ftpInfo = new FtpInfo()
            {
                Host = "192.168.1.85",
                UserName = "ftpuser",
                Passwd = "gktel123456",
                Port = 21
            };
            VersionInfo versionInfo = new VersionInfo() { Version = "V1.0-H01-D20200827",PatchName= "ARClient-V1.0-H01-D20200827.ph" };
            //versionInfo.SetXmlConfig(PathConfig.Load+ "VersionInfo.config");


            ComVersionConfig com = new ComVersionConfig();
            
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            FtpInfo ftpInfo = new FtpInfo()
            {
                Host = "192.168.1.85",
                UserName = "ftpuser",
                Passwd = "gktel123456",
                Port = 21
            };

            VersionInfo version = new VersionInfo();
            version.PatchName = "ARClient-V1.2-H01-D20200901.zip";
            string exe = "";

            StartUpdatePatch startUpdate = new StartUpdatePatch();
            startUpdate.DownLoadPatch(ftpInfo, exe, version);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {



            string zipPath = "E:\\ISClient-B3323-H01-y00554-20200831.ph";
            string path = PathConfig.Load + "ARClinet-V1.0-H02-D20200902.zip";
          // UnZipClass.ZipFile(zipPath, "update.json");
          // UnZipClass.UnFiles(zipPath);
             
        }

        private void RefGet()
        {
           

            string path = PathConfig.Load + "UpdateClient.dll";
            string path2 = PathConfig.Load + "UpdateFile.dll";

            Assembly assembly = Assembly.LoadFile(path);
            string name = assembly.FullName;

            byte[] buffer = System.IO.File.ReadAllBytes(path);

            var ect = Assembly.Load(name);//加载程序集，创建程序集里面的 命名空间.类型名 实例

            string nameSpace = Path.GetFileNameWithoutExtension(path);
            var update = ect.CreateInstance($"{nameSpace}.Update",true);
            Type type = update.GetType();
            MethodInfo method = type.GetMethod("GetInfo");
            MethodInfo method1 = type.GetMethod("ResFile");
            MethodInfo method2 = type.GetMethod("DelFile");

            string source = @"E:\Code\PatchUpdateMechanism\UpdateClient\bin\Debug\UpdateClient.dll";
            string toPath = PathConfig.Load+ @"UpdateClient.dll";
            //UpdateClient.Update update1 = new UpdateClient.Update();
            //update1.ResFile(toPath, source);
            //update1.DelFile(toPath);
            var res = method.Invoke(update,new object[]{ });
            var res1 = method1.Invoke(update,new object[]{ toPath, source });
            var res2 = method2.Invoke(update,new object[]{ toPath });


            //Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //Type type = assembly.GetType("Dictionary.Class.S0002.StudentDict");     //命名空间名 + 类名
            //object obj = Activator.CreateInstance(type, true);

            //try
            //{
            //    FieldInfo classField = type.GetField("ClassName");
            //    Console.WriteLine("班级名称：" + classField.GetValue(obj).ToString());
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("\t班级名称获取失败：" + ex.Message);
            //}
        }

        private void RefByte()
        {
            string assemblyName = "AssemblyTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=fffb45e56dd478e3";

            //Assembly ass = Assembly.ReflectionOnlyLoad(assemblyName);
            string path = PathConfig.Load + "UpdateClient.dll";
            string path2 = PathConfig.Load + "UpdateFile.dll";
            byte[] buffer = System.IO.File.ReadAllBytes(path2);

            Assembly assembly = Assembly.Load(buffer);
        }

        /// <summary>
        /// 生成Vbs文件
        /// </summary>
        private void MathVbs(string batPath)
        {
            try
            {
                System.IO.StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.vbs", false);
                string text1 = "set ws = WScript.CreateObject(\"WScript.Shell\")";
                string text2 = $"ws.Run \"{batPath}\",0";
                stream.WriteLine(text1);
                stream.WriteLine(text2);

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();

                }
            }
            catch(Exception ex)
            {
            }
            

        }

        /// <summary>
        /// 生成bat文件
        /// </summary>
        private void MathBat(string newPath,string oldPath)
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
            }
            catch (Exception ex)
            {
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {


            string fileName = $"{PathConfig.Load}UpdateFile.dll";//要检查被那个进程占用的文件

            Process tool = new Process();
            tool.StartInfo.FileName = "handle64.exe";
            tool.StartInfo.Arguments = fileName + " /accepteula";
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.Start();
            string outputTool = tool.StandardOutput.ReadToEnd();

            string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
             var matches =Regex.Matches(outputTool, matchPattern);
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                Process.GetProcessById(int.Parse(match.Value)).Kill();
            }

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("版本信息：V1.0-H04\n更新内容：1.增加录屏功能", "检查到新版本", MessageBoxButton.YesNo);
            RefGet();
           // RefByte();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            StartUpdatePatch startUpdate = new StartUpdatePatch();
       
            string[] vs = new string[] { "ARClinet-V1.0-H02-D20200902.zip", "ARClinet-V1.0-H01-D20200902.zip", "ARClinet-V1.0-H08-D20200902.zip", "ARClinet-V1.0-H06-D20200902.zip", "ARClinet-V1.0-H07-D20200902.zip", "ARClinet-V1.0-H04-D20200902.zip", "ARClinet-V1.0-H05-D20200902.zip", "ARClinet-V1.0-H05-D20200902.zip" , "ARClinet-V1.0-H03-D20200902.zip" };
            string ver = "ARClinet-V1.0-H03-D20200902.zip";
            List<string> aa= startUpdate.Sorting(vs, ver);
            List<string> aa2=startUpdate.Sorting(vs, "ARClinet-V1.0-H01-D20200902.zip");
            List<string> aa3=startUpdate.Sorting(vs, "ARClinet-V1.0-H08-D20200902.zip");
            List<string> aa1=startUpdate.Sorting(vs, null);
           startUpdate.DelPatchPath();

            //string batPath = AppDomain.CurrentDomain.BaseDirectory + "UpdateThis.bat";
            //MathVbs(batPath);
            //string oldPath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            //string newPath = PathConfig.LoadPatchPath + AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            //MathBat(newPath, oldPath);
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            string o = @"E:\Code\PatchUpdateMechanism\PatchUpdate\bin\Debug\aa.txt";
            string newFile = @"E:\Code\PatchUpdateMechanism\PatchUpdate\bin\Debug\ttt\aaa\aa.txt";
            //覆盖文件
            using (FileStream open = File.OpenRead(o))
            {
                using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    open.CopyTo(fs);
                    Logs.WriteLog($"CopyTo:{newFile} ", PathConfig.UpdateLog);
                }
            }
        }

        private void Button_Click11(object sender, RoutedEventArgs e)
        {
            MessgerInfo messWin = new MessgerInfo("ArStyle");
            messWin.Topmost = true;
            
            //try
            //{

            //    info.LineBursh = info.StrToBrush("#006FE9");
            //    info.FontBursh = info.StrToBrush("#FFFFFF");
            //    info.TitelBrush = info.StrToBrush("#35FFFA");
            //    var aaa = new BitmapImage(new Uri("pack://application:,,,/images/arImg/背景框.png"));

            //    //string path = "pack://application:,,,/UpdateFile;component/images/arImg/";
            //    string path = "pack://application:,,,/component/images/arImg/";
            //    info.BgImg = info.StrToImg(path + "背景框.png");
            //    info.YesImgHov = info.StrToImg(path + "按钮背景hov.png");
            //    info.YesImgNor = info.StrToImg(path + "按钮背景nor.png");
            //    info.NoImgHov = info.StrToImg(path + "按钮背景hov.png");
            //    info.NoImgNor = info.StrToImg(path + "按钮背景nor.png");

            //    info.CloseImgHov = info.StrToImg(path + "背景框关闭nor.png");
            //    info.CloseImgNor = info.StrToImg(path + "背景框关闭nor.png");
            //}catch(Exception ex)
            //{

            //}
            messWin.ShowDialog();


        }

        private void Button_Click12(object sender, RoutedEventArgs e)
        {
            
            MainUpdate mianUpdate = new MainUpdate(null);

            mianUpdate.Topmost = true;
            mianUpdate.ShowDialog();

        }
    }
}
