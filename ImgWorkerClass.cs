using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Text.Json;
using ColorMine.ColorSpaces;
using System.Collections;

namespace color_palette_creator_v2
{
    public class ImgWorkerClass
    {
        public List<Rgba32> GetRefSheetColors(string imgPath)
        {
            List<Rgba32> colors = new List<Rgba32>();

            // Load the image
            using (var image = Image.Load<Rgba32>(imgPath))
            {
                int middleX = image.Width / 2;
                int middleY = image.Height / 2;

                // Get colors along the vertical middle line
                for (int y = 0; y < image.Height; y++)
                {
                    Rgba32 color = image[middleX, y];
                    color.A = 255; // Set alpha to maximum
                    colors.Add(color);
                }

                // Get colors along the horizontal middle line
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 color = image[x, middleY];
                    color.A = 255; // Set alpha to maximum
                    colors.Add(color);
                }
            }

            // Remove duplicate colors
            return new HashSet<Rgba32>(colors).ToList();
        }


        // Method to read an image, store it in local storage, and return it as RGBA data in RefImageData format
        public static RefImageData PrepareAndStoreImageAsRGBA(string imagePath, string fileName = "RefImage.png")
        {
                // Create and return RefImageData with forhight, forwidth, matrix, and the file path
                var image = Image.Load<Rgba32>(imagePath);
                // Get the forhight and forwidth of the image
                int width = image.Width;
                int height = image.Height;

                // Create and return RefImageData with forhight, forwidth, matrix, and the file path
                return new RefImageData
                {
                    Width = width,
                    Height = height,
                    Image = image,
                    FilePath = imagePath // Set the path to the locally saved image
                };
        }

        public static (bool isMatch, List<string> missingColors) checkRefImage(RefImageData refImageData)
        {
            // Extract hex values from the reference image
            List<string> refImageHexValues = RefImageProcessor.ExtractRefImageHexValues(refImageData);

            //
            int colorCount = refImageHexValues.Count;

            // Create a unique hue gradient based on the image dimensions
            List<string> uniqueHues = RefImageProcessor.CreateUniqueHueGradient(colorCount);


            // Compare the unique hues with the reference image colors
            (bool isMatch, List<string> missingColors) = RefImageProcessor.FindBestColorMatch(refImageHexValues, uniqueHues);

            return (isMatch, missingColors);
        }


    }


    internal class RefImageProcessor
    {

        public static List<string> CreateUniqueHueGradient(int colorCount)
        {
            // 2D list to store colors row-wise
            var colors = new List<string>();

            // Set saturation and brightness for vibrant colors
            double saturation = 1.0;
            double brightness = 1.0;

            // Calculate hue step to distribute hues across the grid
            double hueStep = Math.Round(1.0 / colorCount, 4);
            var hightIterator = 1;
            var widthIterator = colorCount;

            for (int forhight = 0; forhight < hightIterator; forhight++) // Adjust inner loop to use hightIterator as the iteration count
                {
                    for (int forwidth = 0; forwidth < widthIterator; forwidth++) // Adjust inner loop to use widthIterator as the iteration count
                    {
                    int cellIndex = forhight * forwidth; // Calculate cell index based on widthIterator and forhight
                    if (cellIndex > colorCount) continue; // Stop if we reach the color count

                        // Calculate hue for the current cell
                        double hue = (forhight * widthIterator + forwidth) * hueStep;
                        double huescaled = hue * 360;
                        // Create HSV color and convert to RGB
                        var hsv = new Hsv { H = huescaled, S = saturation, V = brightness };
                    var rgb = hsv.To<Rgb>();

                        // Format as hex string and add to row
                    var hexvalue = $"#{((int)rgb.R):X2}{((int)rgb.G):X2}{((int)rgb.B):X2}{(255):X2}";
                        colors.Add(hexvalue);
                    }
                }
            return colors;
        }


        public static List<string> ExtractRefImageHexValues(RefImageData imageData)
        {
            List<string> hexColors = new List<string>();

            // Extract colors from the image Imagesharp object

            for (int y = 0; y < imageData.Image.Height; y++)
            {
                for (int x = 0; x < imageData.Image.Width; x++)
                {
                    var pixel = imageData.Image[x, y];
                    hexColors.Add('#'+pixel.ToHex());
                }
            }
            hexColors = hexColors.Where(c => c != "#00000000" && c != "#FFFFFFFF").ToList();
            // remove duplicates
            hexColors = hexColors.Distinct().ToList();
            return hexColors;
        }

        public static (bool isMatch, List<string> bestMissingColors) FindBestColorMatch(List<string> fromRefImage, List<string> fromGenMatrix)
        {
            List<string> bestMissingColors = null;
            bool isExactMatch = false;

            // Filter out black and white from the flat list
            var filteredRefImage = fromRefImage.Where(c => c != "#00000000" && c != "#FFFFFFFF").ToList();
                // Flatten the current 2D color grid to a single list
            var flattenedGenMatrix = fromGenMatrix.Where(c => c != "#00000000" && c != "#FFFFFFFF").ToList();

                // Find colors in the flat list that are missing in the current flattened grid
                var missingColors = filteredRefImage.Except(flattenedGenMatrix).ToList();

                // Check if this is an exact match (no missing colors)
                if (missingColors.Count == 0)
                {
                    isExactMatch = true;
                    bestMissingColors = flattenedGenMatrix;
                }
                else {
                    bestMissingColors = missingColors;
                }
                return (isExactMatch, bestMissingColors);
        }
    }
}
