using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ColorPickeriOSTest
{
    public class ColorWheelUIView : UIView
    {
        private UIColor _color;

        // Layer for the Hue and Saturation wheel
        private CALayer _wheelLayer;

        // Overlay layer for the brightness
        private CAShapeLayer _brightnessLayer;
        private nfloat _brightness = 1.0f;

        // Layer for the indicator
        private CAShapeLayer _indicatorLayer;
        private CGPoint _point;
        private float _indicatorCircleRadius = 12.0f;
        private CGColor _indicatorColor = UIColor.LightGray.CGColor;
        private nfloat _indicatorBorderWidth = 2.0f;

        // Retina scaling factor
        private nfloat _scale = UIScreen.MainScreen.Scale;

        #region Constructors

        public ColorWheelUIView()
        {
            Initialize();
        }

        public ColorWheelUIView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public ColorWheelUIView(CGRect frame) : base(frame)
        {
            Initialize();
        }

        //protected ColorWheelUIView(NSObjectFlag t) : base(t)
        //{
        //}

        //protected internal ColorWheelUIView(IntPtr handle) : base(handle)
        //{
        //}

        #endregion

        #region Methods

        private void Initialize()
        {
            _color = UIColor.White; // color

            // Layer for the Hue/Saturation wheel
            _wheelLayer = new CALayer();
            _wheelLayer.Frame = new CGRect(x: 20, y: 20, width: Frame.Width - 40, height: Frame.Height - 40);
            _wheelLayer.Contents = CreateColorWheel(_wheelLayer.Frame.Size);
            Layer.AddSublayer(_wheelLayer);

            // Layer for the brightness
            _brightnessLayer = new CAShapeLayer();
            _brightnessLayer.Path = UIBezierPath.FromRoundedRect(rect: new CGRect(x: 20.5f,
                                                                                 y: 20.5f,
                                                                                 width: Frame.Width - 40.5f,
                                                                                 height: Frame.Height - 40.5f),
                                                                cornerRadius: (Frame.Height - 40.5f) / 2)
                                               .CGPath;

            //Layer.AddSublayer(_brightnessLayer);

            // Layer for the indicator
            _indicatorLayer = new CAShapeLayer();
            _indicatorLayer.StrokeColor = _indicatorColor;
            _indicatorLayer.LineWidth = _indicatorBorderWidth;
            _indicatorLayer.FillColor = null;
            Layer.AddSublayer(_indicatorLayer);

            SetViewColor(_color);
        }

        #region Overrides

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            _indicatorCircleRadius = 18.0f;
            TouchHandler(touches);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            TouchHandler(touches);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            _indicatorCircleRadius = 12.0f;
            TouchHandler(touches);
        }

        #endregion

        private void TouchHandler(NSSet touches)
        {
            // Set reference to the location of the touch in member point
            var touch = touches.FirstOrDefault() as UITouch;

            if (touch == null)
            {
                return;
            }

            _point = touch.LocationInView(this);

            var indicator = GetIndicatorCoordinate(_point);
            _point = indicator.Point;
            var color = (hue: (nfloat)0, saturation: (nfloat)0f);

            if (!indicator.IsCenter)
            {
                color = HueSaturationAtPoint(new CGPoint(x: _point.X * _scale, y: _point.Y * _scale));
            }

            _color = UIColor.FromHSBA(hue: color.hue, saturation: color.saturation, brightness: _brightness, alpha: 1.0f);

            nfloat red = 0f;
            nfloat green = 0f;
            nfloat blue = 0f;
            nfloat alpha = 0f;

            _color.GetRGBA(out red, out green, out blue, out alpha);

            Console.WriteLine($"color hue: Red {Math.Round(red, 2)} Green {Math.Round(green, 2)} Blue {Math.Round(blue, 2)}");

            // Notify delegate of the new Hue and Saturation
            //delegate?.hueAndSaturationSelected(color.hue, saturation: color.saturation);

            // Draw the indicator
            DrawIndicator();
        }

        private void DrawIndicator()
        {
            // Draw the indicator
            if (_point != CGPoint.Empty)
            {
                _indicatorLayer.Path = UIBezierPath.FromRoundedRect(rect: new CGRect(x: _point.X - _indicatorCircleRadius,
                                                                                    y: _point.Y - _indicatorCircleRadius,
                                                                                    width: _indicatorCircleRadius * 2,
                                                                                    height: _indicatorCircleRadius * 2),
                                                                   cornerRadius: _indicatorCircleRadius)
                                                  .CGPath;

                _indicatorLayer.FillColor = _color.CGColor;
            }
        }

        private (CGPoint Point, bool IsCenter) GetIndicatorCoordinate(CGPoint coord)
        {
            // Making sure that the indicator can't get outside the Hue and Saturation wheel
            nfloat dimension = (nfloat)Math.Min(_wheelLayer.Frame.Width, _wheelLayer.Frame.Height);
            nfloat radius = dimension / 2;

            var wheelLayerCenter = new CGPoint(x: _wheelLayer.Frame.X + radius,
                                               y: _wheelLayer.Frame.Y + radius);

            nfloat dx = coord.X - wheelLayerCenter.X;
            nfloat dy = coord.Y - wheelLayerCenter.Y;
            nfloat distance = (nfloat)Math.Sqrt(dx * dx + dy * dy);
            CGPoint outputCoord = coord;

            // If the touch coordinate is outside the radius of the wheel, transform it to the edge of the wheel with polar coordinates
            if (distance > radius)
            {
                nfloat theta = (nfloat)Math.Atan2(dy, dx);
                outputCoord.X = radius * (nfloat)Math.Cos(theta) + wheelLayerCenter.X;
                outputCoord.Y = radius * (nfloat)Math.Sin(theta) + wheelLayerCenter.Y;
            }

            // If the touch coordinate is close to center, focus it to the very center at set the color to white
            nfloat whiteThreshold = 2f;
            var isCenter = false;

            if (distance < whiteThreshold)
            {
                outputCoord.X = wheelLayerCenter.X;
                outputCoord.Y = wheelLayerCenter.Y;
                isCenter = true;
            }

            return (outputCoord, isCenter);
        }

        private CGImage CreateColorWheel(CGSize size)
        {
            // Creates a bitmap of the Hue Saturation wheel
            nfloat originalWidth = size.Width;
            nfloat originalHeight = size.Height;
            nfloat dimension = (nfloat)Math.Min(originalWidth * _scale, originalHeight * _scale);
            var bufferLength = (nuint)(dimension * dimension * 4);

            //NSMutableData bitmapData = NSMutableData.FromBytes(IntPtr.Zero, 0);
            var bitmapData = new NSMutableData
            {
                Length = bufferLength
            };

            /*CFMutableData bitmapData  = CFDataCreateMutable(nil, 0)
        CFDataSetLength(bitmapData, CFIndex(bufferLength))
        let bitmap = CFDataGetMutableBytePtr(bitmapData)*/

            for (nfloat y = 0f; y < dimension; y++)
            {
                for (nfloat x = 0f; x < dimension; x++)
                {
                    var hsv = new HSV { Hue = 0, Saturation = 0, Brightness = 0, Alpha = 0 };
                    var rgb = new RGB { Red = 0, Green = 0, Blue = 0, Alpha = 0 };

                    var color = HueSaturationAtPoint(new CGPoint(x, y));
                    var hue = color.hue;
                    var saturation = color.saturation;
                    nfloat a = 0.0f;

                    if (saturation < 1.0)
                    {
                        // Antialias the edge of the circle.
                        if (saturation > 0.99)
                        {
                            a = (1.0f - saturation) * 100;
                        }
                        else
                        {
                            a = 1.0f;
                        }

                        hsv.Hue = hue;
                        hsv.Saturation = saturation;
                        hsv.Brightness = 1.0f;
                        hsv.Alpha = a;

                        rgb = Hsv2Rgb(hsv);
                    }

                    var offset = (int)(4 * (x + y * dimension));
                    bitmapData[offset] = (byte)(rgb.Red * 255);
                    bitmapData[offset + 1] = (byte)(rgb.Green * 255);
                    bitmapData[offset + 2] = (byte)(rgb.Blue * 255);
                    bitmapData[offset + 3] = (byte)(rgb.Alpha * 255);//*/
                }
            }

            // Convert the bitmap to a CGImage
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var dataProvider = new CGDataProvider(bitmapData);
            var bitmapInfo = CGImageAlphaInfo.Last; //CGBitmapInfo(rawValue: CGBitmapInfo().rawValue | CGImageAlphaInfo.Last.rawValue);
            var imageRef = new CGImage(width: (int)dimension,
                                       height: (int)dimension,
                                       bitsPerComponent: 8,
                                       bitsPerPixel: 32,
                                       bytesPerRow: (int)dimension * 4,
                                       colorSpace: colorSpace,
                                       alphaInfo: bitmapInfo,
                                       provider: dataProvider,
                                       decode: null,
                                       shouldInterpolate: false,
                                       intent: CGColorRenderingIntent.Default);
            return imageRef;
        }

        private (nfloat hue, nfloat saturation) HueSaturationAtPoint(CGPoint position)
        {
            // Get hue and saturation for a given point (x,y) in the wheel

            var c = _wheelLayer.Frame.Width * _scale / 2;
            var dx = (position.X - c) / c;
            var dy = (position.Y - c) / c;
            var d = Math.Sqrt(dx * dx + dy * dy);

            var saturation = (nfloat)d;

            nfloat hue;

            if (d == 0)
            {
                hue = 0;
            }
            else
            {
                hue = (nfloat)(Math.Acos(dx / d) / Math.PI / 2.0);

                if (dy < 0)
                {
                    hue = 1.0f - hue;
                }
            }

            return (hue, saturation);
        }

        private CGPoint PointAtHueSaturation(nfloat hue, nfloat saturation)
        {
            // Get a point (x,y) in the wheel for a given hue and saturation
            var dimension = (nfloat)Math.Min(_wheelLayer.Frame.Width, _wheelLayer.Frame.Height);
            var radius = saturation * dimension / 2;
            var x = dimension / 2 + radius * (nfloat)Math.Cos(hue * Math.PI * 2) + 20;
            var y = dimension / 2 + radius * (nfloat)Math.Sin(hue * Math.PI * 2) + 20;

            return new CGPoint(x: x, y: y);
        }

        private void SetViewColor(UIColor color)
        {
            // Update the entire view with a given color
            nfloat hue = 0.0f;
            nfloat saturation = 0.0f;
            nfloat brightness = 0.0f;
            nfloat alpha = 0.0f;

            try
            {
                color.GetHSBA(out hue, out saturation, out brightness, out alpha);
            }
            catch (Exception ex)
            {
                Console.Write("SwiftHSVColorPicker: exception <The color provided to SwiftHSVColorPicker is not convertible to HSV>");
                throw ex;
            }
            //color// getHue(&hue, saturation: &saturation, brightness: &brightness, alpha: &alpha)
            /*if (!ok)
                {
                    print("SwiftHSVColorPicker: exception <The color provided to SwiftHSVColorPicker is not convertible to HSV>")
            }*/
            _color = color;
            _brightness = brightness;
            _brightnessLayer.FillColor = new UIColor(white: 0, alpha: 1.0f - _brightness).CGColor;
            //point = pointAtHueSaturation(hue, saturation: saturation);
            DrawIndicator();
        }

        private RGB Hsv2Rgb(HSV hsv)
        {
            // Converts HSV to a RGB color
            var rgb = new RGB { Red = 0.0f, Green = 0.0f, Blue = 0.0f, Alpha = 0.0f };
            nfloat r, g, b;

            var i = (int)(hsv.Hue * 6);
            var f = hsv.Hue * 6 - i;
            var p = hsv.Brightness * (1 - hsv.Saturation);
            var q = hsv.Brightness * (1 - f * hsv.Saturation);
            var t = hsv.Brightness * (1 - (1 - f) * hsv.Saturation);

            switch (i % 6)
            {
                case 0: r = hsv.Brightness; g = t; b = p; break;
                case 1: r = q; g = hsv.Brightness; b = p; break;
                case 2: r = p; g = hsv.Brightness; b = t; break;
                case 3: r = p; g = q; b = hsv.Brightness; break;
                case 4: r = t; g = p; b = hsv.Brightness; break;
                case 5: r = hsv.Brightness; g = p; b = q; break;
                default: r = hsv.Brightness; g = t; b = p; break;
            }

            rgb.Red = r;
            rgb.Green = g;
            rgb.Blue = b;
            rgb.Alpha = hsv.Alpha;
            return rgb;
        }

        #endregion

        #region Helper Classes

        private class HSV
        {
            public nfloat Hue { get; set; }
            public nfloat Saturation { get; set; }
            public nfloat Brightness { get; set; }
            public nfloat Alpha { get; set; }
        }

        private class RGB
        {
            public nfloat Red { get; set; }
            public nfloat Green { get; set; }
            public nfloat Blue { get; set; }
            public nfloat Alpha { get; set; }
        }

        #endregion
    }
}
