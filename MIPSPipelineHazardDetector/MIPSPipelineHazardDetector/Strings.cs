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
        public static readonly string WelcomeMemoText = "Welcome! This application simulates a pipeline for MIPS instructions.\n" +
            "There are 5 sections outputted to the program once an instruction has been read from a text file.\n" +
            "\tThe first section is the hazard detection section. This section is meant as a warning to warn for potential hazards.\n" +
            "\t\tThe section mimicks a person identifying hazards by looking at a list of instructions,\n" +
            "\tThe second section is a timing diagram of a pipeline without forwarding hardware.\n" +
            "\t\tNext to the pipeline, you will see an output showing the state of the pipeline at each cycle in time.\n" +
            "\tRespectively, the final section shows mirrors the second section but with forwarding enabled.\n" +
            "\n\n" +
            "Known Bugs: \n" +
            "-Export currently only exports the pipeline-cycle texts instead of the diagram.\n" +
            "-\"New\" only works to add a text file when the program first runs. To run another text file through the pipeline\n" +
            "\tthe application has to be exited and re-entered.\n" +
            "-There is also a UI bug when adding more than 4 instructions.";
    }
}
