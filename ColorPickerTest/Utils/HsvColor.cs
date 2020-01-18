using System;
using Android.Graphics;

namespace ColorPickerTest.Utils
{
    public class HsvColor
    {
        #region Fields

        private readonly float[] _hsvColor;

        #endregion

        #region Properties

        public float Hue
        {
            get => _hsvColor[0];
            set => _hsvColor[0] = EnsureHueWithinRange(value);
        }

        public float Saturation
        {
            get => _hsvColor[1];
            set => _hsvColor[1] = EnsureSaturationWithinRange(value);
        }

        public float Value
        {
            get => _hsvColor[2];
            set => _hsvColor[2] = EnsureValueWithinRange(value);
        }

        public Color RGBColor
        {
            get => Color.HSVToColor(_hsvColor);
            set => Color.ColorToHSV(value, _hsvColor);
        }

        #endregion

        #region Constructor

        public HsvColor(float hue = 0f, float saturation = 0f, float value = 0f)
        {
            _hsvColor = new float[]
            {
                EnsureHueWithinRange(hue),
                EnsureSaturationWithinRange(saturation),
                EnsureValueWithinRange(value)
            };
        }

        #endregion

        #region Methods

        private float EnsureHueWithinRange(float hue) => EnsureNumberWithinRange(hue, 0f, 360f);

        private float EnsureSaturationWithinRange(float saturation) => EnsureNumberWithinRange(saturation, 0f, 1f);

        private float EnsureValueWithinRange(float value) => EnsureNumberWithinRange(value, 0f, 1f);

        private T EnsureNumberWithinRange<T>(T value, T start, T end) where T : IComparable<T>
        {
            if (value.CompareTo(start) < 0)
            {
                return start;
            }

            if (value.CompareTo(end) > 0)
            {
                return end;
            }

            return value;
        }

        #endregion
    }
}
