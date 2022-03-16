using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
    internal class GlobalLanguageEntries
    {
    }

    public class Register
    {

        public Register(string k, string bv, int i = 0)
        {
            key = k;
            binaryValue = bv;
            value = i;
        }

        private string key;
        private string binaryValue;
        private int value;

        public string GetKey()
        {
            return key;
        }

        public string GetBinaryValue()
        {
            return binaryValue;
        }

        public int GetValue()
        {
            return value;
        }

        public void SetValue(int i)
        {
            value = i;
        }
    }

    public enum InstructionType { rType, iType, jType }

    public struct Instruction
    {

        public Instruction(string k, string bv, InstructionType it)
        {
            key = k;
            binaryValue = bv;
            type = it;

            if (it == InstructionType.rType)
                opcode = "000";
            else if (it == InstructionType.jType)
                opcode = "110";
            else if (it == InstructionType.sType)
                opcode = "111";
            else
                opcode = "";
        }

        private string key;
        private string binaryValue;
        private InstructionType type;
        private string opcode;

        public string GetKey()
        {
            return key;
        }

        public string GetBinaryValue()
        {
            return binaryValue;
        }

        public InstructionType GetType()
        {
            return type;
        }

        public string GetOpcode()
        {
            return opcode;
        }

    }

    public static class Globals
    {
        public static string register0 = "$zero";
        public static string register1 = "$at";
        public static string register2 = "$v0";
        public static string register3 = "$v1";
        public static string register4 = "$a0";
        public static string register5 = "$a1";
        public static string register6 = "$a2";
        public static string register7 = "$a3";
        public static string register8 = "$t0";
        public static string register9 = "$t1";
        public static string register10 = "$t2";
        public static string register11 = "$t3";
        public static string register12 = "$t4";
        public static string register13 = "$t5";
        public static string register14 = "$t6";
        public static string register15 = "$t7";
        public static string register16 = "$s0";
        public static string register17 = "$s1";
        public static string register18 = "$s2";
        public static string register19 = "$s3";
        public static string register20 = "$s4";
        public static string register21 = "$s5";
        public static string register22 = "$s6";
        public static string register23 = "$s7";
        public static string register24 = "$t8";
        public static string register25 = "$t9";
        public static string register26 = "$k0";
        public static string register27 = "$k1";
        public static string register28 = "$gp";
        public static string register29 = "$sp";
        public static string register30 = "$fp";
        public static string register31 = "$ra";

        public const string command1 = "add";
        public const string command2 = "sub";
        public const string command3 = "lw";
        public const string command4 = "sw";

        public const int instructionBitLegnth = 20;
        public const int opcodeLength = 3;
        public const int registerLength = 4;
        public const int sTypeImmediate = 8;
        public const int sTypeFuncLength = 5;
        public const int rTypeFuncLength = 3;
        public const int shamtFieldLength = 2;
        public const int immediateFieldLength = 10; //for i-type instructions

        public const string Add = "001";
        public const string Subtract = "010";
        public const string Max = "011";
        public const string Min = "100";

        public const string sysPrintInt = "00000";
        public const string sysPrintDouble = "00001";
        public const string sysPrintFloat = "00010";
        public const string sysPrintInt2 = "00011";
        public const string sysPrintDouble2 = "00100";
        public const string sysPrintFloat2 = "00101";
        public const string Load = "00110";
        public const string Stow = "00111";

        public static string memory0 = "0000000000";
        public static string memory1 = "0000000000";
        public static string memory2 = "0000000000";
        public static string memory3 = "0000000000";
        public static string memory4 = "0000000000";
        public static string memory5 = "0000000000";
        public static string memory6 = "0000000000";
        public static string memory7 = "0000000000";

        public static List<string> MemoryAddresses = new List<string>()
    {
        memory0, memory1, memory2, memory3, memory4, memory5, memory6, memory7
    };

        public static List<string> RegisterArgSysInstructions = new List<string>
    {
        command5, command6, command7
    };

        public static List<string> PrintInstructs = new List<string>() {
            Globals.sysPrintInt, Globals.sysPrintDouble, Globals.sysPrintFloat,
            Globals.sysPrintInt2, Globals.sysPrintDouble2, Globals.sysPrintFloat2
    };

        public static List<string> MemoryInstructions = new List<string>
    {
        command11, command12
    };



        public static Dictionary<string, Instruction> instructionDictionary = new Dictionary<string, Instruction>() {
        { command1, new Instruction(command1, Add, InstructionType.rType) }, //func
        { command3, new Instruction(command3, Subtract, InstructionType.rType) }, //func
        //{ "move", new Instruction("move", "010", InstructionType.rType) }, //func
        { command13, new Instruction(command13, Max, InstructionType.rType) }, //func
        { command15, new Instruction(command15, Min, InstructionType.rType) }, //func

        { command5, new Instruction(command5, sysPrintInt, InstructionType.sType) }, //funct
        { command6, new Instruction(command6, sysPrintDouble, InstructionType.sType) }, //funct
        { command7, new Instruction(command7, sysPrintFloat, InstructionType.sType) }, //funct
        { command8, new Instruction(command8, sysPrintInt2, InstructionType.sType) }, //funct
        { command9, new Instruction(command9, sysPrintDouble2, InstructionType.sType) }, //funct
        { command10, new Instruction(command10, sysPrintFloat2, InstructionType.sType) }, //funct
        { command11, new Instruction(command11, Load, InstructionType.sType) }, //funct
        { command12, new Instruction(command12, Stow, InstructionType.sType) }, //funct

        { command2, new Instruction(command2, Add, InstructionType.iType) }, //opcode
        { command4, new Instruction(command4, Subtract, InstructionType.iType) }, //opcode
        { command14, new Instruction(command14, Max, InstructionType.iType) }, //opcode
        { command16, new Instruction(command16, Min, InstructionType.iType) } //opcode
    };

        public static Dictionary<int, string> MemoryDictionary = new Dictionary<int, string>() {
        { 0, memory0 }, { 1, memory0 }, { 2, memory0 }, { 3, memory0 },
        { 4, memory0 }, { 5, memory0 }, { 6, memory0 }, { 7, memory0 }
    };

        public static Dictionary<string, Register> RegisterDictionary = new Dictionary<string, Register>() {
        { Globals.register1, new Register(Globals.register1, "0000") },
        { Globals.register2, new Register(Globals.register2, "0001") },
        { Globals.register3, new Register(Globals.register3, "0010") },
        { Globals.register4, new Register(Globals.register4, "0011") },
        { Globals.register5, new Register(Globals.register5, "0100") },
        { Globals.register6, new Register(Globals.register6, "0101") },
        { Globals.register7, new Register(Globals.register7, "0110") },
        { Globals.register8, new Register(Globals.register8, "0111") },
        { Globals.register9, new Register(Globals.register9, "1000") },
        { Globals.register10, new Register(Globals.register10, "1001") },
        { Globals.register11, new Register(Globals.register11, "1010") },
        { Globals.register12, new Register(Globals.register12, "1011") },
        { Globals.register13, new Register(Globals.register13, "1100") },
        { Globals.register14, new Register(Globals.register14, "1101") },
        { Globals.register15, new Register(Globals.register15, "1110") },
        { Globals.register16, new Register(Globals.register16, "1111") }
    };
    }
}
