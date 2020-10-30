using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
/**
 * 检测更新，提示
 **/
namespace UpdateFile.Control
{
    /// <summary>
    /// MessgerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class MessgerInfo : Window
    {
        public MessgerInfo()
        {
            InitializeComponent();

            this.winStyle.Source = new Uri("pack://application:,,,/UpdateFile;component/Styles/MeetStyle.xaml");
        }

        public MessgerInfo(string Type)
        {
            InitializeComponent();

            this.winStyle.Source = new Uri($"pack://application:,,,/UpdateFile;component/Styles/{Type}.xaml");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NineGridBorder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source == nineboder)
            {
                this.DragMove();
            }
        }

        public Action YesAction;

        public Action NoAction;

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {

            YesAction?.Invoke();
            this?.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            NoAction?.Invoke();
            this?.Close();

        }
    }
}
