using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge.Html5;

namespace bitOxide.MarioWiiPowerup.Javascript
{
    public class ScaledImage
    {
        private readonly HTMLImageElement image;
        private readonly bool blackBg;
        private int? targetSize;
        private HTMLCanvasElement scaledImage = null;

        public int TargetSize
        {
            get
            {
                return targetSize.Value;
            }
            set
            {
                if (value != targetSize)
                {
                    var scale = value / (double)image.Width;
                    scaledImage = CanvasDownscaleHelper.ScaleImage(image, scale);
                    targetSize = value;
                }
            }
        }

        public bool BlackBackground => blackBg;
        public bool Complete => image.Complete;
        public HTMLCanvasElement ScaledImg => scaledImage;

        public ScaledImage(HTMLImageElement image, bool blackBg)
        {
            this.image = image;
            this.blackBg = blackBg;
            targetSize = image.Width;
        }
    }
}
