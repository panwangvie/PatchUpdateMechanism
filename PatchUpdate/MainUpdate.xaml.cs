using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UpdateFile;
using UpdateFile.Control;
using UpdateFile.Log;
/**
 * 
 * 补丁更新的程序，进行更新流程
 * 
 * */
namespace PatchUpdate
{
    /// <summary>
    /// MianUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class MainUpdate : Window
    {
        public MainUpdate()
        {
            InitializeComponent();
        }

        public MainUpdate(StartupEventArgs e)
        {
            string[] args = e?.Args;
            string path = "pack://application:,,,/UpdateFile;component/Styles/";
            FtpInfo ftpInfo = new FtpInfo();
            VersionInfo version = new VersionInfo();
            if (args?.Length > 0)
            {

                System.IO.File.WriteAllLines($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}my.txt", args);

            }
            else
            {
                System.IO.File.WriteAllLines($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}my.txt", new string[] { $"没有，{DateTime.Now.ToString("yyyyMMMdd:HH:mm:ss")}" });

            }
          
            if (args?.Length >= 4)
            {
                ftpInfo.Host = args[0];
                int port;
                int.TryParse(args[1], out port);
                ftpInfo.Port = port;

                ftpInfo.UserName = args[2];
                ftpInfo.Passwd = args[3];
            }
            if (args?.Length >= 6)
            {
                ftpInfo.PasswdMd5 = args[4];
                strExeName = args[5];
            }
            if (args?.Length >= 8)
            {
                FtpInfo info = Json.Parse<FtpInfo>(args[5]);
                version = Json.Parse<VersionInfo>(args[6]);
                Logs.WriteLog($"{args[5]}{args[6]}");
                //"{\"Host\":\"192.168.1.85\",\"Passwd\":\"gktel123456\",\"PasswdMd5\":null,\"Port\":21,\"UserName\":\"ftpuser\"}"
            }
            try
            {
                if (!string.IsNullOrEmpty(strExeName)&&strExeName.Contains("SEngine"))
                {
                    path += "ArStyle.xaml";
                }
                else
                {
                    path += "MeetStyle.xaml";

                }

              
            }catch(Exception ex)
            {

            }

            InitializeComponent();

            this.winStyle.Source = new Uri(path);

            Closed += MianUpdate_Closed;
       
       
            start = new StartUpdatePatch();
            start.DownloadEvent += DownloadEvent;
            start.MessageEvent += MessageEvent;
            start.UnZipEvent += UnZipEvent;
            start.VersionEvent += VersionEvent;
            start.OnFileTranProgress += OnFileTranProgress;
            start.DownLoadPatch(ftpInfo, strExeName, version);

        }

        /// <summary>
        /// 
        /// </summary>
        private void StartDown(string[] args)
        {

        }

        /// <summary>
        /// 动态设置样式设置样式
        /// </summary>
        [Obsolete]
        private void SetStyle()
        {
         //使用资源中的solidBrush
         //this.Background = (SolidColorBrush)this.FindResource("solidBrush001");

         //使用setResourceReference绑定，可以动态修改，如果dictionary发生改变，界面也随之改变
         this.SetResourceReference(BackgroundProperty,"solidBrush002");

            this.nineboder.SetResourceReference(NineGridBorder.ImageProperty, "MianBg.Image");
            nineboder.SetResourceReference(NineGridBorder.ImageMarginProperty, "NineMarge");
            tbTitel.SetForegroundKey("TitleBrush");
            tbPercentage.SetForegroundKey("FontBrush");
            tbSpeed.SetForegroundKey("FontBrush");
            tbUpdateInfo.SetForegroundKey("FontBrush");
            tbUpText.SetForegroundKey("FontBrush");
            tbVer.SetForegroundKey("FontBrush");
            tbPercentage.SetForegroundKey("FontBrush");
            TitLine.SetResourceReference(Line.StrokeProperty, "LineBrush");
            bdrVer.SetResourceReference(Border.BorderBrushProperty, "BorderBrush");
            pBar.SetResourceReference(StyleProperty, "PBarStyle");
            btnClose.SetResourceReference(StyleProperty, "btnCloseStyle");
            //PBarColorStart.


        }

        StartUpdatePatch start;

        string strExeName;

        private void MianUpdate_Closed(object sender, EventArgs e)
        {
            Closed -= MianUpdate_Closed;
            if (start != null)
            {
                start.DownloadEvent -= DownloadEvent;
                start.MessageEvent -= MessageEvent;
                start.UnZipEvent -= UnZipEvent;
                start.VersionEvent -= VersionEvent;
                start.OnFileTranProgress -= OnFileTranProgress;
            }
        }

        //下载情况
        private void DownloadEvent(string info)
        {
            this.Dispatcher.Invoke(()=> { this.tbUpdateInfo.Text = info; });
        }

        /// <summary>
        /// 操作日志事件
        /// </summary>
        private void MessageEvent(string info)
        {

        }
        /// <summary>
        /// 进度事件
        /// </summary>
        private void OnFileTranProgress(int pro)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.pBar.Value = pro*100;
            });
        }

        /// <summary>
        /// 解压进度
        /// </summary>
        private void UnZipEvent(int pro)
        {
            this.Dispatcher.Invoke(() =>
            {
                 this.pBar.Value = pro*100;
            });
        }

        /// <summary>
        /// 补丁信息事件
        /// </summary>
        private void VersionEvent(VersionInfo version)
        {
            this.Dispatcher.Invoke(()=> 
            {
                this.runVer.Text = version.Version;
                tbUpInfo.Text= version.UpdateDesc;
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            start = new StartUpdatePatch();
            start.DownloadEvent += DownloadEvent;
            start.MessageEvent += MessageEvent;
            start.UnZipEvent += UnZipEvent;
            start.VersionEvent += VersionEvent;
            start.OnFileTranProgress += OnFileTranProgress;
            string aa = "{\"Host\":\"192.168.1.85\",\"Passwd\":\"gktel123456\",\"PasswdMd5\":null,\"Port\":21,\"UserName\":\"ftpuser\"}";
            FtpInfo info = Json.Parse<FtpInfo>(aa);

            //start.DelPatchPath();
            start.DownLoadPatch(info, null);

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NineGridBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source == nineboder)
            {
                this.DragMove();
            }
        }
    }
}
