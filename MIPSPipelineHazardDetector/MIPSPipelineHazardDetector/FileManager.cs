using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{

    public static class FileManager
    {
        public static string ReadTextFile(string FilePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(FilePath))
                {
                    string contents = sr.ReadToEnd();
                    return contents;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }


        public static void WriteOutputToTextFile(dynamic JSONString, string fileName)
        {
            //ToDo
        }

        public static void WriteJSONStringToText(string JSONString, string fileName)
        { /*
         * print json object string to file */
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName)))
                outputFile.WriteLine(JSONString);
        }
    }
}
