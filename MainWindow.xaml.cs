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
        public BrightnessViewModel BrightnessViewModel { get; set; }
        public HueViewModel HueViewModel { get; set; }


        public MainWindow()
        {
           appSettings = new AppSettings();

            this.InitializeComponent();
            this.Closed += OnWindowClosed;
            BrightnessViewModel = new BrightnessViewModel();
            HueViewModel = new HueViewModel();
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
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

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
                // Process the selected file path
                string imagePath = file.Path;
                await ShowDialog($"Image selected: {imagePath}");
            }
            else
            {
                await ShowDialog("No image was selected.");
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the folder picker
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Add a file type filter for compatibility (required in WinUI 3)
            folderPicker.FileTypeFilter.Add("*");

            // Get the window handle and initialize the picker for desktop apps
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            // Show the picker and get the selected folder
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Process the selected folder path
                string folderPath = folder.Path;
                await ShowDialog($"Folder selected: {folderPath}");
            }
            else
            {
                await ShowDialog("No folder was selected.");
            }
        }

        private void AddNewHueFactor_Click(object sender, RoutedEventArgs e)
        {
            HueViewModel.AddFactor();
        }

        private void AddNewBrightnessFactor_Click(object sender, RoutedEventArgs e)
        {
            BrightnessViewModel.AddFactor();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is int factor)
            {
                BrightnessViewModel.BrightnessFactors.Add(factor + 1); // Add 1 to the selected factor
            }
        }

        private void RemoveBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem)?.DataContext is int factor)
            {
                BrightnessViewModel.RemoveFactor(factor);
            }
        }

        private void RemoveHueButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem)?.DataContext is int factor)
            {
                HueViewModel.RemoveFactor(factor);
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            appSettings.resetSettings();
        }


    }
}
