using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UpdateFile.Control
{
    public class NineGridBorder : Border
    {
        /// <summary>
        /// 背景图片
        /// </summary>
        public ImageSource Image
        {
            get
            {
                return (ImageSource)base.GetValue(NineGridBorder.ImageProperty);
            }
            set
            {
                base.SetValue(NineGridBorder.ImageProperty, value);
            }
        }


        public Thickness ImageMargin
        {
            get
            {
                return (Thickness)base.GetValue(NineGridBorder.ImageMarginProperty);
            }
            set
            {
                base.SetValue(NineGridBorder.ImageMarginProperty, value);
            }
        }

        /// <summary>
        /// 背景图片不透明度
        /// </summary>
        public double ImageOpacity
        {
            get
            {
                return (double)base.GetValue(NineGridBorder.ImageOpacityProperty);
            }
            set
            {
                base.SetValue(NineGridBorder.ImageOpacityProperty, value);
            }
        }


        private bool IsNineGrid
        {
            get
            {
                return !this.ImageMargin.Equals(default(Thickness));
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            this.DrawImage(dc, new Rect(0.0, 0.0, base.RenderSize.Width, base.RenderSize.Height));
        }

        private void DrawImage(DrawingContext dc, Rect rect)
        {
            ImageSource image = this.Image;
            if (image != null)
            {
                double imageOpacity = this.ImageOpacity;
                if (this.IsNineGrid)
                {
                    Thickness thickness = NineGridBorder.Clamp(this.ImageMargin, new Size(image.Width, image.Height), rect.Size);
                    double[] guidelinesX = new double[]
                    {
                        0.0,
                        thickness.Left,
                        rect.Width - thickness.Right,
                        rect.Width
                    };
                    double[] guidelinesY = new double[]
                    {
                        0.0,
                        thickness.Top,
                        rect.Height - thickness.Bottom,
                        rect.Height
                    };
                    GuidelineSet guidelineSet = new GuidelineSet(guidelinesX, guidelinesY);
                    guidelineSet.Freeze();
                    dc.PushGuidelineSet(guidelineSet);
                    double[] array = new double[]
                    {
                        0.0,
                        thickness.Left / image.Width,
                        (image.Width - thickness.Right) / image.Width,
                        1.0
                    };
                    double[] array2 = new double[]
                    {
                        0.0,
                        thickness.Top / image.Height,
                        (image.Height - thickness.Bottom) / image.Height,
                        1.0
                    };
                    double[] array3 = new double[]
                    {
                        rect.Left,
                        rect.Left + thickness.Left,
                        rect.Right - thickness.Right,
                        rect.Right
                    };
                    double[] array4 = new double[]
                    {
                        rect.Top,
                        rect.Top + thickness.Top,
                        rect.Bottom - thickness.Bottom,
                        rect.Bottom
                    };
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            dc.DrawRectangle(new ImageBrush(image)
                            {
                                Opacity = imageOpacity,
                                Viewbox = new Rect(array[j], array2[i], Math.Max(0.0, array[j + 1] - array[j]), Math.Max(0.0, array2[i + 1] - array2[i]))
                            }, null, new Rect(array3[j], array4[i], Math.Max(0.0, array3[j + 1] - array3[j]), Math.Max(0.0, array4[i + 1] - array4[i])));
                        }
                    }
                    dc.Pop();
                    return;
                }
                dc.DrawRectangle(new ImageBrush(image)
                {
                    Opacity = imageOpacity
                }, null, rect);
            }
        }

        private static Thickness Clamp(Thickness margin, Size firstMax, Size secondMax)
        {
            double num = NineGridBorder.Clamp(margin.Left, firstMax.Width, secondMax.Width);
            double num2 = NineGridBorder.Clamp(margin.Top, firstMax.Height, secondMax.Height);
            double right = NineGridBorder.Clamp(margin.Right, firstMax.Width - num, secondMax.Width - num);
            double bottom = NineGridBorder.Clamp(margin.Bottom, firstMax.Height - num2, secondMax.Height - num2);
            return new Thickness(num, num2, right, bottom);
        }

        private static double Clamp(double value, double firstMax, double secondMax)
        {
            return Math.Max(0.0, Math.Min(Math.Min(value, firstMax), secondMax));
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(NineGridBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(NineGridBorder), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ImageOpacityProperty = DependencyProperty.Register("ImageOpacity", typeof(double), typeof(NineGridBorder), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    }
}
