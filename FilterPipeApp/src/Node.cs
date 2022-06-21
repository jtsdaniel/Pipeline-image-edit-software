using System;

// Needed for Rgba32
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project
{
   
    
    /// <summary>
    /// Modifier in a class declaration to indicate that a class is intended only to be a base class of other classes
    /// </summary>
    abstract class Node    
    {
        public const int processor_Num = 8;
        //method to get node's name
        abstract public string Name{get;}
        //method to process image
        abstract public Image Process(Image input);
    }

    /// <summary>
    /// Node that converts the given image to grayscale.
    /// </summary>
    class N_Grayscale: Node
    {
        // Overide current node to new string name as "GrayScale"
        public override string Name 
        {
            get 
            {
                return "GrayScale";
            }
        }
        /// <summary>
        /// Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input"> image wanted to edit</param>
        /// <returns>grayscale image</returns>
        public override Image Process(Image input)
        {
            // A method to receive the image input and give out the output
            Image output = Image.ToGrayscale(input);
            return output;
        }
    }

    /// <summary>
    /// Node that modify the horizontally or vertically flip the picture
    /// </summary>
    class N_Flip: Node
    {
        // A private data members indicated optional direction
        private bool _Horizontal;
        private bool _Vertical;
        // A constructors to initialises data members
        public N_Flip(bool Horizontal, bool Vertical)
        {
            _Horizontal = Horizontal;
            _Vertical = Vertical;
        }
        // Overide current node to new string name as "Flip" with 2 elements
        public override string Name 
        {
            get {return $"Flip (Horizontal:{_Horizontal}, Vertical:{_Vertical})";}
        }
        /// <summary>
        /// receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input">image wanted to edit</param>
        /// <returns>flip image</returns>
        public override Image Process(Image input)
        {
            // Take the width and height of input picture
            Image output = new Image(input.Width, input.Height);
            // Create conditions to flip the image vertically or horizontally
            // Vertical flip
            if (_Vertical == true)
            {
                // Go through each image's pixels
                Parallel.For(0, input.Width - 1, new ParallelOptions { MaxDegreeOfParallelism = Node.processor_Num },
                    x => {
                        for (int y = 0; y < input.Height; y++)
                        {
                            // Exchange each pixels at the closer x-position with the respectively pixels at further x-position 
                            Rgba32 oldPixel = input[input.Width - 1 - x, y];
                            Rgba32 newPixel = new Rgba32(
                                r: oldPixel.R,
                                g: oldPixel.G,
                                b: oldPixel.B,
                                a: oldPixel.A
                            );
                            output[x, y] = newPixel;
                        }
                    });
            }
            // Horizontal flip
            if (_Horizontal == true)
            {
                // Go through each image's pixels
                Parallel.For(0, input.Width - 1, new ParallelOptions { MaxDegreeOfParallelism = Node.processor_Num },
                    x => {
                        for (int y = 0; y <= input.Height - 1; y++)
                        {
                            // Exchange each pixels at the closer y-position with the respectively pixels at further y-position 
                            Rgba32 oldPixel = input[x, input.Height - 1 - y];
                            Rgba32 newPixel = new Rgba32(
                                r: oldPixel.R,
                                g: oldPixel.G,
                                b: oldPixel.B,
                                a: oldPixel.A
                            );
                            output[x, y] = newPixel;
                        }
                    });
            }
            return output;
        }
    }

    /// <summary>
    /// Node that applies a sepia tone filter to the image by adjusting the colour of pixel values.
    /// </summary>
    class N_Sepia: Node
    {
        // Overide current node to new string name as "Sepia"
        public override string Name 
        {
            get {return "Sepia";}
        }
        /// <summary>
        ///  Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input">image wanted to edit</param>
        /// <returns>sepia image</returns>
        public override Image Process(Image input)
        {
            // Take the width and height of input picture
            Image output = new Image(input.Width, input.Height);
            // Go through each image's pixels
            Parallel.For(0, input.Width - 1, new ParallelOptions { MaxDegreeOfParallelism = Node.processor_Num },
                    x =>
                    {
                        for (int y = 0; y < input.Height; y++)
                        {
                            // Apply new r,g,b values to each pixels to make sepia effect
                            Rgba32 oldPixel = input[x, y];
                            Rgba32 newPixel = new Rgba32(
                                r: (byte)Math.Min(255, Math.Max(0, 0.393 * oldPixel.R + 0.769 * oldPixel.G + 0.189 * oldPixel.B)),
                                g: (byte)Math.Min(255, Math.Max(0, 0.349 * oldPixel.R + 0.686 * oldPixel.G + 0.168 * oldPixel.B)),
                                b: (byte)Math.Min(255, Math.Max(0, 0.272 * oldPixel.R + 0.534 * oldPixel.G + 0.131 * oldPixel.B)),
                                a: oldPixel.A
                            );
                            output[x, y] = newPixel;
                        }
                    });
            return output;
        }
    }

    /// <summary>
    /// Node that applies a vignette effect on an image.
    /// </summary>
    class N_Vignette: Node
    {
        // Overide current node to new string name as "Vignette"
        public override string Name 
        {
            get {return "Vignette";}
        }
        // A data members to make variables for x,y center coordinates 
        private static double _xCorofcenter;
        private static double _yCorofcenter;
        /// <summary>
        /// Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input">image wanted to edit</param>
        /// <returns>vignette image</returns>
        public override Image Process(Image input)
        {
            // Get the image width and height
            Image output = new Image(input.Width, input.Height);

            // Prepare necessary elements to caluclate the brightness 
            _xCorofcenter = input.Width / 2;
            _yCorofcenter = input.Height / 2;

            double maxDist = Math.Sqrt(Math.Pow(_yCorofcenter, 2) + Math.Pow(_xCorofcenter, 2));
            // Go through each pixel of image
            Parallel.For(0, input.Width - 1, new ParallelOptions { MaxDegreeOfParallelism = Node.processor_Num },
                    x =>
                    {
                        for (int y = 0; y < input.Height; y++)
                        {
                            Rgba32 oldPixel = input[x, y];
                            double xDistance = Math.Abs(_xCorofcenter - x);
                            double yDistance = Math.Abs(_yCorofcenter - y);
                            double distance = Math.Round(Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2)));
                            // Follow the formula to calculate the brightness of each pixel
                            double brightness = Math.Pow((maxDist - distance) / maxDist, 2);
                            // Apply brightness intensity to each pixel r,g,b colors
                            Rgba32 newPixel = new Rgba32(
                                r: (byte)Math.Min(255, Math.Max(0, oldPixel.R * brightness)),
                                g: (byte)Math.Min(255, Math.Max(0, oldPixel.G * brightness)),
                                b: (byte)Math.Min(255, Math.Max(0, oldPixel.B * brightness)),
                                a: oldPixel.A
                            );
                            output[x, y] = newPixel;
                        }
                    });
            return output;
        }
    }

    /// <summary>
    /// Node that Normalises an image by scaling the range of intensities to fall in the range (0, 255)
    /// </summary>
    class N_Normalise: Node
    {
        // Overide current node to new string name as "Normalise"
        public override string Name 
        {
            get {return "Normalise";}
        }
        // The private data members to make variable use of the old minimum and maximum about the image
        private static double _oldMin;
        private static double _oldMax;
        /// <summary>
        /// Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Image Process(Image input)
        {
            // Turn the image to gray scale
            Image output = Image.ToGrayscale(input);
            // Necessary elements to normalises an image
            const double _newMax = 255;
            const double _newMin = 0;
            _oldMin = 255;
            _oldMax = 0;
            // Go through each pixel of image
            for (int x = 0; x < input.Width - 1; x++)
            {
                for (int y = 0; y < input.Height - 1; y++)
                {
                    // Check the old minimum value of pixel image
                    if ((int)(output[x, y].R) < _oldMin)
                    {
                        _oldMin = (int)(output[x, y].R);
                    }
                    // Check the old maximum value of pixel image
                    if ((int)(output[x, y].R) > _oldMax)
                    {
                        _oldMax = (int)(output[x, y].R);
                    }

                }
            }
            // Go through each pixel of image
            Parallel.For(0, input.Width - 1, new ParallelOptions { MaxDegreeOfParallelism = Node.processor_Num },
                    x =>
                    {
                        for (int y = 0; y < input.Height - 1; y++)
                        {
                            // Normalise number of the old image based on the intensity of image
                            double _numOld = output.GetIntensity(x, y);
                            Rgba32 oldPixel = input[x, y];
                            // Formula to find a new normalise number for the pixel image 
                            double relavtiveBrightness = Math.Abs((_numOld - _oldMin) / (_oldMax - _oldMin));
                            double numNew = Math.Abs(_newMin + relavtiveBrightness * (_newMax - _newMin));
                            // Apply new normalise number to each pixel red, green and blue colors
                            Rgba32 newPixel = new Rgba32(
                                r: (byte)Math.Min(255, Math.Max(0, numNew)),
                                g: (byte)Math.Min(255, Math.Max(0, numNew)),
                                b: (byte)Math.Min(255, Math.Max(0, numNew)),
                                a: oldPixel.A
                            );
                            output[x, y] = newPixel;
                        }
                    });
            return output;
        }
    }

    /// <summary>
    /// Node to convert the image to a new size
    /// </summary>
    class N_Resize: Node
    {
        // The private data members to make variable of use to create new width and new height for the new image
        private static int _newWidth;
        private static int _newHeight;
        // The constructor to initialises the data members
        public N_Resize(int newWidth, int newHeight)
        {
            _newHeight = newHeight;
            _newWidth = newWidth;
        }
        // Overide current node to new string name as "Resize" with 2 elements
        public override string Name 
        {
            get {return $"Resize(New Width:{_newWidth}, New Height:{_newHeight})";}
        }
        /// <summary>
        /// Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Image Process(Image input)
        {
            // Input the resized image by inserting the new value of width and height
            Image output = Image.Resize(input, _newWidth, _newHeight);
            return output;
        }
    }

    /// <summary>
    /// Add a border of a certain size to the input image
    /// </summary>
    class N_AddBorder: Node
    {
        // A private data member of the color of border 
        private static Rgba32 _colourOfborder;
        // A private data member of border size
        private static int _borderSize;
        // The constructor to initialises the data members 
        public N_AddBorder(Rgba32 colourOfborder, int borderSize) 
        {
            _colourOfborder = colourOfborder;
            _borderSize = borderSize;
        }
        // Overide current node to new string name as "AddBorder" with 2 elements
        public override string Name 
        {
            get {return $"AddBorder(Rgba values:{_colourOfborder}, Size:{_borderSize})";}
        }
        /// <summary>
        /// Receive the input image and give out the processed output image
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Image Process(Image input)
        {
            // The width and height of the image
            Image output = new Image(input.Width, input.Height);
            // Modify the image's width and height according to border size
            Image resizedImage = Image.Resize(input, input.Width - _borderSize, input.Height - _borderSize);
            // Check and draw the right height of border
            Parallel.For(0, _borderSize, x => {
                for (int y = 0; y < input.Height; y++)
                {
                    // Apply color to the right height of border
                    Rgba32 newPixel = _colourOfborder;
                    output[x, y] = newPixel;
                }
            });

            // Check and draw the top width of border
            Parallel.For(0, resizedImage.Width, x => {
                for (int y = 0; y < _borderSize; y++)
                {
                    // Apply color to the top width of border
                    Rgba32 newPixel = _colourOfborder;
                    output[x, y] = newPixel;
                }
            });

            // Check and draw the left height of border
            Parallel.For(input.Width - _borderSize, input.Width, x => {
                for (int y = 0; y < input.Height; y++)
                {
                    // Apply color to the left height of border
                    Rgba32 newPixel = _colourOfborder;
                    output[x, y] = newPixel;
                }
            });

            // Check and draw the bottom width of border
            Parallel.For(0, resizedImage.Width, x =>
            {
                for (int y = input.Height - _borderSize; y < input.Height; y++)
                {
                    // Apply color to the bottom width of border
                    Rgba32 newPixel = _colourOfborder;
                    output[x, y] = newPixel;
                }
            });
            // Check the size of border to insert the image 
            Parallel.For(_borderSize, input.Width - _borderSize, x => {
                for (int y = _borderSize; y < input.Height - _borderSize; y++)
                {
                    // Insert the image based on the size of border
                    output[x, y] = resizedImage[x, y];
                }
            });

            return output;
        }
    }

}



