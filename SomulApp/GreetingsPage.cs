using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SomulApp
{
    public class GreetingsPage : ContentPage
    {
        private Slider intensitySlider;
        private Slider hueSlider;
        private Label sliderLabel;
        private ContentView hueView;

        public GreetingsPage()
        {
            BackgroundColor = SomulColors.PrimaryDarker;

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

            StackLayout bluetoothIOLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 20, 20, 0),
                        Text = "Bluetooth",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        TextColor = SomulColors.Accent
                    },

                    new Switch
                    {
                        Margin = new Thickness(20, 20, 10, 0),
                        IsToggled = false
                    }
                }
            };

            StackLayout bluetoothPickerLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Children = {
                    new MenuItem
                    {
                        Text = "Bluetooth Menu"
                    }
                }
            };

            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,

                Children = {

                    new Label
                    {
                        Margin = new Thickness(20, 20 , 20, 0),
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

                    bluetoothIOLayout
                }
            };

            intensitySlider.ValueChanged += IntensitySliderChanged;
            hueSlider.ValueChanged += HueSliderChanged;

        }

        // Changes the color viewer's hue in real time
        private void HueSliderChanged(object sender, ValueChangedEventArgs e)
        {
            hueView.BackgroundColor = Color.FromHsla(e.NewValue, 0.5, 0.5, 1.0);
        }

        // CANNOTDO: Set the initial slider to whatever the lamp's intensity is
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
        }
    }
}