using System;
using Xamarin.Forms;

namespace SomulApp
{
    public partial class App : Application
	{
		public App()
		{
            InitializeComponent();

            MainPage = new SplashScreen();
        }

		protected override void OnStart ()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MainPage = new GreetingsPage();
            });
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
