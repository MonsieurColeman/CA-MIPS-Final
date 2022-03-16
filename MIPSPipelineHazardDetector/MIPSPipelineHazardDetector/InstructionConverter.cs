using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public static class InstructionConverter
    {
        public static (bool, dynamic) StringInstructionConverter(string stringInput)
        {
            string[] instruction = StringToListCovnerter(stringInput);
            return(false,null);
        }

        static string[] StringToListCovnerter(string s)
        //Takes a parces a string into a string array by delimiters
        {
            char[] delims = new[] { '\r', '\n' };
            string[] lines = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }
    }
}
