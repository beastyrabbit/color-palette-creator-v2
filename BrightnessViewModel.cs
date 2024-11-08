using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace color_palette_creator_v2
{
    public class BrightnessViewModel : INotifyPropertyChanged
    {
        // Observable collection to hold brightness values
        public ObservableCollection<int> BrightnessFactors { get; set; } = new ObservableCollection<int>();

        private AppSettings appSettings;
        private int newFactor;
        public int NewFactor


        {
            get => newFactor;
            set
            {
                newFactor = value;
                OnPropertyChanged(nameof(NewFactor));
            }
        }

        public SolidColorBrush GetBrightnessColor(int brightness)
        {
            // Clamp the brightness value to ensure it’s within the range 0-255
            byte brightnessByte = (byte)Math.Clamp(brightness, 0, 255);

            // Create a grayscale color based on the clamped brightness value
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, brightnessByte, brightnessByte, brightnessByte));
        }


        public BrightnessViewModel()
        {
            appSettings = new AppSettings();
            // Load the brightness factors from settings
            var loadedFactors = appSettings.LoadBrightnessFactors();
            BrightnessFactors = new ObservableCollection<int>(loadedFactors);
            BrightnessFactors.CollectionChanged += (s, e) => SaveBrightnessFactors();
        }

        // Method to add a new factor to the list
        public void AddFactor()
        {
            if (newFactor > 0) // Add only if newFactor is valid
            {
                BrightnessFactors.Add(newFactor);
                newFactor = 0; // Reset input after adding
            }
        }

        // Method to remove a factor from the list
        public void RemoveFactor(int factor)
        {
            if (BrightnessFactors.Contains(factor))
            {
                BrightnessFactors.Remove(factor);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SaveBrightnessFactors()
        {
            appSettings.SaveBrightnessFactors(new List<int>(BrightnessFactors));
        }


    }
}
