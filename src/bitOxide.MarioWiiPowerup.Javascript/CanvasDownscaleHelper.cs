using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace bitOxide.MarioWiiPowerup.Javascript
{
    internal static class CanvasDownscaleHelper
    {
        private static HTMLCanvasElement DownscaleImage(HTMLImageElement img, double scale)
        {
            var nWidth = (int)Math.Round(img.Width * scale);
            var nHeight = (int)Math.Round(img.Height * scale);

            var imgCV = new HTMLCanvasElement();
            imgCV.Width = img.Width;
            imgCV.Height = img.Height;
            var imgCtx = imgCV.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            imgCtx.DrawImage(img, 0, 0);

            Script.Write<HTMLCanvasElement>("resample_single(imgCV,nWidth,nHeight,true);");

            return imgCV;
        }

        private static HTMLCanvasElement UpscaleImage(HTMLImageElement img, double scale)
        {
            var nWidth = (int)Math.Round(img.Width * scale);
            var nHeight = (int)Math.Round(img.Height * scale);

            var imgCV = new HTMLCanvasElement();
            imgCV.Width = nWidth;
            imgCV.Height = nHeight;
            var imgCtx = imgCV.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            imgCtx.DrawImage(img, 0, 0, nWidth, nHeight);

            return imgCV;
        }

        public static HTMLCanvasElement ScaleImage(HTMLImageElement img, double scale)
        {
            if(scale <= 0)
            {
                var imgCV = new HTMLCanvasElement();
                imgCV.Width = 0;
                imgCV.Height = 0;
                return imgCV;
            }
            else if(scale < 1)
            {
                return DownscaleImage(img, scale);
            }
            else
            {
                return UpscaleImage(img, scale);
            }
        }
    }
}
