using color_palette_creator_v2;
using System.Collections.Generic;
using System.Text.Json;
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

    // Method to load BrightnessFactors
    public List<FactorItem> LoadBrightnessFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(BrightnessFactors), out object serializedList))
        {
            List<FactorItem> BrightnessFactorsList = JsonSerializer.Deserialize<List<FactorItem>>((string)serializedList);
            if (BrightnessFactorsList.Count >= 1)
            {
                return BrightnessFactorsList;
            }
            else
            {
                return new List<FactorItem>
                {
                    new FactorItem { valueFactor = 64, matchBrush = DataViewModel.GetBrightnessColor(64) },
                    new FactorItem { valueFactor = 96, matchBrush = DataViewModel.GetBrightnessColor(96) },
                    new FactorItem { valueFactor = 160, matchBrush = DataViewModel.GetBrightnessColor(160) },
                    new FactorItem { valueFactor = 192, matchBrush = DataViewModel.GetBrightnessColor(192) }
                };
            }
        }
        return new List<FactorItem>
                {
                    new FactorItem { valueFactor = 64, matchBrush = DataViewModel.GetBrightnessColor(64) },
                    new FactorItem { valueFactor = 96, matchBrush = DataViewModel.GetBrightnessColor(96) },
                    new FactorItem { valueFactor = 160, matchBrush = DataViewModel.GetBrightnessColor(160) },
                    new FactorItem { valueFactor = 192, matchBrush = DataViewModel.GetBrightnessColor(192) }
                };
    }

    // Method to save BrightnessFactors
    public void SaveBrightnessFactors(List<FactorItem> brightnessFactors)
    {
        string serializedList = JsonSerializer.Serialize(brightnessFactors);
        localSettings.Values[nameof(HueFactors)] = serializedList;
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
                return HueFactorsList;
            }
            else
            {
                return new List<FactorItem>
                {
                    new FactorItem { valueFactor = 0, matchBrush = DataViewModel.GetHueColor(0) },
                    new FactorItem { valueFactor = 30, matchBrush = DataViewModel.GetHueColor(30) },
                    new FactorItem { valueFactor = 60, matchBrush = DataViewModel.GetHueColor(60) },
                    new FactorItem { valueFactor = 90, matchBrush = DataViewModel.GetHueColor(90) }
                };
            }
        }
        return new List<FactorItem>
                {
                    new FactorItem { valueFactor = 0, matchBrush = DataViewModel.GetHueColor(0) },
                    new FactorItem { valueFactor = 30, matchBrush = DataViewModel.GetHueColor(30) },
                    new FactorItem { valueFactor = 60, matchBrush = DataViewModel.GetHueColor(60) },
                    new FactorItem { valueFactor = 90, matchBrush = DataViewModel.GetHueColor(90) }
                };
    }

    // Method to save HueFactors
    public void SaveHueFactors(List<FactorItem> HueFactors)
    {
        string serializedList = JsonSerializer.Serialize(HueFactors);
        localSettings.Values[nameof(HueFactors)] = serializedList;
    }
}
