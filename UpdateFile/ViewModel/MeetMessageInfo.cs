using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UpdateFile.Interface;

namespace UpdateFile.ViewModel
{
    [Obsolete]
    public class MeetMessageInfo: IGetMessageVm
    {
        public VMmessgerInfo GetVm()
        {
            VMmessgerInfo info = new VMmessgerInfo();
            try
            {

                info.LineBursh = info.StrToBrush("#006FE9");
                info.FontBursh = info.StrToBrush("#FFFFFF");
                info.TitelBrush = info.StrToBrush("#35FFFA");

                string path = "pack://application:,,,/images/meetImg/";

                info.BgImg = info.StrToImg(path + "背景框.png");
                info.YesImgHov = info.StrToImg(path + "按钮背景hov.png");
                info.YesImgNor = info.StrToImg(path + "按钮背景nor.png");
                info.NoImgHov = info.StrToImg(path + "按钮背景hov.png");
                info.NoImgNor = info.StrToImg(path + "按钮背景nor.png");

                info.CloseImgHov = info.StrToImg(path + "背景框关闭hov.png");
                info.CloseImgNor = info.StrToImg(path + "背景框关闭nor.png");
            }
            catch (Exception ex)
            {

            }

            return info;
        }
    }
}
