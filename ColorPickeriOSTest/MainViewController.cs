using System;
using CoreGraphics;
using UIKit;

namespace ColorPickeriOSTest
{
    public class MainViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Master View";

            View.BackgroundColor = UIColor.Purple;

            var colorWheel = new ColorWheelUIView(new CGRect(0, 0, 270, 270));

            View.AddSubview(colorWheel);

            colorWheel.TranslatesAutoresizingMaskIntoConstraints = false;

            colorWheel.WidthAnchor.ConstraintEqualTo(270).Active = true;
            colorWheel.HeightAnchor.ConstraintEqualTo(270).Active = true;
            colorWheel.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            colorWheel.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
        }
    }
}
