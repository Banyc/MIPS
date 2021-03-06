using System;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MIPS.Shared;
using MIPS.Shared.Models;

namespace MIPS.Interpreter.Interpreter
{
    // main entry for the interpreter
    public partial class MipsToBinary
    {
        public ProgramInfo ParseMips(string input)
        {
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new MipsAsmLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            MipsAsmParser parser = new MipsAsmParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();
            // Console.WriteLine(tree.ToStringTree());

            MipsAsmVisitorToBinary visitor = new MipsAsmVisitorToBinary(); 
            ProgramInfo prog = (ProgramInfo)visitor.Visit(tree);

            // second round to update the WordAddress field
            foreach (var stat in prog.Statements)
            {
                if (stat.Instruction.Type == FormatType.Jump)
                {
                    // update the WordAddress field since Visiter didn't know where is the exact address at the first parse
                    uint address = GetAddress(prog, stat.Instruction.TargetLabelIdentity);
                    stat.Instruction.WordAddress = address;
                }
                // set offset, NOT absolute address!
                else if (stat.Instruction.Type == FormatType.Immediate)
                {
                    if (stat.Instruction.TargetLabelIdentity != null)
                    {
                        uint targetAddress = GetAddress(prog, stat.Instruction.TargetLabelIdentity);
                        uint thisAddress = GetAddress(prog, stat);
                        stat.Instruction.Immediate = (int)targetAddress - (int)thisAddress - 1;
                    }
                }
            }

            // parse raw data segments
            prog = ParseRawDataSegments(prog, input);

            return prog;
        }

        private uint GetAddress(ProgramInfo prog, string targetLabelIdentity)
        {
            bool isFound = false;
            uint count = 0;
            foreach (Statement stat in prog.Statements)
            {
                if (stat.Label?.Identity == targetLabelIdentity)
                {
                    isFound = true;
                    break;
                }
                count += 1;
            }
            // TODO: report when label not found
            if (!isFound)
                Console.WriteLine($"[ERROR] Label \"{targetLabelIdentity}\" not found!");
            return count;
        }

        private uint GetAddress(ProgramInfo prog, Statement statement)
        {
            uint count = 0;
            foreach (Statement stat in prog.Statements)
            {
                if (Object.ReferenceEquals(stat, statement))
                    break;
                count += 1;
            }
            return count;
        }
        
        public string GetBinaryString(string mipsCode, bool isWithNewLines) {
            ProgramInfo prog = ParseMips(mipsCode);
            MachineCodePack codePack = prog.ToMachineCode();
            return codePack.ToBinaryString(isWithNewLines);
        }

        public string GetHexString(string mipsCode, bool isWithNewLines, bool isBytePartition) {
            ProgramInfo prog = ParseMips(mipsCode);
            MachineCodePack codePack = prog.ToMachineCode();
            string hex = codePack.ToHexString(false);
            if (isBytePartition || isWithNewLines)
            {
                StringBuilder partitioned = new StringBuilder();
                int cursor = 0;
                while (cursor < hex.Length)
                {
                    partitioned.Append(hex.Substring(cursor, 2));
                    cursor += 2;
                    if (cursor % 8 == 0 && isWithNewLines)
                    {
                        partitioned.Append("\n");
                    }
                    else if (isBytePartition)
                    {
                        partitioned.Append(" ");
                    }
                }
                return partitioned.ToString();
            }
            return hex;
        }
    }
}
