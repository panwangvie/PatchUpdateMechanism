using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UpdateFile;

namespace HotfixMath
{

    ///客户端补丁更新，使用实例

    /// <summary>
    /// 
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ///客户端启动或者是需要检测更新的时候调用FtpTestMethod.Update方法，并且入参传入连接FTP的相关参数即可。
            ///
            ///获取FTP的方式由各个客户端自行获取
            ///

            FtpInfo ftpInfo = new FtpInfo()
            {
                Host = "192.168.1.85",
                UserName = "ftpuser",
                Passwd = "gktel123456",
                Port = 21
            };

            FtpTestMethod.Update(ftpInfo);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
    }
}
