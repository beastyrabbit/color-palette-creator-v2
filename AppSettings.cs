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
    public List<int> BrightnessFactors
    {
        get => LoadBrightnessFactors();
        set => SaveBrightnessFactors(value);
    }

    // Method to load BrightnessFactors
    public List<int> LoadBrightnessFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(BrightnessFactors), out object serializedList))
        {
            List<int> BrightnessFactorsList = JsonSerializer.Deserialize<List<int>>((string)serializedList);
            if (BrightnessFactorsList.Count >= 1)
            {
                return BrightnessFactorsList;
            }
            else
            {
              return  new List<int> { 64, 96, 160, 192 };
            }
        }
        return new List<int> { 64, 96, 160, 192 }; // Return an empty list if no data is found
    }

    // Method to save BrightnessFactors
    public void SaveBrightnessFactors(List<int> brightnessFactors)
    {
        string serializedList = JsonSerializer.Serialize(brightnessFactors);
        localSettings.Values[nameof(HueFactors)] = serializedList;
    }



    // Property to store HueFactors
    public List<int> HueFactors
    {
        get => LoadHueFactors();
        set => SaveHueFactors(value);
    }

    // Method to load HueFactors
    public List<int> LoadHueFactors()
    {
        if (localSettings.Values.TryGetValue(nameof(HueFactors), out object serializedList))
        {
            List<int> HueFactorsList = JsonSerializer.Deserialize<List<int>>((string)serializedList);
            if (HueFactorsList.Count >= 1)
            {
                return HueFactorsList;
            }
            else
            {
                return new List<int> { 0, 30, 60, 90 };
            }
        }
        return new List<int> { 0, 30, 60, 90 }; // Return an empty list if no data is found
    }

    // Method to save HueFactors
    public void SaveHueFactors(List<int> HueFactors)
    {
        string serializedList = JsonSerializer.Serialize(HueFactors);
        localSettings.Values[nameof(HueFactors)] = serializedList;
    }
}
