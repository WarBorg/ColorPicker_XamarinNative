using System;
using Android.Graphics;

namespace ColorPickerTest.Utils
{
    public class ColorUtils
    {
        public void SetColorAlpha(int argb, int alpha) => ClearColorAlpha(argb);// or (alpha << 24);

        public int ClearColorAlpha(int argb) => (int)((uint)(argb << 8) >> 8);

        public Color InterpolateColorLinear(int startColor, int endColor, float offset) => Color.Argb(
            alpha: (int)Math.Round(Color.GetAlphaComponent(startColor) + offset * (Color.GetAlphaComponent(endColor) - Color.GetAlphaComponent(startColor))),
            red: (int)Math.Round(Color.GetRedComponent(startColor) + offset * (Color.GetRedComponent(endColor) - Color.GetRedComponent(startColor))),
            green: (int)Math.Round(Color.GetGreenComponent(startColor) + offset * (Color.GetGreenComponent(endColor) - Color.GetGreenComponent(startColor))),
            blue: (int)Math.Round(Color.GetBlueComponent(startColor) + offset * (Color.GetBlueComponent(endColor) - Color.GetBlueComponent(startColor))));
    }
}