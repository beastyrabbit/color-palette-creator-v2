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



        public MainWindow()
        {
           appSettings = new AppSettings();

            this.InitializeComponent();
            this.Closed += OnWindowClosed;
            DataContext = new DataViewModel();
            setupButton();


        }

        private void setupButton()
        {
            UpdateResetSettingsButton(true,Microsoft.UI.Colors.Gray);
            UpdateSelectImageButton(true, Microsoft.UI.Colors.YellowGreen);
            UpdateSelectFolderButton(true, Microsoft.UI.Colors.YellowGreen);
            UpdateSelectColorPaletteButton(true, Microsoft.UI.Colors.Gray);
            UpdateSelectRefFileButton(true, Microsoft.UI.Colors.Gray);
            UpdateSubmitButton(false, Microsoft.UI.Colors.Red);

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
                await ShowDialog($"Image selected: {imagePath}");
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
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.SettingsIdentifier = "LastFolderPicker";
            openPicker.CommitButtonText = "Select Color Ref";

            // File types to be displayed
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hwnd);

            // Show the picker and get the selected file
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Check if the file extension is not ".jpeg"
                if (file.FileType.ToLower() != ".jpeg")
                {
                    // Process the selected file path and save it in local settings
                    string imagePath = file.Path;
                    ApplicationData.Current.LocalSettings.Values["LastSelectedRefFilePath"] = imagePath;
                    await ShowDialog($"Image selected: {imagePath}");
                }
                else
                {
                    await ShowDialog("JPEG files are not allowed.");
                }
            }
            else
            {
                await ShowDialog("No image was selected.");
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file save picker
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Downloads;

            // Custom "file type" to indicate location selection (placeholder extension)
            savePicker.FileTypeChoices.Add("Select Folder", new List<string>() { ".folder" });
            savePicker.SuggestedFileName = "ChooseThisFolder";

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Show the picker and get the selected folder path
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Retrieve the parent folder of the "file" to get the chosen folder path
                StorageFolder folder = await file.GetParentAsync();
                if (folder != null)
                {
                    string folderPath = folder.Path;

                    // Save the folder path for future use
                    ApplicationData.Current.LocalSettings.Values["LastSelectedFolderPath"] = folderPath;

                    await ShowDialog($"Folder selected: {folderPath}");
                }
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

    }
}
