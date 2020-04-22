using System;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace MIPS_interpreter.Interpreter
{
    public class MipsToBinary
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
            return prog;
        }

        private uint GetAddress(ProgramInfo prog, string targetLabelIdentity)
        {
            uint count = 0;
            foreach (Statement stat in prog.Statements)
            {
                if (stat.Label?.Identity == targetLabelIdentity)
                    break;
                count += 1;
            }
            // TODO: report when label not found
            return count;
        }

        private uint GetAddress(ProgramInfo prog, Statement statement)
        {
            uint count = 0;
            foreach (Statement stat in prog.Statements)
            {
                if (stat == statement)
                    break;
                count += 1;
            }
            return count;
        }
        
        public string MipsToBinaryMethod(string input, bool isWithNewLines) {
            ProgramInfo prog = ParseMips(input);
            StringBuilder binary = new StringBuilder();
            foreach (var stat in prog.Statements)
            {
                binary.Append(stat.Instruction.ToBinaryString());
                if (isWithNewLines)
                    binary.Append("\n");
            }
            return binary.ToString();
        }
    }
}
