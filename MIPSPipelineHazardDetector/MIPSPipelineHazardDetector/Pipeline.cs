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

            public PipelineObj(InstructionCommand ic)
            {
                command = ic;
                str = new List<string>() { "F"};
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
            __fetchStack.Push(new PipelineObj(command));
            StateOfPipeline();
            //NextCycle();
        }

        public void AddCommandToPipline(InstructionCommand command)
        {
            InstructionCommand prevCommand = __fetchStack.Peek().command;
            InstructionCommand grandFatherCommand = __DecodeStack.Peek().command;
            InstructionCommand greatGrandFatherCommand = __DecodeStack.Peek().command;


            //move everyone out of the way
            NextCycle(false);

            //instruction always gets into fetch
            __fetchStack.Push(new PipelineObj(command));

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
                stall = PipelineDependencyChecker.StallDeterminer(__forwardingEnabled, command.inst_, grandFatherCommand.inst_ ) - 1;
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

        /*
        public void AddCommandToPipline(InstructionCommand command)
        {
            //move everyone out of the way
            NextCycle(false);

            //instruction always gets into fetch
            __fetchStack.Push(new PipelineObj(command));

            //if there is a hazard,
            if (PipelineDependencyChecker.HazardChecker(command, __fetchStack.Peek().command))
            {
                StateOfPipeline();
                int stall = 0;
                stall = PipelineDependencyChecker.StallDeterminer(__forwardingEnabled, command.inst_, __fetchStack.Peek().command.inst_);
                AdvancePipelineByXCycles(stall);
            }
            else
            {
                StateOfPipeline();
            }
        }
        */

        public void EndPipeline()
        {
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
                    __DecodeStack.Push(new PipelineObj(InstructionCommand.Stall()));
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


        private void Stall()
        {
            __cycle++;
        }

        private void AdvancePipelineByXCycles(int num)
        {
            for (int i = 0; i < num; i++)
            {
                NextCycle(true);
                StateOfPipeline();
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
            __DecodeStack.Push(new PipelineObj(InstructionCommand.Empty()));
            __ExecuteStack.Push(new PipelineObj(InstructionCommand.Empty()));
            __MemoryStack.Push(new PipelineObj(InstructionCommand.Empty()));
            __WritebackStack.Push(new PipelineObj(InstructionCommand.Empty()));
        }
    }
}
