using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public static class Coverter
    {
        public static (bool, List<InstructionCommand>, bool) StringInstructionConverter(string stringInput)
        {
            List<InstructionCommand> commands = new List<InstructionCommand>();
            InstructionCommand command;
            bool stringWasParsedCorrectly = true;
            bool unrecognizedCommandsEntered = false;

            //get list of text
            string[] instructions = StringToListCovnerter(stringInput);

            //Iterate Over intructions and generate a list of valid commands
            foreach (string instructionLiteral in instructions)
            {
                //Get the instruction chars by section
                string inst = instructionLiteral.Split(' ')[0];

                //If the instruction is supported, turn it into a command and add to list
                if (Globals.instructionDictionary.ContainsKey(inst))
                {
                    try
                    {
                        command = ProcessInstructionToInstructionCommand(Globals.instructionDictionary[inst], instructionLiteral);
                        commands.Add(command);
                    }
                    catch (Exception ex)
                    {
                        stringWasParsedCorrectly = false;
                        break;
                    }
                }
                else
                    unrecognizedCommandsEntered = true;

            }
            return (stringWasParsedCorrectly, commands, unrecognizedCommandsEntered);
        }

        public static string[] StringToListCovnerter(string s)
        //Takes a parces a string into a string array by delimiters
        {
            char[] delims = new[] { '\r', '\n' };
            string[] lines = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }

        public static InstructionCommand ProcessInstructionToInstructionCommand(Instruction instruction, string instructionLiteral)
        {
            /*
             This turns an instruction string into an intructioncommand type for the purpose of reading
             */

            InstructionCommand command = new InstructionCommand();
            string output = "";
            string[] inst = StringCleaner(instructionLiteral).Split(' ');
            Register rs;
            Register rt;
            Register rd;
            int shamt; //unused
            int immediate;
            switch (instruction.GetInstructionType())
            {
                case InstructionType.rType:
                    rs = Globals.RegisterDictionary[inst[1]];
                    rt = Globals.RegisterDictionary[inst[2]];
                    try
                    {
                        rd = Globals.RegisterDictionary[inst[3]];
                        command = new InstructionCommand(instruction,
                            rs, rt, rd);
                    }
                    catch (Exception ex)
                    {
                        immediate = Convert.ToInt32(inst[3]);
                        command = new InstructionCommand(instruction,
                            rs, rt, null, immediate, true);
                    }
                    break;
                case InstructionType.iType:
                    rs = Globals.RegisterDictionary[inst[1]];
                    immediate = Int32.Parse(inst[2]);
                    rt = Globals.RegisterDictionary[inst[3]];
                    command = new InstructionCommand(instruction,
                            rs, rt, null, immediate, true);
                    break;
                default:
                    break;
            }
            return command;
        }

        private static string StringCleaner(string s)
        {
            s = s.Replace(",", "");
            s = s.Replace("(", " ");
            s = s.Replace(")", "");
            return s;
        }

       
    }

}
