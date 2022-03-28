using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public class Pipeline
    {
        /*
         Honestly, i could implement this with just a queue instead of 5 stacks, but ,,, time 
         is of the essence
         */

        public struct PipelineObj
        {
            public InstructionCommand command;
            public List<string> str;
            public int _entryCycle;
            public int stallsNeed;

            public PipelineObj(InstructionCommand ic, int entryCycle)
            {
                command = ic;
                str = new List<string>() { "F"};
                _entryCycle = entryCycle;
                stallsNeed = 0;
            }
        }

        int __cycle = 0;
        Stack<PipelineObj> __fetchStack = new Stack<PipelineObj>();
        Stack<PipelineObj> __DecodeStack = new Stack<PipelineObj>();
        Stack<PipelineObj> __ExecuteStack = new Stack<PipelineObj>();
        Stack<PipelineObj> __MemoryStack = new Stack<PipelineObj>();
        Stack<PipelineObj> __WritebackStack = new Stack<PipelineObj>();
        //public static List<PipelineObj> __output = new List<PipelineObj>();
        public List<PipelineObj> __output = new List<PipelineObj>();
        public string __state = "";
        public bool __forwardingEnabled = false;
        private int stalls = 0;
        Queue<(int, int, int)> stallAcc = new Queue<(int, int, int)>();

        public Pipeline(bool forwarding)
        {
            __forwardingEnabled = forwarding;
            __fetchStack = new Stack<PipelineObj> ();
            __DecodeStack = new Stack<PipelineObj>();
            __ExecuteStack = new Stack<PipelineObj>();
            __MemoryStack = new Stack<PipelineObj>();
            __WritebackStack = new Stack<PipelineObj>();
            __output = new List<PipelineObj>();
            __state = "";
        }

        public void StartPipelineWithCommand(InstructionCommand command)
        {
            FillPipeline();
            StateOfPipelineStart();
            __cycle++; //first cycle
            __fetchStack.Push(new PipelineObj(command, __cycle));
            StateOfPipeline();
            //NextCycle();
        }

        private void StallerAccumulator()
        {
            //item 1 = the cycle at which to stall
            //item 2 = the stage of the instruction to stall at
            //item 3 = the number of stalls

            //return, if there are no stalls scheduled
            if (!(stallAcc.Count > 0))
                return;

            //return, if it is not the cycle to stall
            int a = __cycle + 1;
            int b = stallAcc.Peek().Item1;
            if (a != b)
                return;

            //setup
            (int, int, int) stallObj = stallAcc.Dequeue();
            int numStalls = stallObj.Item3;
            int stallStage = stallObj.Item2;

            //advance pipeline for x cycles
            for (int i = 0; i < numStalls; i++)
                NextCycleStallForward(stallStage);
        }

        public void AddCommandToPipline(InstructionCommand command)
        {
            InstructionCommand prevCommand = __fetchStack.Peek().command;
            InstructionCommand grandFatherCommand = __DecodeStack.Peek().command;
            InstructionCommand greatGrandFatherCommand = __DecodeStack.Peek().command;

            //perform stall if scheduled
            if(__forwardingEnabled)
                StallerAccumulator();

            //move everyone out of the way
            if (__forwardingEnabled)
                NextCycle();
            else
                NextCycle(false);


            //instruction always gets into fetch
            __fetchStack.Push(new PipelineObj(command, __cycle));

            if (!__forwardingEnabled)
                PerformNonForwardingExecution(command, prevCommand, grandFatherCommand, greatGrandFatherCommand);
            else
                PerformForwardingExecution(command, prevCommand, grandFatherCommand, greatGrandFatherCommand);
        }

        public void PerformNonForwardingExecution(InstructionCommand command, InstructionCommand prevCommand, InstructionCommand grandFatherCommand, InstructionCommand greatGrandFatherCommand)
        {
            //if there is a hazard,
            if (PipelineDependencyChecker.HazardChecker(command, prevCommand))
            {
                StateOfPipeline();
                int stall = 0;
                stall = PipelineDependencyChecker.StallDeterminer(__forwardingEnabled, command.inst_, prevCommand.inst_);
                AdvancePipelineByXCycles(stall);
            }
            else if (PipelineDependencyChecker.HazardChecker(command, grandFatherCommand))
            {
                StateOfPipeline();
                int stall = 0;
                stall = PipelineDependencyChecker.StallDeterminer(__forwardingEnabled, command.inst_, grandFatherCommand.inst_) - 1;
                stall = (stall >= 0) ? stall : 0;
                AdvancePipelineByXCycles(stall);
            }
            else if (PipelineDependencyChecker.HazardChecker(command, greatGrandFatherCommand))
            {
                StateOfPipeline();
                int stall = 0;
                stall = PipelineDependencyChecker.StallDeterminer(__forwardingEnabled, command.inst_, greatGrandFatherCommand.inst_) - 2;
                stall = (stall >= 0) ? stall : 0;
                AdvancePipelineByXCycles(stall);
            }
            else
            {
                StateOfPipeline();
            }
        }

        public void PerformForwardingExecution(InstructionCommand command, InstructionCommand prevCommand, InstructionCommand grandFatherCommand, InstructionCommand greatGrandFatherCommand)
        {
            StateOfPipeline();

            //if there is a hazard,
            if (PipelineDependencyChecker.HazardChecker(command, prevCommand))
            {


                (int,int) stallInfo = PipelineDependencyChecker.StallDeterminerAdvanced(__forwardingEnabled, command.inst_, prevCommand.inst_);
                int numOfStalls = stallInfo.Item2;
                int stallStage = stallInfo.Item1;


                if (numOfStalls > 0)
                {
                    int cycleToStartStall = __fetchStack.Peek()._entryCycle + stallStage - 1;
                    (int, int, int) plannedStall = (cycleToStartStall, stallStage, numOfStalls);
                    stallAcc.Enqueue(plannedStall);
                }
            }
            else if (PipelineDependencyChecker.HazardChecker(command, grandFatherCommand))
            {
                (int, int) stallInfo = PipelineDependencyChecker.StallDeterminerAdvanced(__forwardingEnabled, command.inst_, grandFatherCommand.inst_);
                int stallStage = stallInfo.Item1;
                int numOfStalls = stallInfo.Item2 - 1; //minus for heirachy difference
                numOfStalls = (numOfStalls >= 0) ? numOfStalls : 0;

                if (numOfStalls > 0)
                {
                    int cycleToStartStall = __fetchStack.Peek()._entryCycle + stallStage - 1;
                    (int, int, int) plannedStall = (cycleToStartStall, stallStage, numOfStalls);
                    stallAcc.Enqueue(plannedStall);
                }
            }
            else if (PipelineDependencyChecker.HazardChecker(command, greatGrandFatherCommand))
            {
                (int, int) stallInfo = PipelineDependencyChecker.StallDeterminerAdvanced(__forwardingEnabled, command.inst_, greatGrandFatherCommand.inst_);
                int stallStage = stallInfo.Item1;
                int numOfStalls = stallInfo.Item2 - 2; //minus for heirachy difference
                numOfStalls = (numOfStalls >= 0) ? numOfStalls : 0;

                if (numOfStalls > 0)
                {
                    int cycleToStartStall = __fetchStack.Peek()._entryCycle + stallStage - 1;
                    (int, int, int) plannedStall = (cycleToStartStall, stallStage, numOfStalls);
                    stallAcc.Enqueue(plannedStall);
                }
            }
            else
            {

            }
        }

        public void EndPipeline()
        {
            if(!__forwardingEnabled)
                __DecodeStack.Push(__fetchStack.Pop());
            AdvancePipelineByXCycles(5);
        }


        public void NextCycle(bool stall)
        {
            __cycle++;

            /* 
             * remove instructions from the end of the pipeline
             * to simulate a flush
             */
            PipelineObj obj;
            if (__WritebackStack.Count > 0)
            {
                //remove from pipline
                obj = __WritebackStack.Pop();
                //modify string
                obj.str.Add("W");
                //add to output for display
                if (obj.command.inst_.GetKey() != Strings.outputText_stall && obj.command.inst_.GetKey() != Strings.outputText_empty)
                    __output.Add(obj);
            }
            if (__MemoryStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __MemoryStack.Pop();
                //modify string
                obj.str.Add("M");
                //add to next stage of pipeline
                __WritebackStack.Push(obj);
            }
            if (__ExecuteStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __ExecuteStack.Pop();
                //modify string
                obj.str.Add("X");
                //add to next stage of pipeline
                __MemoryStack.Push(obj);
            }
            if (__DecodeStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __DecodeStack.Pop();
                //modify string
                obj.str.Add("D");
                //add to next stage of pipeline
                __ExecuteStack.Push(obj);
            }

            /*
             * If an instruction had not entered the pipeline, a stall must
             * be enqueued
             */
            if (stall)
            {
                if (__fetchStack.Count > 0)
                {
                    //pop to modify pipeline object
                    obj = __fetchStack.Peek();
                    //modify string
                    obj.str.Add("S");
                    //add to next stage of pipeline
                    __DecodeStack.Push(new PipelineObj(InstructionCommand.Stall(), __cycle));
                }
            }
            else
            {
                if (__fetchStack.Count > 0)
                    //add to next stage of pipeline
                    __DecodeStack.Push(__fetchStack.Pop());
            }

            //StateOfPipeline();
        }

        public void NextCycleStallForward(int stallStage)
        {

            /*
             Notes:
             --F was added to the object string at instantiation
             --All objects should receive their letters before going into the respective stack
             --     to symbolize the part of the process they are at
             --Objects below stall stage receive 'S'
             --Objects at stall stage move on to next stack, but stall object is placed in stack
             --Objects above stall stage move on to next stack
             */


            __cycle++;
            PerformFetchStall();
            PerformStallOnStack(__DecodeStack, __ExecuteStack, stallStage, "X", 2);
            PerformStallOnStack(__ExecuteStack, __MemoryStack, stallStage, "M", 3);
            PerformStallOnStack(__MemoryStack, __WritebackStack, stallStage, "W", 4);
            AdvanceWritebackStage();
            StateOfPipeline();
        }

        public void NextCycle()
        {
            __cycle++;

            /* 
             * remove instructions from the end of the pipeline
             * to simulate a flush
             */
            PipelineObj obj;
            if (__WritebackStack.Count > 0)
            {
                //remove from pipline
                obj = __WritebackStack.Pop();
                //add to output for display
                if (obj.command.inst_.GetKey() != Strings.outputText_stall && obj.command.inst_.GetKey() != Strings.outputText_empty)
                    __output.Add(obj);
            }
            if (__MemoryStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __MemoryStack.Pop();
                //modify string
                obj.str.Add("W");
                //add to next stage of pipeline
                __WritebackStack.Push(obj);
            }
            if (__ExecuteStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __ExecuteStack.Pop();
                //modify string
                obj.str.Add("M");
                //add to next stage of pipeline
                __MemoryStack.Push(obj);
            }
            if (__DecodeStack.Count > 0)
            {
                //pop to modify pipeline object
                obj = __DecodeStack.Pop();
                //modify string
                obj.str.Add("X");
                //add to next stage of pipeline
                __ExecuteStack.Push(obj);
            }

            /*
             * If an instruction had not entered the pipeline, a stall must
             * be enqueued
             */


            
            if (__fetchStack.Count > 0)
            {
                //add to next stage of pipeline
                obj = __fetchStack.Pop();
                obj.str.Add("D");
                __DecodeStack.Push(obj);
                
            }
            

            //StateOfPipeline();
        }

        private void PerformFetchStall()
        {
            PipelineObj obj;
            if (__fetchStack.Count > 0)
            {
                /* Mark that the object in the fetch stage has been stalled */
                /* A stall cannot be inserted at the fetch*/
                obj = __fetchStack.Peek();
                obj.str.Add("S");
            }
        }

        private void AdvanceWritebackStage()
        {
            PipelineObj obj;
            if (__WritebackStack.Count > 0)
            {
                //remove from pipline
                obj = __WritebackStack.Pop();
                //modify string
                obj.str.Add("W");
                //add to output for display
                if (obj.command.inst_.GetKey() != Strings.outputText_stall && obj.command.inst_.GetKey() != Strings.outputText_empty)
                    __output.Add(obj);
            }
        }



        private void PerformStallOnStack(Stack<PipelineObj> stack, Stack<PipelineObj> nextStack, int stallStage, string NextLetter, int stageNum)
        {
            PipelineObj obj;
            if (stack.Count <= 0)
                return;

            if (stallStage == stageNum)
            {//If this is the stage to insert a stall

                //pop to modify pipeline object
                obj = stack.Pop();

                //Add letter for stage it is entering
                obj.str.Add(NextLetter);

                //add to next stage of pipeline
                PipelineObj objOther = nextStack.Pop();
                nextStack.Push(obj);
                nextStack.Push(objOther);

                stack.Push(new PipelineObj(InstructionCommand.Stall(), __cycle));
            }
            else if (stallStage < stageNum)
            {
                //pop to modify pipeline object
                obj = stack.Pop();

                //Add letter for stage it is entering
                obj.str.Add(NextLetter);

                //add to next stage of pipeline
                nextStack.Push(obj);
            }
            else
            {
                //object cant move because of stall
                obj = stack.Peek();
                obj.str.Add("S");
            }

        }

        private void Stall()
        {
            __cycle++;
        }

        private void AdvancePipelineByXCycles(int num)
        {
            if (!__forwardingEnabled)
            {
                for (int i = 0; i < num; i++)
                {
                    NextCycle(true);
                    StateOfPipeline();
                }
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    NextCycle();
                    StateOfPipeline();
                }
            }

        }

        private int DependencyCheckNoForwarding(InstructionCommand command)
        {
            int numOfStallCycles = 0;
            InstructionCommand fetchCommand = __fetchStack.Peek().command;
            InstructionCommand DecodeCommand = __DecodeStack.Peek().command;
            InstructionCommand ExecuteCommand = __ExecuteStack.Peek().command;
            InstructionCommand MemoryCommand = __MemoryStack.Peek().command;
            InstructionCommand WritebackCommand = __WritebackStack.Peek().command;
            InstructionCommand newInstruction = command;

            //do something

            return numOfStallCycles;
        }

        private void StateOfPipeline()
        {
            string IF = (__fetchStack.Count > 0) ? __fetchStack.Peek().command.ToString() : InstructionCommand.Stall().ToString();
            string ID = (__DecodeStack.Count > 0) ? __DecodeStack.Peek().command.ToString() : InstructionCommand.Stall().ToString();
            string X = (__ExecuteStack.Count > 0) ? __ExecuteStack.Peek().command.ToString() : InstructionCommand.Stall().ToString();
            string M = (__MemoryStack.Count > 0) ? __MemoryStack.Peek().command.ToString() : InstructionCommand.Stall().ToString();
            string W = (__WritebackStack.Count > 0) ? __WritebackStack.Peek().command.ToString() : InstructionCommand.Stall().ToString();

            //keep a record, or print the state of the pipelien
            string s = String.Format("This is the state of the pipeline at cycle {0}:\n" +
                "Fetch: {1}\n" +
                "Decode: {2}\n" +
                "Execute: {3}\n" +
                "Memory: {4}\n" +
                "WriteBack: {5}\n\n" +
                "----------", 
                __cycle.ToString(),
                IF.ToString(),
                ID.ToString(),
                X.ToString(),
                M.ToString(),
                W.ToString());

            UpdatePipelineState(s);
        }

        private void StateOfPipelineStart()
        {
            string s = (__forwardingEnabled) ? " (Forwarding Enabled)" : "";
            UpdatePipelineState(String.Format("This is the start of the{0} Pipeline",s));
        }

        private void UpdatePipelineState(string s)
        {
            __state += "\n" + s + "\n";
        }

        private void FillPipeline()
        {
            __DecodeStack.Push(new PipelineObj(InstructionCommand.Empty(), __cycle));
            __ExecuteStack.Push(new PipelineObj(InstructionCommand.Empty(), __cycle));
            __MemoryStack.Push(new PipelineObj(InstructionCommand.Empty(), __cycle));
            __WritebackStack.Push(new PipelineObj(InstructionCommand.Empty(), __cycle));
        }
    }
}
