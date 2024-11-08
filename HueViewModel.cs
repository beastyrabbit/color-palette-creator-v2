using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorMine.ColorSpaces;

namespace color_palette_creator_v2
{
    public class HueViewModel : INotifyPropertyChanged
    {
        // Observable collection to hold Hue values
        public ObservableCollection<int> HueFactors { get; set; } = new ObservableCollection<int>();

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

        public SolidColorBrush GetHueColor(int hue)
        {
            var hsl = new Hsl { H = hue, S = 1.0, L = 0.5 };
            var rgb = hsl.To<Rgb>();

            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B));

        }


        public HueViewModel()
        {
            appSettings = new AppSettings();
            // Load the brightness factors from settings
            var loadedFactors = appSettings.LoadHueFactors();
            HueFactors = new ObservableCollection<int>(loadedFactors);
            HueFactors.CollectionChanged += (s, e) => SaveHueFactors();
        }

        // Method to add a new factor to the list
        public void AddFactor()
        {
            if (newFactor > 0) // Add only if newFactor is valid
            {
                HueFactors.Add(newFactor);
                newFactor = 0; // Reset input after adding
            }
        }

        // Method to remove a factor from the list
        public void RemoveFactor(int factor)
        {
            if (HueFactors.Contains(factor))
            {
                HueFactors.Remove(factor);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SaveHueFactors()
        {
            appSettings.SaveHueFactors(new List<int>(HueFactors));
        }


    }
}
