using System;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ColorPickerTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private View _colorIndicator;
        private GradientDrawable _colorIndicatorBackground;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var colorWheel = FindViewById<ColorWheelView>(Resource.Id.colorWheel);
            _colorIndicator = FindViewById<View>(Resource.Id.colorIndicator);

            colorWheel.ColorChangeListener += ColorWheel_ColorChangeListener;

            _colorIndicatorBackground = new GradientDrawable();
            _colorIndicatorBackground.SetColor(colorWheel.RgbColor);

            _colorIndicator.Background = _colorIndicatorBackground;
        }

        private void ColorWheel_ColorChangeListener(object sender, Android.Graphics.Color e)
        {
            _colorIndicatorBackground.SetColor(e);

            _colorIndicator.Background = _colorIndicatorBackground;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

