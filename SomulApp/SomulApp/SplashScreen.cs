using Xamarin.Forms;

namespace SomulApp
{
    public class SplashScreen : ContentPage
	{
		public SplashScreen ()
		{
            BackgroundColor = SomulColors.PrimaryDarker;

            Content = new StackLayout {

                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,

                Children = {

                    new Image
                    {
                        Source = ImageSource.FromFile("somul_logo.png")
                    },
                    new ActivityIndicator
                    {
                        Color = SomulColors.Accent,
                        IsRunning = true,
                        IsEnabled = true,
                        IsVisible = true
                    }
                }
            };
		}
	}
}