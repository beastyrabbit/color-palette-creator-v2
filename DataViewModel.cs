using ColorMine.ColorSpaces;
using Microsoft.UI.Xaml.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace color_palette_creator_v2
{
    public class FactorItem
    {
        // Private backing field to store the value as a string
        private string _valueFactor;

        // Public property to store and retrieve the value as a string
        public string valueFactor
        {
            get => _valueFactor;
            set => _valueFactor = value;
        }


        // Integer interface for ValueFactor, converting to and from string
        [JsonIgnore]
        public int IntValueFactor
        {
            get => int.TryParse(_valueFactor, out int result) ? result : 0; // Default to 0 if parse fails
            set => _valueFactor = value.ToString(); // Store the integer as a string
        }

        // Non-serialized property to convert Color to Brush
        [JsonIgnore]
        public Brush matchBrush { get; set; }
    }


    public class RefImageData
    {
        // Original properties for image dimensions and pixel data
        public int Width { get; set; }
        public int Height { get; set; }
        public Image<Rgba32> Image { get; set; }
        public List<string> ColorMatrix { get; set; }

        // New property to store the file path of the image in local storage
        public string FilePath { get; set; }
    }

    public class DataViewModel : INotifyPropertyChanged
    {
        // Observable collection to hold brightness values
        public ObservableCollection<FactorItem> BrightnessFactors { get; set; } = new ObservableCollection<FactorItem>();
        public ObservableCollection<FactorItem> HueFactors { get; set; } = new ObservableCollection<FactorItem>();
        public ObservableCollection<FactorItem> ColorFactors { get; set; } = new ObservableCollection<FactorItem>();
        public bool AddColorFactors  { get; set; }
        public bool UseReferanceImage
        {
            get => useReferanceImage;
            set
            {
                useReferanceImage = value;
                OnPropertyChanged(nameof(UseReferanceImage));
            }
        }
        private bool useReferanceImage;
        public bool DownSizeImage  { get; set; }

    private AppSettings appSettings;
        private int brightnessFactor;
        public int BrightnessFactor


        {
            get => brightnessFactor;
            set
            {
                brightnessFactor = value < -255 ? 0 : value > 255 ? 360 : value;
                OnPropertyChanged(nameof(BrightnessFactor));
            }
        }

        

        private int hueFactor;
        public int HueFactor


        {
            get => hueFactor;
            set
            {
                hueFactor = value < -360 ? 0 : value > 360 ? 360 : value;
                OnPropertyChanged(nameof(HueFactor));
            }
        }

        private Windows.UI.Color colorFactor ;
        public Windows.UI.Color ColorFactor
        {
            get => colorFactor;
            set
            {
                if (colorFactor != value)
                {
                    colorFactor = value;
                    OnPropertyChanged(nameof(ColorFactor));
                }
            }
        }

        private string hexColorPicked = "#FFFFFFFF";
        public string HexColorPicked
        {
            get => hexColorPicked;
            set
            {
                if (hexColorPicked != value)
                {
                    hexColorPicked = value;
                    OnPropertyChanged(nameof(HexColorPicked));
                }
            }
        }
        public List<string> InputFileSelected { get; set; }
        public string OutputFolderSelected { get; set; }

        private Windows.UI.Color colorPicked = Windows.UI.Color.FromArgb(255, 255, 255, 255);
        public Windows.UI.Color ColorPicked
        {
            get => colorPicked;
            set
            {
                colorPicked = value;
                HexColorPicked = $"#{colorPicked.A:X2}{colorPicked.R:X2}{colorPicked.G:X2}{colorPicked.B:X2}";
            }
        }

        public RefImageData RefImageData { get; set; }


        public DataViewModel()
        {
            appSettings = new AppSettings();
            // Load collections directly from settings
            BrightnessFactors = new ObservableCollection<FactorItem>(appSettings.LoadBrightnessFactors());
            BrightnessFactors.CollectionChanged += (s, e) =>
            {
                SaveBrightnessFactors();
            };

            HueFactors = new ObservableCollection<FactorItem>(appSettings.LoadHueFactors());
            HueFactors.CollectionChanged += (s, e) =>
            {
                SaveHueFactors();
            };

            ColorFactors = new ObservableCollection<FactorItem>(appSettings.LoadColorFactors());
            ColorFactors.CollectionChanged += (s, e) =>
            {
                SaveColorFactors();
            };
        }

        public void ResetandReload()
        {
            BrightnessFactors.Clear(); // Clear existing items
            var loadedBrightnessFactors = appSettings.LoadBrightnessFactors();
            foreach (var item in loadedBrightnessFactors)
            {
                BrightnessFactors.Add(item); // Append reloaded items
            }
            HueFactors.Clear(); // Clear existing items
            var loadedHueFactors = appSettings.LoadHueFactors();
            foreach (var item in loadedHueFactors)
            {
                HueFactors.Add(item); // Append reloaded items
            } 
            ColorFactors.Clear(); // Clear existing items
            var loadedColorFactors = appSettings.LoadColorFactors();
            foreach (var item in loadedColorFactors)
            {
                ColorFactors.Add(item); // Append reloaded items
            }
        }

        // Method to add a new factor to the list
        public void AddBrightnessFactor()
        {
                BrightnessFactors.Add(new FactorItem { IntValueFactor = brightnessFactor, matchBrush = GetBrightnessColor(brightnessFactor) });
                BrightnessFactor = 0; // Reset input after adding
        }
        // Method to remove a factor from the list
        public void RemoveBrightnessFactor(FactorItem factor)
        {
            // var itemToRemove = BrightnessFactors.FirstOrDefault(factor);
            BrightnessFactors.Remove(factor);
        }
        // Method to add a new factor to the list
        public void AddColorFactor()
        {
            
                ColorFactors.Add(new FactorItem { valueFactor = hexColorPicked, matchBrush = new SolidColorBrush(ColorPicked) });
                ColorFactor = default; // Reset input after adding
            hexColorPicked = "#FFFFFFFF";


        }

        // Method to remove a factor from the list
        public void RemoveColorFactor(FactorItem factor)
        {
            ColorFactors.Remove(factor);
        }



        // Method to add a new factor to the list
        public void AddHueFactor()
        {
                HueFactors.Add(new FactorItem { IntValueFactor = hueFactor, matchBrush = GetHueColor(hueFactor) });
                HueFactor = 0; // Reset input after adding
        }

        // Method to remove a factor from the list
        public void RemoveHueFactor(FactorItem factor)
        {
            //var itemToRemove = HueFactors.FirstOrDefault(factor);
            HueFactors.Remove(factor);
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


        private void SaveColorFactors()
        {
            appSettings.SaveColorFactors(new List<FactorItem>(ColorFactors));
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

            // Set the target color in HSL with the specified hue, full saturation, and medium lightness
            var hsl = new Hsl { H = hue % 360, S = 1.0, L = 0.5 };
            var rgb = hsl.To<Rgb>();

            // Convert the RGB values to a SolidColorBrush
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B));

        }

        public void RemoveAllBrightnessFactors()
        {
            BrightnessFactors.Clear();
        }
        public void RemoveAllHueFactors()
        {
            HueFactors.Clear();
        }
        public void RemoveAllColorFactors()
        {
            ColorFactors.Clear();
        }

        internal void AddColorFactor(FactorItem factorItem)
        {
            ColorFactors.Add(factorItem);
        }

        internal void AddHueFactor(int hueFactor)
        {
            HueFactors.Add(new FactorItem { IntValueFactor = hueFactor, matchBrush = GetHueColor(hueFactor) });
            HueFactor = 0; // Reset input after adding
        }
    }
}
