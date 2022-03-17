using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    public class Pipeline
    {
        static int cycle = 0;
        static Stack<InstructionCommand> fetchStack = new Stack<InstructionCommand>();
        static Stack<InstructionCommand> DecodeStack = new Stack<InstructionCommand>();
        static Stack<InstructionCommand> ExecuteStack = new Stack<InstructionCommand>();
        static Stack<InstructionCommand> MemoryStack = new Stack<InstructionCommand>();
        static Stack<InstructionCommand> WritebackStack = new Stack<InstructionCommand>();
        public string __state = "";

        public void StartPipelineWithCommand(InstructionCommand command)
        {
            StateOfPipelineStart();
            cycle++; //first cycle
            fetchStack.Push(command);
            StateOfPipeline();
            //NextCycle();
        }

        public void AddCommandToPipline(InstructionCommand command)
        {
            /*
            int stallcycle = DependencyCheckNoForwarding(command); //incomplete
            if (stallcycle > 0)
                AdvancePipelineByXCycles(stallcycle);
            */
            NextCycle();
            fetchStack.Push(command);
            //NextCycle();
            StateOfPipeline();

        }

        public void NextCycle()
        {
            cycle++;

            /* 
             * remove instructions from the end of the pipeline
             * to simulate a flush
             */
            if (WritebackStack.Count > 0)
                WritebackStack.Pop();
            if (MemoryStack.Count > 0)
                WritebackStack.Push(MemoryStack.Pop());
            if (ExecuteStack.Count > 0)
                MemoryStack.Push(ExecuteStack.Pop());
            if (DecodeStack.Count > 0)
                ExecuteStack.Push(DecodeStack.Pop());

            /*
             * If an instruction had not entered the pipeline, a stall must
             * be enqueued
             */
            if (fetchStack.Count > 0)
                DecodeStack.Push(fetchStack.Pop());
            else
                DecodeStack.Push(InstructionCommand.Stall());

            //StateOfPipeline();
        }

        public void NextCycleTest()
        {
            cycle++;

            //remove instruction at the end of the pipeline
            if (WritebackStack.Count > 0)
                WritebackStack.Pop();

            //enqueue instructions from the previous state of the pipeline
            //if nothing was in that stage, place a stall cycle in
            if (MemoryStack.Count > 0)
                WritebackStack.Push(MemoryStack.Pop());
            else
                WritebackStack.Push(InstructionCommand.Stall());


            DecodeStack.Push(InstructionCommand.Stall());

            StateOfPipeline();
        }

        private void Stall()
        {
            cycle++;
        }

        private void StallByValue(int num)
        {
            cycle += num;
        }

        private void AdvancePipelineByXCycles(int num)
        {
            for (int i = 0; i < num; i++)
                NextCycle();
        }

        private int DependencyCheckNoForwarding(InstructionCommand command)
        {
            int numOfStallCycles = 0;
            InstructionCommand fetchCommand = fetchStack.Peek();
            InstructionCommand DecodeCommand = DecodeStack.Peek();
            InstructionCommand ExecuteCommand = ExecuteStack.Peek();
            InstructionCommand MemoryCommand = MemoryStack.Peek();
            InstructionCommand WritebackCommand = WritebackStack.Peek();
            InstructionCommand newInstruction = command;

            //do something

            return numOfStallCycles;
        }

        private void StateOfPipeline()
        {
            InstructionCommand IF = (fetchStack.Count > 0) ? fetchStack.Peek() : InstructionCommand.Stall();
            InstructionCommand ID = (DecodeStack.Count > 0) ? DecodeStack.Peek() : InstructionCommand.Stall();
            InstructionCommand X = (ExecuteStack.Count > 0) ? ExecuteStack.Peek() : InstructionCommand.Stall();
            InstructionCommand M = (MemoryStack.Count > 0) ? MemoryStack.Peek() : InstructionCommand.Stall();
            InstructionCommand W = (WritebackStack.Count > 0) ? WritebackStack.Peek() : InstructionCommand.Stall();


            //keep a record, or print the state of the pipelien
            string s = String.Format("This is the state at of the pipeline at cycle {0}:\n" +
                "Fetch: {1}\n" +
                "Decode: {2}\n" +
                "Execute: {3}\n" +
                "Memory: {4}\n" +
                "WriteBack {5}\n\n" +
                "----------", 
                cycle.ToString(),
                IF.ToString(),
                ID.ToString(),
                X.ToString(),
                M.ToString(),
                W.ToString());

            UpdatePipelineState(s);
        }

        private void StateOfPipelineStart()
        {
            UpdatePipelineState("This is the start of the Pipeline");
        }

        private void UpdatePipelineState(string s)
        {
            __state += "\n" + s + "\n";
        }
    }
}
