using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared
{
    // this class only serves for AST (abstract syntax tree)
    // the class does not take responsibility for disassembled MIPS codes
    public class ProgramInfo
    {
        public List<Statement> Statements = new List<Statement>();

        public MachineCodePack ToMachineCode()
        {
            List<Word32b> code = new List<Word32b>();
            foreach (var stat in this.Statements)
            {
                string binary = stat.Instruction.ToBinaryString();
                Word32b word = new Word32b(binary, CodingSystem.Binary);
                code.Add(word);
            }
            MachineCodePack codePack = new MachineCodePack(code);
            return codePack;
        }
    }
}
