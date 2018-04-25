using System;
using Xamarin.Forms;

namespace SomulApp
{
    public class GreetingsPage : ContentPage
    {
        // Elements to display on the content page
        private Image somulBanner;
        private Slider intensitySlider;
        private Slider hueSlider;
        private Slider warmthSlider;
        private Label sliderLabel;
        public Button bluetoothButton;
        public Button cancelAlarmButton;
        private ContentView hueView;
        private ContentView warmView;
        private Switch partySwitch;
        private DateTime time;
        private DateTime alarmTime;
        private TimePicker alarmPicker;

        // For connecting and sending bluetooth data
        private IBluetoothConnect bluetoothConnect;
        private string dataToSend;

        // For displaying toast messages
        IToastMessageDisplay messageDisplay;

        // For changing the visiblity of the cancel alarm button
        bool isAlarmSet = false;

        // For easy multiplication in the Arduino
        double scaledIntensity;

        // We never want the hot to be completely red, for asthetic reasons
        double tempPercentage = 0.9;
        Color lampColor = Color.Wheat;

        public GreetingsPage()
        {
            bluetoothConnect = DependencyService.Get<IBluetoothConnect>();
            messageDisplay = DependencyService.Get<IToastMessageDisplay>();

            BackgroundColor = SomulColors.PrimaryDarker;

            StackLayout bluetoothIOLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 20, 20, 0),
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        TextColor = SomulColors.Accent,
                        Text = "Connect Somul"
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

