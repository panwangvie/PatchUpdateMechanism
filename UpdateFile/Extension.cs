using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UpdateFile
{
    /// <summary>
    /// 拓展方法
    /// </summary>

    public static class TextBlockExtension
    {
        public static void SetFontSizeKey(this TextBlock text_block, String font_size_key)
        {
            text_block.SetResourceReference(TextBlock.FontSizeProperty, font_size_key);
        }
        public static void SetTextKey(this TextBlock text_block, String resource_key)
        {
            text_block.SetResourceReference(TextBlock.TextProperty, resource_key);
        }
        public static void SetBackgroundKey(this TextBlock text_block, String background_key)
        {
            text_block.SetResourceReference(TextBlock.BackgroundProperty, background_key);
        }
        public static void SetForegroundKey(this TextBlock text_block, String foreground_key)
        {
            text_block.SetResourceReference(TextBlock.ForegroundProperty, foreground_key);
        }
    }

}
