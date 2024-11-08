using ColorMine.ColorSpaces;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace color_palette_creator_v2
{
    public class FactorItem
    {
        public int valueFactor { get; set; }

        // Non-serialized property to convert Color to Brush
        [JsonIgnore]
        public Brush matchBrush { get; set; }
    }


    public class DataViewModel : INotifyPropertyChanged
    {
        // Observable collection to hold brightness values
        public ObservableCollection<FactorItem> BrightnessFactors { get; set; } = new ObservableCollection<FactorItem>();
        public ObservableCollection<FactorItem> HueFactors { get; set; } = new ObservableCollection<FactorItem>();

        private AppSettings appSettings;
        private int brightnessFactor;
        public int BrightnessFactor


        {
            get => brightnessFactor;
            set
            {
                hueFactor = value < 0 ? 0 : value > 255 ? 255 : value;
                OnPropertyChanged(nameof(BrightnessFactor));
            }
        }

        private int hueFactor;
        public int HueFactor


        {
            get => hueFactor;
            set
            {
                hueFactor = value < 0 ? 0 : value > 360 ? 360 : value;
                OnPropertyChanged(nameof(HueFactor));
            }
        }


        public DataViewModel()
        {
            appSettings = new AppSettings();
            // Load the brightness factors from settings
            var loadedBrightnessFactors = appSettings.LoadBrightnessFactors();
            BrightnessFactors = new ObservableCollection<FactorItem>(loadedBrightnessFactors);
            BrightnessFactors.CollectionChanged += (s, e) => SaveBrightnessFactors();
            var loadedHueFactors = appSettings.LoadHueFactors();
            HueFactors = new ObservableCollection<FactorItem>(loadedHueFactors);
            HueFactors.CollectionChanged += (s, e) => SaveHueFactors();
        }

        // Method to add a new factor to the list
        public void AddBrightnessFactor()
        {
            if (brightnessFactor > 0) // Add only if brightnessFactor is valid
            {
                BrightnessFactors.Add(new FactorItem { valueFactor = brightnessFactor, matchBrush = GetBrightnessColor(brightnessFactor) });
                brightnessFactor = 0; // Reset input after adding
            }
        }

        // Method to remove a factor from the list
        public void RemoveBrightnessFactor(int factor)
        {
            var itemToRemove = BrightnessFactors.FirstOrDefault(f => f.valueFactor == factor);
            if (itemToRemove != null)
            {
                BrightnessFactors.Remove(itemToRemove);
            }
        }

        // Method to add a new factor to the list
        public void AddHueFactor()
        {
            if (hueFactor > 0) // Add only if brightnessFactor is valid
            {
                HueFactors.Add(new FactorItem { valueFactor = hueFactor, matchBrush = GetHueColor(hueFactor) });
                hueFactor = 0; // Reset input after adding
            }
        }

        // Method to remove a factor from the list
        public void RemoveHueFactor(int factor)
        {
            var itemToRemove = HueFactors.FirstOrDefault(f => f.valueFactor == factor);
            if (itemToRemove != null)
            {
                HueFactors.Remove(itemToRemove);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SaveBrightnessFactors()
        {
            appSettings.SaveBrightnessFactors(new List<FactorItem>(BrightnessFactors));
        }

        private void SaveHueFactors()
        {
            appSettings.SaveHueFactors(new List<FactorItem>(HueFactors));
        }

        public static Brush GetBrightnessColor(int brightness)
        {
            // Clamp the brightness value to ensure it’s within the range 0-255
            byte brightnessByte = (byte)Math.Clamp(brightness, 0, 255);

            // Create a grayscale color based on the clamped brightness value
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, brightnessByte, brightnessByte, brightnessByte));
        }
        public static Brush GetHueColor(int hue)
        {
            var hsl = new Hsl { H = hue, S = 1.0, L = 0.5 };
            var rgb = hsl.To<Rgb>();

            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B));

        }

    }
}
