using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Tools
{
    internal static class ImageToColorArray
    {
        public static void ImportStandard()
            => GenerateColorPaletteArray("tw-color-palette.png", 5, 8, "FbBaseColors", "FbBaseColors.cs");


        /// <summary>
        /// Open the png file and generate a color palette cs file as a two dimenssion C# array of web color strings (like "#458735").
        /// It takes the center pixel of each square. Array size is the result of the image size.
        /// </summary>
        /// <param name="filePath">A png file path.</param>
        /// <param name="outputFilePath">If the file is not found, it search for the file in the upper directories incrementally up to the searchLimit parameters parents.</param>
        /// <param name="squareSize">The size of each color square in the image.</param>
        /// <param name="outputFilePath">The cs result</param>
        public static void GenerateColorPaletteArray(string filePath, int searchLimit, int squareSize, string arrayName, string outputFilePath)
        {

        }
    }
}
