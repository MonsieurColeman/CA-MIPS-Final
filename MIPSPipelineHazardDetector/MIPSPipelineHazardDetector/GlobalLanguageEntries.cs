using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSPipelineHazardDetector
{
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

        public override string ToString()
        {
            return key;
        }
    }

    public enum InstructionType { rType, iType, jType, stall, empty }

    public struct Instruction
    {

        public Instruction(string k, string bv, InstructionType it)
        {
            key = k;
            binaryValue = bv;
            type = it;
            funct = "000000";

            if (it == InstructionType.rType)
            {
                opcode = "000000";
                funct = bv;
            }
            else
                opcode = bv;
        }

        private string key;
        private string binaryValue;
        private InstructionType type;
        private string opcode;
        private string funct;

        public string GetKey()
        {
            return key;
        }

        public string GetBinaryValue()
        {
            return binaryValue;
        }

        public InstructionType GetInstructionType()
        {
            return type;
        }

        public string GetOpcode()
        {
            return opcode;
        }

        public string GetFunct()
        {
            return funct;
        }

        public static Instruction Stall()
        {
            return new Instruction(Globals.command0, Globals.stall_binary, InstructionType.stall);
        }

        public static Instruction Empty()
        {
            return new Instruction(Strings.outputText_empty, Globals.stall_binary, InstructionType.empty);
        }

        public override string ToString()
        {
            return key;
        }
    }

    public struct InstructionCommand
    {
        public Instruction inst_;
        public InstructionType type_;
        public Register rs_;
        public Register rt_;
        public Register rd_;
        public int immediate_;
        public int wordAddress_;
        public bool rTypeImmediateFlag_;
        public int shamt_;

        public InstructionCommand(Instruction i, Register rs = null, Register rt = null, Register rd = null,
            int immediate = 0, bool rTypeImmediateFlag = false, int wordAddress = 0, int shamt = 0)
        {
            inst_ = i;
            type_ = i.GetInstructionType();
            rs_ = rs;
            rt_ = rt;
            rd_ = rd;
            immediate_ = immediate;
            wordAddress_ = wordAddress;
            rTypeImmediateFlag_ = rTypeImmediateFlag;
            shamt_ = shamt;
        }

        public override string ToString()
        { //convert back to input syntax
            string returnStr = "";
            switch (type_)
            {

                case InstructionType.rType:
                    if (rTypeImmediateFlag_)
                        returnStr =
                            inst_.ToString() + " " +
                            rs_.ToString() + ", " +
                            rt_.ToString() + ", " +
                            immediate_.ToString();
                    else
                        returnStr =
                            inst_.ToString() + " " +
                            rs_.ToString() + ", " +
                            rt_.ToString() + ", " +
                            rd_.ToString();
                    break;
                case InstructionType.iType:
                    returnStr =
                        inst_.ToString() + " " +
                        rs_.ToString() + ", " +
                        immediate_.ToString() +
                        "(" + rt_.ToString() + ")";
                    break;
                case InstructionType.stall:
                    returnStr = Strings.outputText_stall;
                    break;
                case InstructionType.empty:
                    returnStr = Strings.outputText_empty;
                    break;
                default:
                    returnStr = base.ToString();
                    break;
            }
            return returnStr;
        }

        public static InstructionCommand Stall()
        {
            return new InstructionCommand(Instruction.Stall());
        }

        public static InstructionCommand Empty()
        {
            return new InstructionCommand(Instruction.Empty());
        }
    }

    public static class Globals
    {
        public static readonly string register0 = "$zero";
        public static readonly string register1 = "$at";
        public static readonly string register2 = "$v0";
        public static readonly string register3 = "$v1";
        public static readonly string register4 = "$a0";
        public static readonly string register5 = "$a1";
        public static readonly string register6 = "$a2";
        public static readonly string register7 = "$a3";
        public static readonly string register8 = "$t0";
        public static readonly string register9 = "$t1";
        public static readonly string register10 = "$t2";
        public static readonly string register11 = "$t3";
        public static readonly string register12 = "$t4";
        public static readonly string register13 = "$t5";
        public static readonly string register14 = "$t6";
        public static readonly string register15 = "$t7";
        public static readonly string register16 = "$s0";
        public static readonly string register17 = "$s1";
        public static readonly string register18 = "$s2";
        public static readonly string register19 = "$s3";
        public static readonly string register20 = "$s4";
        public static readonly string register21 = "$s5";
        public static readonly string register22 = "$s6";
        public static readonly string register23 = "$s7";
        public static readonly string register24 = "$t8";
        public static readonly string register25 = "$t9";
        public static readonly string register26 = "$k0";
        public static readonly string register27 = "$k1";
        public static readonly string register28 = "$gp";
        public static readonly string register29 = "$sp";
        public static readonly string register30 = "$fp";
        public static readonly string register31 = "$ra";

        public static readonly string command0 = "stall";
        public static readonly string command1 = "add";
        public static readonly string command2 = "sub";
        public static readonly string command3 = "lw";
        public static readonly string command4 = "sw";
        public static readonly string command5 = "xor";

        public static readonly int instructionBitLegnth = 32;
        public static readonly int opcodeLength = 6;
        public static readonly int registerLength = 5;
        public static readonly int  jTypeWordAddressLength = 26;
        public static readonly int rTypeFuncLength = 6;
        public static readonly int shamtFieldLength = 5;
        public static readonly int immediateFieldLength = 16;

        public static readonly string add_binary = "100000";
        public static readonly string xor_binary = "100001";
        public static readonly string sub_binary = "100010";
        public static readonly string lw_binary = "100011";
        public static readonly string sw_binary = "101011";
        public static readonly string stall_binary = "111111";

        public static string memory0 = "0000000000";
        public static string memory1 = "0000000000";
        public static string memory2 = "0000000000";
        public static string memory3 = "0000000000";
        public static string memory4 = "0000000000";
        public static string memory5 = "0000000000";
        public static string memory6 = "0000000000";
        public static string memory7 = "0000000000";

        public static readonly List<string> MemoryAddresses = new List<string>()
        {
            memory0, memory1, memory2, memory3, memory4, memory5, memory6, memory7
        };

        public static readonly List<string> MemoryInstructions = new List<string>
        {
            command3, command4
        };

        public static Dictionary<string, Instruction> instructionDictionary = new Dictionary<string, Instruction>() {
        { command0, new Instruction(command0, stall_binary, InstructionType.stall) }, //func
        { command1, new Instruction(command1, add_binary, InstructionType.rType) }, //func
        { command2, new Instruction(command2, sub_binary, InstructionType.rType) }, //func
        { command5, new Instruction(command5, xor_binary, InstructionType.rType) }, //func
        { command3, new Instruction(command3, lw_binary, InstructionType.iType) }, //load
        { command4, new Instruction(command4, sw_binary, InstructionType.iType) }, //store
    };

        public static Dictionary<int, string> MemoryDictionary = new Dictionary<int, string>() {
        { 0, memory0 }, { 1, memory0 }, { 2, memory0 }, { 3, memory0 },
        { 4, memory0 }, { 5, memory0 }, { 6, memory0 }, { 7, memory0 }
    };

        public static Dictionary<string, Register> RegisterDictionary = new Dictionary<string, Register>() {
        { Globals.register0, new Register(Globals.register0, "00000") },
        { Globals.register1, new Register(Globals.register1, "00001") },
        { Globals.register2, new Register(Globals.register2, "00010") },
        { Globals.register3, new Register(Globals.register3, "00011") },
        { Globals.register4, new Register(Globals.register4, "00100") },
        { Globals.register5, new Register(Globals.register5, "00101") },
        { Globals.register6, new Register(Globals.register6, "00110") },
        { Globals.register7, new Register(Globals.register7, "00111") },
        { Globals.register8, new Register(Globals.register8, "01000") },
        { Globals.register9, new Register(Globals.register9, "01001") },
        { Globals.register10, new Register(Globals.register10, "01010") },
        { Globals.register11, new Register(Globals.register11, "01011") },
        { Globals.register12, new Register(Globals.register12, "01100") },
        { Globals.register13, new Register(Globals.register13, "01101") },
        { Globals.register14, new Register(Globals.register14, "01110") },
        { Globals.register15, new Register(Globals.register15, "01111") },
        { Globals.register16, new Register(Globals.register16, "10000") },
        { Globals.register17, new Register(Globals.register17, "10001") },
        { Globals.register18, new Register(Globals.register18, "10010") },
        { Globals.register19, new Register(Globals.register19, "10011") },
        { Globals.register20, new Register(Globals.register20, "10100") },
        { Globals.register21, new Register(Globals.register21, "10101") },
        { Globals.register22, new Register(Globals.register22, "10110") },
        { Globals.register23, new Register(Globals.register23, "10111") },
        { Globals.register24, new Register(Globals.register24, "11000") },
        { Globals.register25, new Register(Globals.register25, "11001") },
        { Globals.register26, new Register(Globals.register26, "11010") },
        { Globals.register27, new Register(Globals.register27, "11011") },
        { Globals.register28, new Register(Globals.register28, "11100") },
        { Globals.register29, new Register(Globals.register29, "11101") },
        { Globals.register30, new Register(Globals.register30, "11110") },
        { Globals.register31, new Register(Globals.register31, "11111") }
    };
    }
}
