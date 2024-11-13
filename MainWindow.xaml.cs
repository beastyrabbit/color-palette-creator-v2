using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using ColorMine.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using Windows.Devices.Haptics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace color_palette_creator_v2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly AppSettings appSettings;
        public DataViewModel DataContext { get; set; }
        public static ImgWorkerClass ImgWorker { get; } = new ImgWorkerClass();



        public MainWindow()
        {
           appSettings = new AppSettings();

            this.InitializeComponent();
            this.Closed += OnWindowClosed;
            DataContext = new DataViewModel();
            SetupButton();
            // Call the async method without ContinueWith
            _ = LoadAndProcessImageAsync();
        }

        private async Task LoadAndProcessImageAsync()
        {
            var imageFile = await appSettings.LoadImageFromLocalStorageAsync();

            if (imageFile != null)
            {
                var RefImageData = ImgWorkerClass.PrepareAndStoreImageAsRGBA(imageFile.FilePath);

                if (RefImageData != null)
                {
                    (bool isMatch, List<string> missingColors) = ImgWorkerClass.checkRefImage(RefImageData);

                    if (isMatch)
                    {
                        RefImageData.ColorMatrix = missingColors;
                        DataContext.RefImageData = RefImageData;
                        UpdateSelectRefFileButton(true, Microsoft.UI.Colors.YellowGreen);
                        DataContext.UseReferanceImage = true;
                    }
                    else
                    {
                        UpdateSelectRefFileButton(true, Microsoft.UI.Colors.Gray);
                        DataContext.UseReferanceImage = false;
                    }
                }
            }
        }
        private void SetupButton()
        {
            UpdateResetSettingsButton(true,Microsoft.UI.Colors.Gray);
            UpdateSelectFolderButton(true, Microsoft.UI.Colors.OrangeRed);
            UpdateSelectColorPaletteButton(true, Microsoft.UI.Colors.Gray);
            UpdateSubmitButton(false, Microsoft.UI.Colors.Black);
            UpdateButtonSettings();
            // On DataContect Property Changed Event do an Button Update
            // Attach to the PropertyChanged event
            DataContext.PropertyChanged += (sender, args) =>
            {
                UpdateButtonSettings();
            };




        }

        private void UpdateButtonSettings()
        {
            if (DataContext.ColorFactors.Count > 0)
            {
                UpdateSelectImageButton(true, Microsoft.UI.Colors.YellowGreen);
                ColorFactorstoggle.IsEnabled = true;
            }
            else
            {
                UpdateSelectImageButton(true, Microsoft.UI.Colors.OrangeRed);
                ColorFactorstoggle.IsEnabled = false;
            }

            if(DataContext.RefImageData != null)
            {
                UpdateSelectRefFileButton(true, Microsoft.UI.Colors.YellowGreen);
                RefFileToggle.IsEnabled = true;
            }
            else
            {
                UpdateSelectRefFileButton(true, Microsoft.UI.Colors.Black);
                RefFileToggle.IsEnabled = false;
                RefFileToggle.IsOn = false;
            }

            // Submit Button settings
            // Enable if OutputFolderSelected is not null
            // Enable if ColorFactors are present and ColorFactorstoggle is true
            // Enable if InputFileSelected Count is greater than 0
            if (DataContext.OutputFolderSelected != null)
            {
                if (DataContext.ColorFactors.Count > 0 && ColorFactorstoggle.IsOn == true)
                {
                    UpdateSubmitButton(true, Microsoft.UI.Colors.YellowGreen);
                }
                else if(DataContext.InputFileSelected != null && DataContext.InputFileSelected.Count > 0)
                {
                    UpdateSubmitButton(true, Microsoft.UI.Colors.YellowGreen);
                }
                else
                {
                    UpdateSubmitButton(false, Microsoft.UI.Colors.Black);
                }
            }


        }

        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            // Save settings

        }

        private async Task ShowDialog(string content)
        {
            var dialog = new ContentDialog
            {
                Title = "Notification",
                Content = content,
                CloseButtonText = "Ok",
                XamlRoot = this.Content.XamlRoot // Required for WinUI 3 to attach dialog to the window
            };
            await dialog.ShowAsync();
        }



        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic for Submit button

        }

        private async void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file picker
            var openPicker = new FileOpenPicker();

                // Set the suggested start location to the last used folder
                openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                openPicker.SettingsIdentifier = "LastFolderPicker";
                openPicker.CommitButtonText = "Select Folder";


            // File types to be displayed
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hwnd);

            // Show the picker and get the selected file
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {

                // Process the selected file path
                string imagePath = files[0].Path;
                await ShowDialog($"Image selected: {imagePath}");
                UpdateSelectRefFileButton(true, Microsoft.UI.Colors.YellowGreen);
            }
            else
            {
                await ShowDialog("No image was selected.");
            }
        }

        private async void SelectRefFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file picker
            var openPicker = new FileOpenPicker();

            // Set the suggested start location to the last used folder
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.SettingsIdentifier = "LastFolderPicker";
            openPicker.CommitButtonText = "Select Color Palette";


            // File types to be displayed
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hwnd);


            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Process the selected file path
                string imagePath = file.Path;

                var prefedFile = ImgWorkerClass.PrepareAndStoreImageAsRGBA(imagePath);
                if (prefedFile != null)
                {
                    (bool isMatch, List<string> missingColors) = ImgWorkerClass.checkRefImage(prefedFile);
                    if (isMatch)
                    {
                        prefedFile.ColorMatrix = missingColors;
                        DataContext.RefImageData = prefedFile;
                        _ = appSettings.SaveImageToLocalStorageAsync(file);
                    }
                    else
                    {
                        await ShowDialog($"Image is not a valid color palette. Missing colors: {string.Join(", ", missingColors)}");
                    }

                }
                await ShowDialog($"Image selected: {imagePath}");
            }
            else
            {
                await ShowDialog("No image was selected.");
            }
        }

        private async void SelectColorPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file picker
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.SettingsIdentifier = "LastFolderPicker";
            openPicker.CommitButtonText = "Select Color Ref";

            // File types to be displayed
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".tif");
            openPicker.FileTypeFilter.Add(".tiff");
            openPicker.FileTypeFilter.Add(".webp");
            openPicker.FileTypeFilter.Add(".ico");
            openPicker.FileTypeFilter.Add(".pbm");
            openPicker.FileTypeFilter.Add(".pgm");
            openPicker.FileTypeFilter.Add(".ppm");

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hwnd);

            // Show the picker and get the selected file
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                List<Rgba32> listofColors = ImgWorker.GetRefSheetColors(file.Path);
                // Add to ColorFactors remove Alpha
                if (listofColors != null) {
                    foreach (var color in listofColors)
                    {

                        // Convert to WPF color for SolidColorBrush
                        var wpfColor = Windows.UI.Color.FromArgb(
                            color.A,
                            color.R,
                            color.G,
                            color.B
                        );
                        var colorFacttor = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
                        DataContext.AddColorFactor(new FactorItem { valueFactor = colorFacttor, matchBrush = new SolidColorBrush(wpfColor) });
                    }
                }

            }
            else
            {
                await ShowDialog("No image was selected.");
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Initialize the folder picker
            var folderPicker = new FolderPicker();

            // Associate with the current window
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            // Set file type filter (required, even for folders)
            folderPicker.FileTypeFilter.Add("*");
            // Set to last used folder
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            // Get the Downloads folder path
            StorageFolder downloadsFolder = KnownFolders.PicturesLibrary;
            string downloadsPath = downloadsFolder.Path;

            StorageFolder initialFolder = await StorageFolder.GetFolderFromPathAsync(@"C:\Users\YourUser\Documents");

            if (initialFolder != null)
            {
                folderPicker.SettingsIdentifier = initialFolder.Path;
            }

            // Show the folder picker
            StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();
            if (selectedFolder != null)
            {
                string folderPath = selectedFolder.Path;

                // Save the folder path for future use
                DataContext.OutputFolderSelected = folderPath;

                await ShowDialog($"Folder selected: {folderPath}");
            }
            else
            {
                await ShowDialog("No folder was selected.");
            }
        }

        private void AddNewHueFactor_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddHueFactor();
        }

        private void AddNewBrightnessFactor_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddBrightnessFactor();
        }


        private void RemoveBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem)?.DataContext is FactorItem factor)
            {
                DataContext.RemoveBrightnessFactor(factor);
            }
        }

        private void RemoveHueButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem)?.DataContext is FactorItem factor)
            {
                DataContext.RemoveHueFactor(factor);
            }
        }

        private void AddNewColorFactor_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddColorFactor();
        }


        private void RemoveColorButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem)?.DataContext is FactorItem factor)
            {
                DataContext.RemoveColorFactor(factor);
            }
        }
        private void RemoveAllBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
                DataContext.RemoveAllBrightnessFactors();
        }
        private void RemoveAllHueButton_Click(object sender, RoutedEventArgs e)
        {
                DataContext.RemoveAllHueFactors();  
        }
        private void RemoveAllColorButton_Click(object sender, RoutedEventArgs e)
        {
                DataContext.RemoveAllColorFactors();
        }

        private void RemoveRefFileButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext.RefImageData = null;
            UpdateSelectRefFileButton(true, Microsoft.UI.Colors.Gray);
            _ = appSettings.DeleteImageFromLocalStorageAsync();
        }


        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            appSettings.resetSettings();
            DataContext.ResetandReload();
        }

        private void UpdateSelectImageButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            SelectImageButton.IsEnabled = isEnabled;
            SelectImageButton.Background = new SolidColorBrush(backgroundColor);
        }

        // Method to update SelectFolderButton properties
        private void UpdateSelectFolderButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            SelectFolderButton.IsEnabled = isEnabled;
            SelectFolderButton.Background = new SolidColorBrush(backgroundColor);
        }

        // Method to update SelectColorPalette button properties
        private void UpdateSelectColorPaletteButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            SelectColorPalette.IsEnabled = isEnabled;
            SelectColorPalette.Background = new SolidColorBrush(backgroundColor);
        }

        // Method to update SelectRefFile button properties
        private void UpdateSelectRefFileButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            SelectRefFile.IsEnabled = isEnabled;
            SelectRefFile.Background = new SolidColorBrush(backgroundColor);
        }

        // Method to update Submit button properties
        private void UpdateSubmitButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            submitButton.IsEnabled = isEnabled; 
            submitButton.Background = new SolidColorBrush(backgroundColor);
        }

        // Method to update ResetSettings button properties
        private void UpdateResetSettingsButton(bool isEnabled, Windows.UI.Color backgroundColor)
        {
            ResetSettings.IsEnabled = isEnabled;
            ResetSettings.Background = new SolidColorBrush(backgroundColor);
        }

        void OnHueVariantItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem)
            {
                string variant = menuItem.Text; // Access the Text property
                switch (variant)
            {
                case "Compl":
                    // Complementary: ±180°
                    DataContext.AddHueFactor(180);
                    DataContext.AddHueFactor(-180);
                    break;

                case "Triadic":
                    // Triadic: ±120°
                    DataContext.AddHueFactor(120);
                    DataContext.AddHueFactor(-120);
                    break;

                case "Split":
                    // Split-Complementary: ±150°
                    DataContext.AddHueFactor(150);
                    DataContext.AddHueFactor(-150);
                    break;

                case "Tetradic":
                    // Tetradic: ±90° and ±180°
                    DataContext.AddHueFactor(90);
                    DataContext.AddHueFactor(-90);
                    DataContext.AddHueFactor(180);
                    DataContext.AddHueFactor(-180);
                    break;

                default:
                    throw new ArgumentException("Unknown variant: " + variant);
                }
            }
        }

        private async void RndImageButton_Click(object sender, RoutedEventArgs e)
        {
            string imageUrl = await ImgWorker.GetRandomFeaturedImageUrl();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                // Display the image URL (or directly bind it to an Image control if you want)
                // Add the image URL to the InputFileSelected list
                DataContext.InputFileSelected.Add(imageUrl);
                // Popup winfow with img
                await ShowDialog($"Random image fetched: {imageUrl}");
            }
            else
            {
                // Pop windows with img 
                await ShowDialog("Failed to fetch a random image.");
            }

        }
    }
}
