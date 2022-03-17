using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MIPSPipelineHazardDetector.UnitTests
{

    [TestClass]
    public class PipelineTests
    {
        [TestMethod]
        public void StallDeterminerTest_LoadThenAdd_NoForwarding()
        {
            //arrange
            Instruction load = Globals.instructionDictionary["lw"]; //pipelined instruction
            Instruction add = Globals.instructionDictionary["add"]; //new instruction

            //act
            int answer = PipelineDependencyChecker.StallDeterminer(false, add, load);

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(answer == 2);
        }

        [TestMethod]
        public void StallDeterminerTest_LoadThenAdds_Forwarding()
        {
            //arrange
            Instruction load = Globals.instructionDictionary["lw"]; //new instruction
            Instruction add = Globals.instructionDictionary["add"]; //pipelined instruction

            //act
            int answer = PipelineDependencyChecker.StallDeterminer(true, add, load);

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(answer == 1);
        }

        [TestMethod]
        public void InstructionComparer_Add_NeedsTiming_Forwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["add"];


            //act
            ((int,int),(int,int)) answer = PipelineDependencyChecker.InstructionComparer(true, add);
            (int,int) needsData = answer.Item1;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (3,1)); //beginning of execution phase
        }

        [TestMethod]
        public void InstructionComparer_Add_NeedsTiming_NoForwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["add"];


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(false, add);
            (int, int) needsData = answer.Item1;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (2,2)); //middle of the decode phase
        }

        [TestMethod]
        public void InstructionComparer_Add_AvailableTiming_Forwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["add"]; 


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(true, add);
            (int, int) needsData = answer.Item2;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (3,3)); //end of the execution phase
        }

        [TestMethod]
        public void InstructionComparer_Add_AvailableTiming_NoForwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["add"]; 


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(false, add);
            (int, int) needsData = answer.Item2;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (5,2)); //middle of the writeback phase
        }

        [TestMethod]
        public void InstructionComparer_Load_NeedsTiming_Forwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["lw"];


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(true, add);
            (int, int) needsData = answer.Item1;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (4, 1)); //beginning of memory phase
        }

        [TestMethod]
        public void InstructionComparer_Load_NeedsTiming_NoForwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["lw"];


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(false, add);
            (int, int) needsData = answer.Item1;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (2, 2)); //middle of the decode phase
        }

        [TestMethod]
        public void InstructionComparer_Load_AvailableTiming_Forwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["lw"];


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(true, add);
            (int, int) needsData = answer.Item2;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (4, 3)); //end of the memory phase
        }

        [TestMethod]
        public void InstructionComparer_Load_AvailableTiming_NoForwarding()
        {
            //arrange
            Instruction add = Globals.instructionDictionary["lw"];


            //act
            ((int, int), (int, int)) answer = PipelineDependencyChecker.InstructionComparer(false, add);
            (int, int) needsData = answer.Item2;

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(needsData == (5, 2)); //middle of the writeback phase
        }

        [TestMethod]
        public void HazardDetector_LoadThenAdd_True()
        {
            //arrange
            InstructionCommand load = new InstructionCommand(Globals.instructionDictionary["lw"],
                Globals.RegisterDictionary["$s1"],
                Globals.RegisterDictionary["$s0"],
                null, 5);
            InstructionCommand add = new InstructionCommand(Globals.instructionDictionary["add"],
                Globals.RegisterDictionary["$s1"],
                Globals.RegisterDictionary["$s1"],
                Globals.RegisterDictionary["$s0"]);

            //act
            bool answer = PipelineDependencyChecker.HazardChecker(add, load);

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(answer);
        }

        [TestMethod]
        public void HazardDetector_LoadThenAdd_False()
        {
            //arrange
            InstructionCommand load = new InstructionCommand(Globals.instructionDictionary["lw"],
                Globals.RegisterDictionary["$s1"],
                Globals.RegisterDictionary["$s0"],
                null, 5);
            InstructionCommand add = new InstructionCommand(Globals.instructionDictionary["add"],
                Globals.RegisterDictionary["$s1"],
                Globals.RegisterDictionary["$s2"],
                Globals.RegisterDictionary["$s0"]);

            //act
            bool answer = PipelineDependencyChecker.HazardChecker(add, load);

            //assert
            Console.WriteLine(answer);
            Assert.IsTrue(!answer);
        }
    }
}
