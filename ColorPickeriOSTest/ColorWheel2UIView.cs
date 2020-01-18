using System;
using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace ColorPickeriOSTest
{
    public class ColorWheel2UIView : UIView
    {
        #region Constructors

        public ColorWheel2UIView()
        {
            Initialize();
        }

        public ColorWheel2UIView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public ColorWheel2UIView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        #endregion

        public void Initialize()
        {
            var filter = new CIHueSaturationValueGradient
            {
                ColorSpace = CGColorSpace.CreateDeviceRGB(),
                Dither = 0,
                Radius = 160,
                Softness = 0,
                Value = 1
            };

            var image = new UIImage(filter.OutputImage);
        }
    }
}
