using System;
using Xamarin.Forms;

namespace SomulApp
{
    public class GreetingsPage : ContentPage
    {
        private Image somulBanner;
        private Slider intensitySlider;
        private Slider hueSlider;
        private Slider warmthSlider;
        private Label sliderLabel;
        public Button bluetoothButton;
        private ContentView hueView;
        private ContentView warmView;
        private Switch partySwitch;

        private IBluetoothConnect bluetoothConnect;
        private string dataToSend;

        // For easy multiplication in the Arduino
        double scaledIntensity;

        // We never want the hot to be completely red, for asthetic reasons
        double tempPercentage = 0.9;
        Color lampColor = Color.Wheat;

        public GreetingsPage()
        {
            bluetoothConnect = DependencyService.Get<IBluetoothConnect>();
            BackgroundColor = SomulColors.PrimaryDarker;

            StackLayout bluetoothIOLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 20, 20, 0),
                        Text = "Connect Somul",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },

                    (bluetoothButton  = new Button
                    {
                        Margin = new Thickness(20, 10, 10, 0),
                        Text = "Connect",
                        TextColor = SomulColors.Accent,
                        BackgroundColor = SomulColors.PrimaryDarkDarkest
                    })
                }
            };

            StackLayout intensitySliderLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    (intensitySlider = new Slider(0.0, 100.0, 0.0)
                    {
                        Margin = new Thickness(10, 0, 0, 0),
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    }),

                    (sliderLabel = new Label
                    {
                        Margin = new Thickness(0, 0, 15, 0),
                        Text = " Off",
                        HorizontalTextAlignment = TextAlignment.Start,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    })
                }
            };

            StackLayout hueSliderLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    (hueSlider = new Slider(0.0, 1.0, 0.0)
                    {
                        Margin = new Thickness(10, 0, 0, 0),
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    }),

                    (hueView  = new ContentView()
                    {
                         BackgroundColor = Color.Red,
                         Margin = new Thickness(0, 0, 15, 0),
                         HeightRequest = 30.0,
                         WidthRequest = 30.0,
                         HorizontalOptions = LayoutOptions.Center,
                         VerticalOptions = LayoutOptions.Start
                    })
                }
            };

            StackLayout warmthSliderLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children =
                {

                    (warmthSlider = new Slider(0.0, 1.0, 0.0)
                    {
                        Margin = new Thickness(10, 0, 0, 0),
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    }),

                    (warmView = new ContentView()
                    {
                        BackgroundColor = Color.FromRgba((329.698727446 * Math.Pow((15000 / 100) - 60, -0.1332047592)) / 255.0,
                            (329.698727446 * Math.Pow((15000 / 100) - 60, -0.1332047592)) / 255.0, 1.0, 1.0),
                        Margin = new Thickness(0, 0, 15, 0),
                        HeightRequest = 30.0,
                        WidthRequest = 30.0,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Start
                    })
                }
            };

            StackLayout partyModeIOLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 20, 20, 0),
                        Text = "Party Mode!",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },

                    (partySwitch = new Switch
                    {
                        Margin = new Thickness(20, 20, 10, 0),
                        IsToggled = false
                    })
                }
            };

            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,

                Children = {
                    (somulBanner = new Image
                    {
                        Source = Device.RuntimePlatform == Device.Android ? ImageSource.FromFile("app_banner_medium.png") : ImageSource.FromFile("Images/app_banner_medium.png")
                    }),
                    bluetoothIOLayout,
                    new Label
                    {
                        Margin = new Thickness(20, 10 , 20, 0),
                        Text = "Lamp Intensity",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },
                    intensitySliderLayout,
                    new Label
                    {
                        Margin = new Thickness(20, 10, 20, 0),
                        Text = "Lamp Hue",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },
                    hueSliderLayout,
                    new Label
                    {
                        Margin = new Thickness(20, 10, 20, 0),
                        Text = "Light Warmth",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },
                    warmthSliderLayout,
                    partyModeIOLayout
                }
            };

            bluetoothButton.Pressed += BluetoothButtonPressed;
            intensitySlider.ValueChanged += IntensitySliderChanged;
            hueSlider.ValueChanged += HueSliderChanged;
            warmthSlider.ValueChanged += WarmthSliderChanged;
            partySwitch.Toggled += PartySwitchToggled;
        }

        private void PartySwitchToggled(object sender, ToggledEventArgs e)
        {
            if (partySwitch.IsToggled)
            {
                dataToSend = "p";
                System.Console.WriteLine("YEET");
            }
            else
            {
                dataToSend = "s";
                System.Console.WriteLine("RIP");
            }
            bluetoothConnect.WriteData(dataToSend);
        }

        private void BluetoothButtonPressed(object sender, EventArgs e)
        {
            if (bluetoothButton.Text.Equals("Connect") && bluetoothConnect.Connect())
                bluetoothButton.Text = "Disconnect";
            else if (bluetoothButton.Text.Equals("Disconnect") && bluetoothConnect.Disconnect())
                bluetoothButton.Text = "Connect";
        }

        // TODO: Set the initial slider to whatever the lamp's intensity is
        // Snap the slider to 101 different discontinuous values (0-100%)
        private void IntensitySliderChanged(object sender, ValueChangedEventArgs e)
        {
            intensitySlider.Value = Math.Round(e.NewValue, 0);

            // Display the percentage of intensity of the lamp
            if ((intensitySlider.Value != 0) && (intensitySlider.Value != 100))
            {
                sliderLabel.Text = intensitySlider.Value.ToString() + " %";
            }
            // Display "Off" and "Max" for readablility
            else if (intensitySlider.Value == 0)
            {
                sliderLabel.Text = " Off";
            }
            else
            {
                sliderLabel.Text = " Max";
            }

            dataToSend = "i" + intensitySlider.Value.ToString() + "\0";
            bluetoothConnect.WriteData(dataToSend);
        }

        // Changes the color viewer's hue in real time
        private void HueSliderChanged(object sender, ValueChangedEventArgs e)
        {
            lampColor = hueView.BackgroundColor = Color.FromHsla(e.NewValue, 1.0, 0.5, 1.0);

            dataToSend = "h" + ScaleColor(lampColor.R) + ":" + ScaleColor(lampColor.G) + ":" 
                + ScaleColor(lampColor.B) + "\0";
            bluetoothConnect.WriteData(dataToSend);
        }

        // Changes the warmth viewer's color temperature in real time
        private void WarmthSliderChanged(object sender, ValueChangedEventArgs e)
        {
            double red;
            double green;
            double blue;

            long minTemp = 1500;
            long maxTemp = 15000;

            // Converts slider value that is between 0-0.875 to a temperature value
            // that is between 1500-15000 to run a bounded set of functions on.
            // But, because we have to go from cold to hot, all of that is subtracted
            // from our maximum temperature
            double temp = maxTemp - (((e.NewValue * tempPercentage) * (maxTemp - minTemp)) + minTemp);

            // Sets RGB for a given temperature range using a complex algorithm
            if (temp < 2000)
            {
                red = 255;
                green = (int)Math.Floor(99.4708025861 * Math.Log(temp / 100) - 161.1195681661);
                blue = 0;
            }
            else if (temp < 6500)
            {
                red = 255;
                green = (int)Math.Floor(99.4708025861 * Math.Log(temp / 100) - 161.1195681661);
                blue = (int)Math.Floor(138.5177312231 * Math.Log(temp / 100) - 305.0447927307);
            }
            else if (temp < 6600)
            {
                red = 255;
                green = (int)Math.Floor(288.1221695283 * (Math.Pow((temp/100) - 60, -0.0755148492)));
                blue = (int)Math.Floor(138.5177312231 * Math.Log(temp / 100) - 305.0447927307);
            }
            else
            {
                red = (int)Math.Floor(329.698727446 * Math.Pow((temp / 100) - 60, -0.1332047592));
                green = (int)Math.Floor(288.1221695283 * Math.Pow((temp / 100) - 60, -0.0755148492));
                blue = 255;
            }

            lampColor = warmView.BackgroundColor = Color.FromRgba(red / 255, green / 255, blue / 255, 1.0);
            dataToSend = "w" + red.ToString() + ":" + green.ToString() + ":" + blue.ToString() + "\0";
            bluetoothConnect.WriteData(dataToSend);
        }
        
        private string ScaleColor(double d)
        {
            return ((int)Math.Floor(d * 255)).ToString();
        }
    }
}