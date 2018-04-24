using Android.App;
using Android.Content;
using Android.OS;
using SomulApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace SomulApp.Droid
{
    [Activity(Label = "Somul Remote", Icon = "@drawable/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "Somul:" + typeof(SplashActivity).Name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        public override void OnBackPressed() { }
    }
}