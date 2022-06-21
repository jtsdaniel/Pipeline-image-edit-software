using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace Project
{
    /// <summary>
    /// An image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The image data
        /// </summary>
        private Image<Rgba32> _data;

        /// <summary>
        /// The image width
        /// </summary>
        public int Width => _data.Width;

        /// <summary>
        /// The image height
        /// </summary>
        public int Height => _data.Height;

        /// <summary>
        /// Gets the Rgba32 pixel value at the specified coordinates
        /// </summary>
        public Rgba32 this[int x, int y]
        {
            get => _data[x, y];
            set => _data[x, y] = value;
        }

        /// <summary>
        /// Gets the overall intensity of a pixel (average of its colors)
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>The overall intensity of the pixel</returns>
        public int GetIntensity(int x, int y) => (int)((
            _data[x, y].R +
            _data[x, y].G +
            _data[x, y].B
        ) / 3);

        /// <summary>
        /// Create a new image using the image at the given filepath
        /// </summary>
        /// <param name="filePath">The filepath of an image file</param>
        public Image(string filePath) => _data = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);

        /// <summary>
        /// Create a new image by copying an existing one
        /// </summary>
        /// <param name="image">An image object</param>
        public Image(Image image) => _data = image._data.Clone();

        /// <summary>
        /// Create a new image using the given ImageSharp image object
        /// </summary>
        /// <param name="data">An ImageSharp Image object</param>
        public Image(Image<Rgba32> data) => _data = data.Clone();

        /// <summary>
        /// Create a new empty (black) image from the given dimensions
        /// </summary>
        /// <param name="width">The desired width of the new image</param>
        /// <param name="height">The desired height of the new image</param>
        public Image(int width, int height) => _data = new Image<Rgba32>(
            configuration: Configuration.Default,
            width: width,
            height: height,
            backgroundColor: new Rgba32(0, 0, 0, 0)
        );

        /// <summary>
        /// Write the image to file with the PNG extension
        /// </summary>
        /// <param name="filePath">The desired filepath of the saved image excluding the extension</param>
        /// <returns>The full filepath written to, including the extension</returns>
        public string Write(string filePath)
        {
            _data.Save($"{filePath}.png");
            return $"{filePath}.png";
        }

        public override string ToString() => $"{Width}x{Height}";

        /// <summary>
        /// Create a new image by resizing this one using NearestNeighbor interpolation
        /// </summary>
        /// <param name="image">The original image</param>
        /// <param name="newWidth">The new width for the image</param>
        /// <param name="newHeight">The new height for the image</param>
        /// <returns>A new resized image</returns>
        public static Image Resize(Image image, int newWidth, int newHeight)
        {
            ResizeOptions options = new ResizeOptions();
            Image newImage = new Image(image);
            newImage._data.Mutate(x => x.Resize(new Size(newWidth, newHeight)));
            return newImage;
        }

        /// <summary>
        /// Create a new image by converting this one to grayscale
        /// </summary>
        /// <param name="image">The original image</param>
        /// <returns>A new grayscale image</returns>
        public static Image ToGrayscale(Image image)
        {
            var parallelOption = new ParallelOptions() {
                MaxDegreeOfParallelism = Node.processor_Num
            };
            Image result = new Image(image.Width, image.Height);
            Parallel.For(0, result.Width, parallelOption, x => {
                for (int y = 0; y < result.Height; y++)
                {
                    byte intensity = (byte)(image.GetIntensity(x, y));
                    result[x, y] = new Rgba32(intensity, intensity, intensity, image[x, y].A);
                }

            });

    
            return result;
        }
    }
}
