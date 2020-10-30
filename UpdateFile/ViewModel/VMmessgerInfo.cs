using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UpdateFile.ViewModel
{
    /// <summary>
    /// 弃用
    /// </summary>
    [Obsolete]
    public class VMmessgerInfo
    {

        private string _messgerTitel = "检测到新版本";

        private string _verText = "版本信息：";

        private string _verInfo = "";

        private string _updateText = "更新内容：";


        private string _updateInfo = "";

        private Brush _titelBrush  ;

        private ImageSource _bgImg;

        public ImageSource CloseImgNor { get; set; }

        public ImageSource CloseImgHov { get; set; }

        public ImageSource YesImgNor { get; set; }

        public ImageSource YesImgHov { get; set; }

        public ImageSource NoImgNor { get; set; }

        public ImageSource NoImgHov { get; set; }

        public Brush FontBursh { get; set; }

        public Brush LineBursh { get; set; }

        public string MessgerTitel { get => _messgerTitel; set => _messgerTitel = value; }
        public string VerText { get => _verText; set => _verText = value; }
        public string VerInfo { get => _verInfo; set => _verInfo = value; }
        public string UpdateText { get => _updateText; set => _updateText = value; }
        public string UpdateInfo { get => _updateInfo; set => _updateInfo = value; }
        public Brush TitelBrush { get => _titelBrush; set => _titelBrush = value; }
        public ImageSource BgImg { get => _bgImg; set => _bgImg = value; }

        public Brush StrToBrush(string colorStr)
        {
            BrushConverter brushConverter = new BrushConverter();
            Brush brush = (Brush)brushConverter.ConvertFromString(colorStr);
            return brush;
        }

        
        public ImageSource StrToImg(string path)

        {

            return new BitmapImage(new Uri(path));
        }



        //public static BitmapImage GetImage(string filename)
        //{
        //    string imgLocation = Application.Current.Resources["ImagesLocation"].ToString();

        //    StreamResourceInfo imageResource = Application.GetResourceStream(new Uri(imgLocation + filename, UriKind.Relative));
        //    BitmapImage image = new BitmapImage();
        //    image.SetSource(imageResource.Stream);

        //    return image;
        //}


    }
}
