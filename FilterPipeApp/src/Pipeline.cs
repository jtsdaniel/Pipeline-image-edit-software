using System;

// needed for Rgba32
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Project
{
    /// <summary>
    /// 
    /// </summary>
    class Pipeline
    {
        /// <summary>
        /// Print out the name of each node with each unique elements and saving of intermediate results
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pipeline"></param>
        /// <param name="logging"></param>
        /// <param name="saveDir"></param>
        /// <returns></returns>
        public static Image Run(Image input, List<Node> pipeline, bool logging, string saveDir)
        {
            List<Image> stepList = new List<Image>();
            Image[] stepArray = new Image[pipeline.Count];
            double total_processing_time = 0;
            double total_nodes_time = 0;
            // Print out running pipeline on image with width and height of the image
            Console.WriteLine($"Running pipeline on Image ( {input.Width} X {input.Height} ) \n" );
            // A build in function to record the times taken for processing each pipeline nodes
            Stopwatch stopwatch = new Stopwatch();
            // Read through all of the pipeline nodes
           for(int x = 0; x < pipeline.Count; x++)
           {
            // The stopwatch should start recording once the program run
            stopwatch.Start();
               Image output = pipeline[x].Process(input);
               input = output;
           
               if(logging == true)
               {
                Console.WriteLine($"Node:{pipeline[x].Name}");
                Console.WriteLine($"Input: Image ({input.Width.ToString()} X {input.Height.ToString()})");
                Console.WriteLine("     Processing...");
                    //save the processed step
                    stepList.Add(output);
                    // The stopwatch should stop recording once the program successfully complete running and print out required stats
                    stopwatch.Stop();
                    Console.WriteLine("Took: {0}", stopwatch.Elapsed);
                    total_nodes_time += (double)stopwatch.Elapsed.TotalSeconds;
                    Console.WriteLine($"Output: Image ({output.Width.ToString()} X {output.Height.ToString()})\n");
                    stopwatch.Reset();
                }
           }


            Stopwatch stopwatch1 = new Stopwatch();
            // Create a folder if the folder doesn't exists
            if (saveDir != "")
            {
                stepArray = stepList.ToArray();
                stopwatch1.Start();
                Parallel.For(0, stepArray.Length, new ParallelOptions {MaxDegreeOfParallelism = Node.processor_Num },
                    i => {
                    // Create a folder to store processed nodes
                    System.IO.Directory.CreateDirectory(saveDir);
                    // Create sub-file
                    stepArray[i].Write($"{saveDir}/image_{i}");
                });
                stopwatch1.Stop();
            }
            total_processing_time = (double) stopwatch1.Elapsed.TotalSeconds + total_nodes_time;
            //print out total processing time
            Console.WriteLine("All Node processing time: " + total_nodes_time + " Seconds" + " On " + Node.processor_Num + " Core(s)");
            Console.WriteLine("Save File processing time: " + (double)stopwatch1.Elapsed.TotalSeconds + " Seconds" + " On " + Node.processor_Num + " Core(s)");
            Console.WriteLine("Total processing time: " + total_processing_time + " Seconds" + " On " + Node.processor_Num+ " Core(s)");
           // Print out the current pipeline nodes that has been stored at a specific folder 
                
                Console.WriteLine($"Saved at: {Path.GetFullPath(saveDir)}");
           Image output2 = input;

           return output2;
        }

        /// <summary>
        /// The nodes that have specific requirement elements be able to running it
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public static List<Node> Load(string pathname)
        {
            string[] lines = File.ReadAllLines(pathname);
            List<Node> pipeline = new List<Node>(){};
            // Read through each piepeline for their required elements
            foreach( var line in lines)
            {   // Spacing out to readers readable readable 
                string[] characteristics = line.Split(" ");
                // Checking the nodes that only required 1 element 
                if(characteristics.Length == 1)
                {
                    if(characteristics[0] == "node=grayscale")
                    {
                        pipeline.Add(new N_Grayscale());   
                    }
                    if(characteristics[0] == "node=sepia")
                    {
                        pipeline.Add(new N_Sepia());   
                    }
                    if(characteristics[0] == "node=vignette")
                    {
                        pipeline.Add(new N_Vignette());   
                    }
                    if(characteristics[0] == "node=normalise")
                    {
                        pipeline.Add(new N_Normalise());   
                    }
                }

                // Checking the nodes that required 2 elements
                if(characteristics.Length == 2)
                {
                // Equal sign is added for readers to acknowledge the string of the required elements
                string[] data1 = characteristics[1].Split("=");
                
                    // For node N_Resize, it required 2 new elements which is new width and new height to output the new image
                    if(data1[0] == "newSize")
                    {
                        string[] parameters = data1[1].Split("x");
                        int Width = int.Parse(parameters[0]);
                        int Height = int.Parse(parameters[1]);
                        pipeline.Add(new N_Resize(newWidth: Width, newHeight: Height));  
                    }

                    // For node N_Flip, it requried 2 new elements which using boolean method to output the new horizontal or vertical image
                    if(data1[0] == "direction")
                    {
                        // Change the image to horizontal
                        if(data1[1] == "[horizontal]")
                        {
                            pipeline.Add(new N_Flip(Horizontal:true, Vertical:false));
                        }
                        // Change the image to vertical
                        if(data1[1] == "[vertical]")
                        {
                            pipeline.Add(new N_Flip(Horizontal:false, Vertical:true));
                        }
                    }
                }
                // Checking the nodes that required 3 elements
                if(characteristics.Length == 3)
                {
                    // Equal sign is added for readers to acknowledge the string of the required elements
                    string[] data1 = characteristics[1].Split("=");
                    string[] data2 = characteristics[2].Split("=");
                    // For node N_AddBorder, it required 3 new elements from RGBA which is red, green and blue
                    if(data2[0] == "borderColor")
                    {
                        // get values of 3 colours between the "," symbol
                        string[] parameters = data2[1].Split(",");
                        int color1 = int.Parse(parameters[0]);
                        int color2 = int.Parse(parameters[1]);
                        int color3 = int.Parse(parameters[2]);
                        int size = int.Parse(data1[1]);
                        pipeline.Add(new N_AddBorder(colourOfborder: new Rgba32((byte)color1,(byte)color2,(byte)color3), borderSize:size));
                    }
                }
            }
            return pipeline; 
        }
    }
}