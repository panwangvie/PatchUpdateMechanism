using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateFile.Interface;

namespace UpdateFile.ViewModel
{
    [Obsolete]
    public class ArMessageInfo : IGetMessageVm
    {
        public VMmessgerInfo GetVm()
        {
            VMmessgerInfo info = new VMmessgerInfo();
            try
            {

                info.LineBursh = info.StrToBrush("#4DB3B5");
                info.FontBursh = info.StrToBrush("#89FAF6");
                info.TitelBrush = info.StrToBrush("#77F7F2");

                string path = "pack://application:,,,/images/arImg/";

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
