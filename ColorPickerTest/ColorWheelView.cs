using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using ColorPickerTest.Utils;

namespace ColorPickerTest
{
    public class ColorWheelView : View
    {
        #region Events

        public event EventHandler<Color> ColorChangeListener;

        #endregion

        #region Fields

        private readonly Context _context;
        private readonly IAttributeSet _attrs;

        private readonly int[] _hueColors = { Color.Red, Color.Yellow, Color.Green, Color.Cyan, Color.Blue, Color.Magenta, Color.Red };
        private readonly int[] _saturationColors = { Color.White, Color.Transparent };

        private GradientDrawable hueGradient;
        private GradientDrawable saturationGradient;

        private ViewConfiguration viewConfig;
        private ThumbDrawable thumbDrawable;
        private HsvColor hsvColor;

        private int _wheelCenterX;
        private int _wheelCenterY;
        private int _wheelRadius;
        private float _motionEventDownX;

        #endregion

        #region Properties

        public bool InterceptTouchEvent { get; set; } = true;

        public Color RgbColor
        {
            get => hsvColor.RGBColor;
            set
            {
                hsvColor.RGBColor = value;
                hsvColor.Value = 1f;
                FireColorListener();
                Invalidate();
            }
        }

        private int _thumbRadius;
        public int ThumbRadius
        {
            get => _thumbRadius;
            set
            {
                _thumbRadius = value;
                UpdateThumbInsets();
                Invalidate();
            }
        }

        #endregion

        #region Constructors

        public ColorWheelView(Context context) :
            base(context)
        {
            _context = context;

            Initialize();
        }

        public ColorWheelView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            _context = context;
            _attrs = attrs;

            Initialize();
        }

        public ColorWheelView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            _context = context;
            _attrs = attrs;

            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            hueGradient = new GradientDrawable();
            hueGradient.SetGradientType(GradientType.SweepGradient);
            hueGradient.SetShape(ShapeType.Oval);
            hueGradient.SetColors(_hueColors);

            saturationGradient = new GradientDrawable();
            saturationGradient.SetGradientType(GradientType.RadialGradient);
            saturationGradient.SetShape(ShapeType.Oval);
            saturationGradient.SetColors(_saturationColors);

            viewConfig = ViewConfiguration.Get(_context);
            thumbDrawable = new ThumbDrawable();
            hsvColor = new HsvColor(value: 1f);

            ParseAttributes(_context, _attrs);
            UpdateThumbInsets();
        }

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            var styledAttributes = context.ObtainStyledAttributes(attrs, Resource.Styleable.ColorWheel, 0, Resource.Style.ColorWheelDefaultStyle);
            _thumbRadius = styledAttributes.GetDimensionPixelSize(Resource.Styleable.ColorWheel_cw_thumbRadius, 0);

            styledAttributes.Recycle();
        }

        private void UpdateThumbInsets() => thumbDrawable.ApplyInsets(_thumbRadius);

        public void SetRgb(int r, int g, int b) => RgbColor = Color.Rgb(r, g, b);

        #region Overrides

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            //base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            SetMeasuredDimension(
                ResolveSize(MeasureSpec.GetSize(widthMeasureSpec), widthMeasureSpec),
                ResolveSize(MeasureSpec.GetSize(heightMeasureSpec), heightMeasureSpec));
        }

        protected override void OnDraw(Canvas canvas)
        {
            //base.OnDraw(canvas);

            DrawColorWheel(canvas);
            DrawThumb(canvas);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    {
                        _motionEventDownX = e.GetX();
                        Parent.RequestDisallowInterceptTouchEvent(InterceptTouchEvent);
                        UpdateColorOnMotionEvent(e);
                        return true;
                    }
                case MotionEventActions.Move:
                    {
                        UpdateColorOnMotionEvent(e);
                        break;
                    }
                case MotionEventActions.Up:
                    {
                        UpdateColorOnMotionEvent(e);

                        if (IsTap(e))
                        {
                            PerformClick();
                        }

                        break;
                    }
            }

            return base.OnTouchEvent(e);
        }

        #endregion

        private void DrawColorWheel(Canvas canvas)
        {
            var hSpace = Width - PaddingLeft - PaddingRight;
            var vSpace = Height - PaddingTop - PaddingBottom;

            _wheelCenterX = PaddingLeft + hSpace / 2;
            _wheelCenterY = PaddingTop + vSpace / 2;
            _wheelRadius = Math.Min(hSpace, vSpace) / 2;
            _wheelRadius = _wheelRadius > 0 ? _wheelRadius : 0;

            var left = _wheelCenterX - _wheelRadius;
            var top = _wheelCenterY - _wheelRadius;
            var right = _wheelCenterX + _wheelRadius;
            var bottom = _wheelCenterY + _wheelRadius;

            hueGradient.SetBounds(left, top, right, bottom);
            saturationGradient.SetBounds(left, top, right, bottom);
            saturationGradient.SetGradientRadius(_wheelRadius);

            hueGradient.Draw(canvas);
            saturationGradient.Draw(canvas);
        }

        private void DrawThumb(Canvas canvas)
        {
            var r = hsvColor.Saturation * _wheelRadius;
            var hueRadians = hsvColor.Hue.ToRadians();
            var thumbX = (int)(Math.Cos(hueRadians) * r + _wheelCenterX);
            var thumbY = (int)(Math.Sin(hueRadians) * r + _wheelCenterY);

            thumbDrawable.SetBounds(
                thumbX - _thumbRadius,
                thumbY - _thumbRadius,
                thumbX + _thumbRadius,
                thumbY + _thumbRadius
            );

            thumbDrawable.IndicatorColor = hsvColor.RGBColor;
            thumbDrawable.Draw(canvas);
        }

        private void UpdateColorOnMotionEvent(MotionEvent e)
        {
            CalculateColor(e);
            FireColorListener();
            Invalidate();
        }

        private void CalculateColor(MotionEvent e)
        {
            var legX = e.GetX() - _wheelCenterX;
            var legY = e.GetY() - _wheelCenterY;
            var r = CalculateRadius(legX, legY);
            var angle = Math.Atan2(legY, legX);
            var x = Math.Cos(angle) * r + _wheelCenterX;
            var y = Math.Sin(angle) * r + _wheelCenterY;
            var dx = x - _wheelCenterX;
            var dy = y - _wheelCenterY;
            var hue = (((float)Math.Atan2(dy, dx)).ToDegrees() + 360) % 360;
            var saturation = Hypotenuse((float)dx, (float)dy) / _wheelRadius;

            hsvColor.Hue = hue;
            hsvColor.Saturation = saturation;
            hsvColor.Value = 1f;
        }

        private float CalculateRadius(float legX, float legY)
        {
            var radius = Hypotenuse(legX, legY);
            return (radius > _wheelRadius) ? _wheelRadius : radius;
        }

        private bool IsTap(MotionEvent e)
        {
            var eventDuration = e.EventTime - e.DownTime;
            var eventTravelDistance = Math.Abs(e.GetX() - _motionEventDownX);

            return eventDuration < ViewConfiguration.TapTimeout && eventTravelDistance < viewConfig.ScaledTouchSlop;
        }

        private void FireColorListener() => ColorChangeListener?.Invoke(this, hsvColor.RGBColor);

        public float Hypotenuse(float a, float b)
        {
            return (float)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        #endregion
    }
}
