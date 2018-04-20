using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeEncoding
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileType = "*.txt";
            var folder = @"D:\YourFolder";

            ChangeEncoder(fileType, folder, Encoding.Unicode);

        }

        private static void ChangeEncoder(string fileType, string folder, Encoding sourceEncoding)
        {
            var files = Directory.GetFiles(folder, fileType);
            Console.WriteLine($"Found {files.Length} files in {folder}");
            foreach (var file in files)
            {
                var atualEncode = GetEncoding(file);
                Console.WriteLine($"Encode atual: {atualEncode.EncodingName} - trocando para: {sourceEncoding.EncodingName}");
                try
                {
                    var content = File.ReadAllText(file);
                    File.WriteAllText(file, content, sourceEncoding);
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Trouble with {0}, in {1}, issue:{2}", file, folder, e.Message));
                    Environment.Exit(1);

                }
            }

            var dirs = Directory.GetDirectories(folder);
            foreach (var dir in dirs)
            {
                ChangeEncoder(fileType, dir, sourceEncoding);
            }
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}
