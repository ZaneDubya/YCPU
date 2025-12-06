using System;

namespace ResourceBuilder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            string path_in_base = @"..\..\..\..\..\Import\";
            string path_out_base = @"..\..\..\..\..\Export\";

            Images.PNGtoBIN(
                path_in_base + "lem1802_charset.png",
                path_out_base + "lem1802_charset.bin");
            Palettes.Palette_ToRGB444(
                path_in_base + "16bitpal.txt",
                path_out_base + "lem1802_16bitpal.bin");
        }
    }
}

