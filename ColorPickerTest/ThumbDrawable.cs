using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

namespace ColorPickerTest
{
    public class ThumbDrawable
    {
        #region Fields

        private readonly GradientDrawable _thumbCircle;
        private readonly ShapeDrawable _colorIndicatorDrawable;
        private readonly LayerDrawable _thumbDrawable;

        #endregion

        #region Properties

        public Color IndicatorColor
        {
            get => _colorIndicatorDrawable.Paint.Color;
            set => _colorIndicatorDrawable.Paint.Color = value;
        }

        public Rect Bounds
        {
            get => _thumbDrawable.Bounds;
            set => _thumbDrawable.Bounds = value;
        }

        #endregion

        #region Constructor

        public ThumbDrawable()
        {
            _thumbCircle = new GradientDrawable();

            _thumbCircle.SetShape(ShapeType.Oval);
            _thumbCircle.SetStroke(width: 1, Color.Gray);
            _thumbCircle.SetColor(Color.White);

            _colorIndicatorDrawable = new ShapeDrawable(new OvalShape());

            _thumbDrawable = new LayerDrawable(new Drawable[] { _thumbCircle, _colorIndicatorDrawable });
        }

        #endregion

        #region Methods

        public void ApplyInsets(float thumbRadius)
        {
            var colorHInset = (int)(thumbRadius * 0.3f);
            var colorVInset = (int)(thumbRadius * 0.3f);

            _thumbDrawable.SetLayerInset(index: 1, colorHInset, colorVInset, colorHInset, colorVInset);
        }

        public void SetBounds(int left, int top, int right, int bottom) => _thumbDrawable.SetBounds(left, top, right, bottom);

        public void Draw(Canvas canvas) => _thumbDrawable.Draw(canvas);

        #endregion
    }
}
