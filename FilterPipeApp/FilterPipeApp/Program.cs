using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
namespace Project
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            // Create the pipeline
            string[] lines = File.ReadAllLines("pipel.txt");
            List<Node> pipeline = Pipeline.Load("pipel.txt");
            // Load the input image
            Image input = new Image("example image/grow.png");
            // Run the full pipeline on the input image
            Image output = Pipeline.Run(input, pipeline, logging: true, saveDir: "out_folder");
            // Save the result
            input.Write("input");
            output.Write("output");
            Console.ReadLine();

        }
    }
}