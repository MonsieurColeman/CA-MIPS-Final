using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public static class Strings
    {
        public static readonly string textFileExtensionFilter = "Text file (*.txt)|*.txt";
        public static readonly string error_InvalidFile = "Invalid File Input";
        public static readonly string error_NullExport = "There has to be something for you to export!";
        public static readonly string error_UnexpectedError = "An unexpected error has occured!";
        public static readonly string error_UnrecognizedArguments = "Unrecognized commands have been entered and ignored!";
        public static readonly string outputText_stall = "stall";
        public static readonly string outputText_empty = "empty";
        public static readonly string WelcomeMemoText = "Welcome! This application, simulates a pipeline for MIPS instructions.\n" +
            "There are 5 sections outputted to the program once an instruction has been read from a text file.\n" +
            "\tThe first section is hazard detection section. This section is meant as a warning to warning for potential hazards.\n" +
            "";
    }
}
