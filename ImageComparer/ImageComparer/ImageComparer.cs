using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ImageComparer
{
    public class ImageComparer
    {
        // basic console.writeline but just to make it more convenient I wrote it like this
        private static void WriteLine(string text = null, bool clearBefore = false)
        {
            if (clearBefore)
                Console.Clear();
            if (text != null)
                Console.WriteLine(text);
            else
                Console.WriteLine();
        }
        
        // console writeline only message when exception given
        private static void WriteLine(Exception e, bool clearBefore = false)
        {
            if (clearBefore)
                Console.Clear();
            Console.WriteLine(e.Message);
        }

        private static string ReadLine()
        {
            return Console.ReadLine();
        }

        private static void Clear()
        {
            WriteLine(clearBefore: true);
        }
        
        public static void Main(string[] args)
        {
            Console.Title = "Image Comparer";
            Clear();
            WriteLine("Deividas Vaiciunas IIF 16/1 (20162699)");
            WriteLine();
            var dir = "";
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLine("Type in 'STOP' to exit");
                Console.ForegroundColor = ConsoleColor.Gray;
                WriteLine();
                dir = SelectDirectory();
                if (dir.ToLower() == "stop")
                    return;
                try
                {
                    if (Directory.Exists(dir))
                        break;
                    Clear();
                    dir = dir == string.Empty ? "Empty value" : dir.Trim();
                    WriteLine("Deividas Vaiciunas IIF 16/1 (20162699)");
                    WriteLine();
                    WriteLine($"{dir} - is not a valid directory.");
                    WriteLine("You will be asked to put in a valid directory path.");
                }
                catch (Exception e)
                {
                    Clear();
                    WriteLine("Deividas Vaiciunas IIF 16/1 (20162699)");
                    WriteLine();
                    WriteLine(e);
                    WriteLine("You will be asked to put in a valid directory path.");
                }
            }
            DirectoryInformation(dir);
            WriteLine();
            WriteLine();
            var index = 1;
            WriteLine("Searching for images that are the same...");
            foreach (var result in Compare(dir).Where(x => x.IsEqual).ToList())
            {
                WriteLine($"{index}. {result.SourceFullName} is equal to {result.CompareFullName}");
                index++;
            }
           
            ReadLine();
        }

        private static string SelectDirectory()
        {
            WriteLine("Please paste in a directory you want to search for duplicates at." +
                              "\nExample: C:\\Users\\User1\\Documents\\Images");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            WriteLine("NOTE: Only .jpg will be compared");
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLine();
            
            return ReadLine();
        }

        private static void DirectoryInformation(string directory)
        {
            Clear();
            WriteLine("Directory information:");
            var di = new DirectoryInfo(directory);
            WriteLine($"{di.FullName}");
            foreach (var file in di.GetFiles().Where(file => file.Name.Contains(".jpg")))
            {
                WriteLine($"{file.Name}");
            }
            WriteLine();
            WriteLine($"Total count of files: {di.GetFiles().Count(file => file.Name.Contains(".jpg"))}");
        }

        
        private static IEnumerable<CompareResult> Compare(string directory)
        {
            var fileNames = new List<CompareResult>();
            
            var di = new DirectoryInfo(directory);
            var searchedAlready = new List<string>();
            
            foreach (var file in di.GetFiles().Where(fileInfo => fileInfo.FullName.Contains(".jpg")))
            {
                foreach (var cmpFile in di.GetFiles().Where(fileInfo => fileInfo.FullName.Contains(".jpg") && 
                                                                        file.Name != fileInfo.Name))
                {
                    if (searchedAlready.Any(x => x == cmpFile.Name))
                        continue;
                    
                    List<bool> iHash1 = GetHash(new Bitmap(file.FullName));
                    List<bool> iHash2 = GetHash(new Bitmap(cmpFile.FullName));

                    //determine the number of equal pixel (x of 256)
                    int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
                    
                    fileNames.Add(new CompareResult
                    {
                        SourceFullName = file.Name,
                        CompareFullName = cmpFile.Name,
                        EqualElements = equalElements
                    });
                }
                searchedAlready.Add(file.Name);
            }
            
            WriteLine("Within the directory:");
            if (fileNames.Count(x => x.IsEqual) == 0)
                WriteLine("There are no images that are the same");
            return fileNames;
        }
        
        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();         
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }             
            }
            return lResult;
        }
        
    }
}

