using PatchUpdate.ftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
using UpdateFile.Log;
using MessageBox = System.Windows.MessageBox;

/**
 * 补丁上传工具，输入上传的服务信息，选择制作的补丁文件上传。
 * */
namespace UpClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static object _lock = new object();
        public MainWindow()
        {
            InitializeComponent();
        }

        bool isCon;
        CSharpFtpClient ftp;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FtpInfo ftpInfo = new FtpInfo();
                ftpInfo.Host = tbIp.Text.Trim();
                ftpInfo.Port = int.Parse(tbPort.Text.Trim());
                ftpInfo.UserName = tbUser.Text.Trim();
                ftpInfo.Passwd = pwbPasswd.Password.Trim();

                ftp = new CSharpFtpClient(ftpInfo.Host, ftpInfo.Port, ftpInfo.UserName, ftpInfo.Passwd);
                ftp.MessageEvent += ShowLog;

                if (!ftp.IsConnect)
                {

                    isCon = ftp.Connect();
                    if (isCon)
                    {
                        btnLogin.Content = "断开连接";
                    }
                }
                else if (ftp.IsConnect)
                {
                    ftp.LogOut();
                    if (!ftp.IsConnect)
                    {
                        btnLogin.Content = "连接";
                    }
                }
            }catch(Exception ex)
            {

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.ph)|*.ph";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbPath.Text = dialog.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string path = this.tbPath.Text.Trim();
            if (!File.Exists(path))
            {
                MessageBox.Show("请选择正确文件");
                return;
            }

            string pathName = Path.GetFileNameWithoutExtension(path);
            
            string exe = pathName.Substring(0, pathName.IndexOf("-"));

            if (ftp.IsConnect)
            {
                string ftpPath = PathConfig.FtpPatchPath + "/" + exe;

                if (ftp.ChangeDir(ftpPath))
                {
                    bool up = UpConfigName(path);
                    if (up)
                    {
                        MessageBox.Show("上传补丁成功");

                    }
                    else
                    {
                        MessageBox.Show("上传补丁失败");

                    }
                }
                else
                {
                   
                    string[] paths = ftpPath.Split('/');

                    if (ChangeNewDir(ftpPath))
                    {
                        if (UpConfigName(path))
                        {
                            MessageBox.Show("上传补丁成功。");

                        }
                        else
                        {
                            MessageBox.Show("上传补丁失败。");

                        }
                    }
                    else
                    {
                        MessageBox.Show("进入目录失败。");

                        Logs.WriteLog($"进入补丁目录失败，{PathConfig.FtpPatchPath}");
                    }
                }
            }
            else
            {
                MessageBox.Show("服务未连接, 请连接服务");
            }
        }

        /// <summary>
        /// 改变并创建目录
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public bool ChangeNewDir(string dirName)
        {
            string[] paths = dirName.Split('/');
            if (ftp.ChangeDir(PathConfig.FtpUser))
            {
                for (var i = 3; i < paths.Length; i++)
                {
                    if (string.IsNullOrEmpty(paths[i]))
                    {
                        continue;
                    }
                    if (!ftp.ChangeDir(paths[i]))
                    {
                        if (ftp.CreateDirectory(paths[i]))
                        {
                            if (!ftp.ChangeDir(paths[i]))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 上传配置文件和补丁
        /// </summary>
        /// <param name="FileToUpZip"></param>
        private bool UpConfigName(string FileToUpZip)
        {
            try
            {
                bool res = false;
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
                                res = ftp.UploadStr(text, PathConfig.ConfigName);
                               
                            }
                           
                        }
                    }
                }

                //先上传配置文件再上传补丁包文件
                if (res)
                {
                    bool up = ftp.UploadFile(FileToUpZip);
                    if (!up)
                    {
                        ftp.DeleteFile(PathConfig.ConfigName);
                        ftp.DeleteFile(Path.GetFileName(FileToUpZip));
                    }
                    return up;
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
            }
            return false;
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="log"></param>
        private void ShowLog(string log)
        {
            this.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    this.tbLog.Text = $"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}----{log}\n{this.tbLog.Text}";
                }
            });
        }

        /// <summary>
        /// 端口设置只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short val;
            if (!Int16.TryParse(e.Text, out val))
                e.Handled = true;
        }

        private void btnLogin_Copy_Click(object sender, RoutedEventArgs e)
        {
            ftp.ListDirectory("");
            ftp.ChangeDir("..");
            ftp.GetDir();
            try
            {
                ZipFile.CreateFromDirectory(@"E:\Code\PatchUpdateMechanism\MathHotfix\bin\Gktel.Arraignment-V1.0-H06-20200907", @"E:\Code\PatchUpdateMechanism\MathHotfix\bin\aaa.zip",CompressionLevel.Optimal,true);
            }catch(Exception ex)
            {

            }

            string FileToUpZip1 = @"E:\Code\PatchUpdateMechanism\MathHotfix\bin\aaa.zip";
            string FileToUpZip2 = @"E:\Code\PatchUpdateMechanism\MathHotfix\bin\Gktel.Arraignment-V1.0-H01-20200907.ph";
            using (ZipArchive archive = ZipFile.OpenRead(FileToUpZip1))
            {
            }
            using (ZipArchive archive = ZipFile.OpenRead(FileToUpZip2))
            {
            }
        }
    }
}
