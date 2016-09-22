using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFACETools;
using Flipper.Classes;

namespace Flipper
{
    public static class Utilities
    {

        /// <summary>
        /// Determines whether or not the file is exists and is a valid JSON file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True or false depending on whether or not the file is valid.</returns>
        public static bool IsFileValid(string filePath)
        {
            // Set a boolean to hold the value.
            bool isValid = true;

            // Check if the file exists, if not, set to false.
            if (!File.Exists(filePath))
                isValid = false;
            else
            {
                // Get the extension of the file.
                string extension = Path.GetExtension(filePath);
                // If the extension is set and the extension is not json, set to false.
                if (extension != null && extension.ToLower() != ".json")
                    isValid = false;
            }

            // Return the boolean value.
            return isValid;
        }

        /// <summary>
        /// Reads the contents of a specified file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>The contents of the file in string format.</returns>
        public static string GetFileContents(string filePath)
        {
            // Set a string variable to hold the contents.
            string fileContent;
            // Use a StreamReader to read the contents and set it to the variable.
            using (StreamReader reader = new StreamReader(filePath))
            {
                fileContent = reader.ReadToEnd();
            }
            // Return the variable.
            return fileContent;
        }

        /// <summary>
        /// Saves a file to a local folder.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="jsonData"></param>
        public static void SaveToFile(string filePath, string fileName, string jsonData)
        {
            // Create the directory if it doesn't exist.
            Directory.CreateDirectory(filePath);
            // Save the data to the designated filepath with the designated filename.
            File.WriteAllText(filePath + fileName, jsonData);
        }
    }
}
