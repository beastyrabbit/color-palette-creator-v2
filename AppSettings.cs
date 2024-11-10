using color_palette_creator_v2;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;


public class AppSettings
{
    private readonly ApplicationDataContainer localSettings;

    public AppSettings()
    {
        localSettings = ApplicationData.Current.LocalSettings;
    }

    public void resetSettings()
    {
        //Remove complete local settings
        localSettings.Values.Clear();

    }
   

    // Property to store DominantColorCount
    public int DominantColorCount
    {
        get => (int)(localSettings.Values[nameof(DominantColorCount)] ?? 10);
        set => localSettings.Values[nameof(DominantColorCount)] = value;
    }



    // Property to store HighlightColorCount
    public int HighlightColorCount
    {
        get => (int)(localSettings.Values[nameof(HighlightColorCount)] ?? 10);
        set => localSettings.Values[nameof(HighlightColorCount)] = value;
    }

    // Property to store BrightnessFactors
    public List<FactorItem> BrightnessFactors
    {
        get => LoadBrightnessFactors();
        set => SaveBrightnessFactors(value);
    }


public async Task SaveImageToLocalStorageAsync(StorageFile sourceFile, string fileName = "RefImage.png")
{
    // Get the app's local storage folder
    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    // Copy the image file to local storage
    await sourceFile.CopyAsync(localFolder, fileName, NameCollisionOption.ReplaceExisting);
}

    public async Task<RefImageData> LoadImageFromLocalStorageAsync(string fileName = "RefImage.png")
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        try
        {
            // Get the image file by name from local storage
            StorageFile imageFile = await localFolder.GetFileAsync(fileName);

            // Use an image decoding library if you need to get the width and height
            using (var stream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                return new RefImageData
                {
                    Width = (int)decoder.PixelWidth,
                    Height = (int)decoder.PixelHeight,
                    FilePath = imageFile.Path
                };
            }
        }
        catch (FileNotFoundException)
        {
            // Handle the case where the image file does not exist
            return null;
        }
    }


    // Method to load BrightnessFactors
    public List<FactorItem> LoadBrightnessFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(BrightnessFactors), out object serializedList))
        {
            List<FactorItem> BrightnessFactorsList = JsonSerializer.Deserialize<List<FactorItem>>((string)serializedList);
            if (BrightnessFactorsList.Count >= 1)
            {
                foreach (var item in BrightnessFactorsList)
                {
                    item.matchBrush = DataViewModel.GetBrightnessColor(item.IntValueFactor); // Assign the conversion function
                }
                return BrightnessFactorsList;
            }
            else
            {
                return new List<FactorItem>
                {
                    new FactorItem { IntValueFactor = 64, matchBrush = DataViewModel.GetBrightnessColor(64) },
                    new FactorItem { IntValueFactor = 96, matchBrush = DataViewModel.GetBrightnessColor(96) },
                    new FactorItem { IntValueFactor = 160, matchBrush = DataViewModel.GetBrightnessColor(160) },
                    new FactorItem { IntValueFactor = 192, matchBrush = DataViewModel.GetBrightnessColor(192) }
                };
            }
        }
        return new List<FactorItem>
                {
                    new FactorItem { IntValueFactor = 64, matchBrush = DataViewModel.GetBrightnessColor(64) },
                    new FactorItem { IntValueFactor = 96, matchBrush = DataViewModel.GetBrightnessColor(96) },
                    new FactorItem { IntValueFactor = 160, matchBrush = DataViewModel.GetBrightnessColor(160) },
                    new FactorItem { IntValueFactor = 192, matchBrush = DataViewModel.GetBrightnessColor(192) }
                };
    }

    // Method to save BrightnessFactors
    public void SaveBrightnessFactors(List<FactorItem> brightnessFactors)
    {
        string serializedList = JsonSerializer.Serialize(brightnessFactors);
        localSettings.Values[nameof(brightnessFactors)] = serializedList;
    }



    // Property to store HueFactors
    public List<FactorItem> HueFactors
    {
        get => LoadHueFactors();
        set => SaveHueFactors(value);
    }

    // Method to load HueFactors
    public List<FactorItem> LoadHueFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(HueFactors), out object serializedList))
        {
            List<FactorItem> HueFactorsList = JsonSerializer.Deserialize<List<FactorItem>>((string)serializedList);
            if (HueFactorsList.Count >= 1)
            {
                foreach (var item in HueFactorsList)
                {
                    item.matchBrush = DataViewModel.GetHueColor(item.IntValueFactor); // Assign the conversion function
                }
                return HueFactorsList;
            }
            else
            {
                return new List<FactorItem>
                {
                    new FactorItem { IntValueFactor = 0, matchBrush = DataViewModel.GetHueColor(0) },
                    new FactorItem { IntValueFactor = 30, matchBrush = DataViewModel.GetHueColor(30) },
                    new FactorItem { IntValueFactor = 60, matchBrush = DataViewModel.GetHueColor(60) },
                    new FactorItem { IntValueFactor = 90, matchBrush = DataViewModel.GetHueColor(90) }
                };
            }
        }
        return new List<FactorItem>
                {
                    new FactorItem { IntValueFactor = 0, matchBrush = DataViewModel.GetHueColor(0) },
                    new FactorItem { IntValueFactor = 30, matchBrush = DataViewModel.GetHueColor(30) },
                    new FactorItem { IntValueFactor = 60, matchBrush = DataViewModel.GetHueColor(60) },
                    new FactorItem { IntValueFactor = 90, matchBrush = DataViewModel.GetHueColor(90) }
                };
    }

   // Method to save HueFactors
    public void SaveHueFactors(List<FactorItem> HueFactors)
    {
        string serializedList = JsonSerializer.Serialize(HueFactors);
        localSettings.Values[nameof(HueFactors)] = serializedList;
    }

    public List<FactorItem> ColorFactors
    {
        get => LoadColorFactors();
        set => SaveColorFactors(value);
    }

    public void SaveColorFactors(List<FactorItem> ColorFactors)
    {
        string serializedList = JsonSerializer.Serialize(ColorFactors);
        localSettings.Values[nameof(ColorFactors)] = serializedList;
    }


    public List<FactorItem> LoadColorFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(ColorFactors), out object serializedList))
        {
            List<FactorItem> ColorFactorsList = JsonSerializer.Deserialize<List<FactorItem>>((string)serializedList);

            if (ColorFactorsList != null && ColorFactorsList.Count >= 1)
            {
                foreach (var item in ColorFactorsList)
                {
                    string hex = item.valueFactor.TrimStart('#');

                    // Determine if the hex string includes an alpha component
                    byte a = hex.Length == 8 ? byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) : byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber); ;
                    byte r = hex.Length == 8 ? byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) : byte.Parse(hex.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                    byte g = hex.Length == 8 ? byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) : byte.Parse(hex.Substring(2, 6), System.Globalization.NumberStyles.HexNumber);
                    byte b = hex.Length == 8 ? byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) : byte.Parse(hex.Substring(4, 8), System.Globalization.NumberStyles.HexNumber);

                    item.matchBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
                }
                return ColorFactorsList;
            }
        }
        return new List<FactorItem>();
    }

}