            StackLayout alarmLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 15, 10, 0),
                        Text = "Alarm",
                        HorizontalTextAlignment = TextAlignment.Start,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },

                    (alarmPicker = new TimePicker
                    {
                        Margin = new Thickness(10, 10, 15, 0),
                        BackgroundColor = SomulColors.PrimaryDarkDarkest,
                        TextColor = SomulColors.Accent,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    }),

                    (cancelAlarmButton = new Button
                    {
                        Margin = new Thickness(5, 10, 15, 0),
                        Text = "Cancel Alarm",
                        TextColor = SomulColors.Accent,
                        BackgroundColor = SomulColors.PrimaryDarkDarkest,
                        IsVisible = isAlarmSet
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
                        Source = Device.RuntimePlatform == Device.Android ? ImageSource.FromFile("app_banner_medium.png")
                            : ImageSource.FromFile("Images/app_banner_medium.png")
                    }),
                    bluetoothIOLayout,
                    alarmLayout,
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
            alarmPicker.PropertyChanged += AlarmPickerPropertyChanged; ;
            cancelAlarmButton.Pressed += CancelAlarmButtonPressed;
            intensitySlider.ValueChanged += IntensitySliderChanged;
            hueSlider.ValueChanged += HueSliderChanged;
            warmthSlider.ValueChanged += WarmthSliderChanged;
            partySwitch.Toggled += PartySwitchToggled;
        }

        // Send the picked alarm time and update GUI
        private void AlarmPickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Update the time for redundant accuracy
            time = DateTime.Now;
            dataToSend = "t" + AddLeadingZeros(time.ToLocalTime().Hour)
                + AddLeadingZeros(time.ToLocalTime().Minute) + AddLeadingZeros(time.ToLocalTime().Second);
            bluetoothConnect.WriteData(dataToSend);
            Console.WriteLine(dataToSend);

            if ((e.PropertyName == TimePicker.TimeProperty.PropertyName) && alarmPicker.IsFocused)
            {
                // Send alarm time
                dataToSend = "a" + AddLeadingZeros(alarmPicker.Time.Hours) + AddLeadingZeros(alarmPicker.Time.Minutes);
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                // Display toast for Android
                if (Device.RuntimePlatform == Device.Android)
                {
                    messageDisplay.ShowMessage("Alarm set for " + ((alarmPicker.Time.Hours > 12) ? (alarmPicker.Time.Hours - 12)
                        : alarmPicker.Time.Hours) + ":" + TimeDisplay(alarmPicker.Time.Minutes));
                }

                // Make the cancel alarm button visible
                isAlarmSet = true;
                cancelAlarmButton.IsVisible = true;
            }
        }

        // Handle canceled alarm data sending and GUI updates
        private void CancelAlarmButtonPressed(object sender, EventArgs e)
        {
            if (cancelAlarmButton.Text.Equals("Cancel Alarm"))
            {
                // Make the alarm cancel button invisible
                isAlarmSet = false;
                cancelAlarmButton.IsVisible = false;

                // Update the time for redundant accuracy
                time = DateTime.Now;
                dataToSend = "t" + AddLeadingZeros(time.ToLocalTime().Hour) + AddLeadingZeros(time.ToLocalTime().Minute)
                    + AddLeadingZeros(time.ToLocalTime().Second);
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                // Send the stop alarm command
                dataToSend = "as";
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                // Display toast for Android
                if (Device.RuntimePlatform == Device.Android)
                    messageDisplay.ShowMessage("Alarm canceled");
            }
        }
        
        // Handle connection to lamp
        private void BluetoothButtonPressed(object sender, EventArgs e)
        {
            if (bluetoothButton.Text.Equals("Connect") && bluetoothConnect.Connect())
            {
                // Update the time
                time = DateTime.Now;
                dataToSend = "t" + AddLeadingZeros(time.ToLocalTime().Hour) + AddLeadingZeros(time.ToLocalTime().Minute)
                    + AddLeadingZeros(time.ToLocalTime().Second);
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                // Set color to white
                dataToSend = "h255255255";
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                // Set intensity to zero
                dataToSend = "i0";
                bluetoothConnect.WriteData(dataToSend);
                Console.WriteLine(dataToSend);

                bluetoothButton.Text = "Disconnect";
            }
            else if (bluetoothButton.Text.Equals("Disconnect") && bluetoothConnect.Disconnect())
                bluetoothButton.Text = "Connect";
        }

        // Send partymode trigger
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
            Console.WriteLine(dataToSend);
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

            dataToSend = "i" + intensitySlider.Value.ToString();
            bluetoothConnect.WriteData(dataToSend);
            Console.WriteLine(dataToSend);
        }

        // Changes the color viewer's hue in real time
        private void HueSliderChanged(object sender, ValueChangedEventArgs e)
        {
            lampColor = hueView.BackgroundColor = Color.FromHsla(e.NewValue, 1.0, 0.5, 1.0);

            dataToSend = "h" + AddLeadingZeros(ScaleColor(lampColor.R)) + AddLeadingZeros(ScaleColor(lampColor.G))
                + AddLeadingZeros(ScaleColor(lampColor.B));
            bluetoothConnect.WriteData(dataToSend);
            Console.WriteLine(dataToSend);
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
                green = (int)Math.Floor(288.1221695283 * (Math.Pow((temp / 100) - 60, -0.0755148492)));
                blue = (int)Math.Floor(138.5177312231 * Math.Log(temp / 100) - 305.0447927307);
            }
            else
            {
                red = (int)Math.Floor(329.698727446 * Math.Pow((temp / 100) - 60, -0.1332047592));
                green = (int)Math.Floor(288.1221695283 * Math.Pow((temp / 100) - 60, -0.0755148492));
                blue = 255;
            }

            lampColor = warmView.BackgroundColor = Color.FromRgba(red / 255, green / 255, blue / 255, 1.0);
            dataToSend = "h" + AddLeadingZeros((int)red) + AddLeadingZeros((int)green) + AddLeadingZeros((int)blue);
            bluetoothConnect.WriteData(dataToSend);
            Console.WriteLine(dataToSend);
        }

        // Turns a color from 0-1 to 0-255
        private int ScaleColor(double d)
        {
            return (int)Math.Floor(d * 255);
        }

        // A way to ensure consistent parsing for sent data
        private string AddLeadingZeros(int i)
        {
            if (i < 10)
                return "00" + i.ToString();
            else if (i < 100)
                return "0" + i.ToString();
            else
                return i.ToString();
        }

        // A formater to display time correctly
        private string TimeDisplay(int i)
        {
            if (i >= 10)
                return i.ToString();
            else
                return "0" + i.ToString();
        }
    }
}