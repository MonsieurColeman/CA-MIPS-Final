using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public class PipelineDependencyChecker
    {
        /*
         * f = 0 bc 1 section to fetch
         * d = 1,2,3 for beg, middle, end
         * x = 4,5 for beg, end
         * m = 6,7 for beg, end
         * w = 8,9,10 for beg, middle, end
         */

        public static bool HazardChecker(InstructionCommand newCommand, InstructionCommand command)
        {
            //if either of the commands is a stall or empty, then no dependencies
            if (newCommand.inst_.GetInstructionType() == InstructionType.stall ||
                newCommand.inst_.GetInstructionType() == InstructionType.empty ||
                command.inst_.GetInstructionType() == InstructionType.stall ||
                command.inst_.GetInstructionType() == InstructionType.empty)
                return false;

            bool hazardExists = false;

            //if new command tries to read from old command, it is a potential data hazard
            if (command.rs_ == newCommand.rt_ || command.rs_ == newCommand.rd_ && newCommand.inst_.GetKey() == "sw")
                return true;

            //if store command, the rs_ section is a read, not a write
            if(newCommand.inst_.GetKey() == "sw" && command.rs_ == newCommand.rs_)
                return true;

            return hazardExists;
        }



        public static int StallDeterminer(bool forwarding, Instruction newCommand, Instruction command, int offset = 1)
        {
            int stalls = 0;

            //item 1 = tuple timing for when data is needed, item 2 = tuple timing for when data is available
            ((int, int), (int, int)) needyCommandTuple_temp = InstructionComparer(forwarding, newCommand);
            ((int, int), (int, int)) usingCommandTuple = InstructionComparer(forwarding, command);

            //Add the offset to the timing, because two instructions cannot start at the same time within a pipeline
            ((int, int), (int, int)) needyCommandTuple = 
                ((needyCommandTuple_temp.Item1.Item1+1, needyCommandTuple_temp.Item1.Item2),
                (needyCommandTuple_temp.Item2.Item1, needyCommandTuple_temp.Item2.Item2));

            //if the data will be available before, or when, the next command needs it, we do not need to stall
            if ((needyCommandTuple.Item1.Item1 > usingCommandTuple.Item2.Item1) || (needyCommandTuple.Item1 == needyCommandTuple.Item2))
                return 0;
            
            //stalls = when data is avilable - when data is needed
            stalls = usingCommandTuple.Item2.Item1 - needyCommandTuple.Item1.Item1;

            //if the first command needs data at the beginning of the stage
            //but the data cant become available until the end, add 1 stall
            if (needyCommandTuple.Item1.Item2 < usingCommandTuple.Item2.Item2)
                stalls++;

            return stalls;
        }

        public static ((int,int),(int,int)) InstructionComparer(bool forwarding, Instruction i)
        {
            (int, int) needed = (0,0);
            (int, int) available = (0,0);
            if (forwarding && i.GetInstructionType() == InstructionType.rType)
            {
                needed = (3,1); //beginng of execution phase
                available = (3,3); //end of execution phase
            }
            else if (forwarding && i.GetInstructionType() == InstructionType.iType)
            {
                needed = (4,1); //beginning of memory
                available = (4,3); //end of memory
            }
            else if (!forwarding && i.GetInstructionType() == InstructionType.rType)
            {
                needed = (2,2); //middle of decode
                available = (5,2); //middle of writeback
            }
            else if (!forwarding && i.GetInstructionType() == InstructionType.iType)
            {
                needed = (2, 2); //middle of decode
                available = (5, 2); //middle of writeback
            }
            else
            {
                throw new NotImplementedException();
            }

            /*         
             * item 1: 1,2,3,4,5 = IF, ID, EX, M, W
             * item 2: 1,2,3 = begging, middle, end
             */


            return (needed, available);
        }
    }
}

/*
 Notes:
        
(item 1, item 2) where                  
* item 1: 1,2,3,4,5 = IF, ID, EX, M, W
* item 2: 1,2,3 = begging, middle, end
             



for rtype instructions:
                //needed middle of decode without forwarding
                //needed beginning of X with forwarding
                //available after write back without forwarding
                //available after execution with forwarding

 for itype instructions:
                //needed middle of decode without forwarding
                //needed beginning of memory with forwarding
                //available after write back without forwarding
                //available after memory with forwarding
 */
