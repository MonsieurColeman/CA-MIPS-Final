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

        public void StartPipelineWithCommand(InstructionCommand command)
        {
            cycle++; //first cycle
            fetchStack.Push(command);
            NextCycle();
        }

        public void AddCommandToPipline(InstructionCommand command)
        {
            int stallcycle = DependencyCheckNoForwarding(command); //incomplete
            if (stallcycle > 0)
                AdvancePipelineByXCycles(stallcycle);
            fetchStack.Push(command);
            NextCycle();
        }

        public void NextCycle()
        {
            cycle++;
            if (fetchStack.Count > 0)
                DecodeStack.Push(fetchStack.Pop());
            if(DecodeStack.Count > 0)
                ExecuteStack.Push(DecodeStack.Pop());
            if (ExecuteStack.Count > 0)
                MemoryStack.Push(ExecuteStack.Pop());
            if (MemoryStack.Count > 0)
                WritebackStack.Push(MemoryStack.Pop());
            if (WritebackStack.Count > 0)
                WritebackStack.Pop();
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
            //keep a record, or print the state of the pipelien
            //to do
        }
    }
}
