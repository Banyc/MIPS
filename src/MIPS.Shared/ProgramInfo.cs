using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared
{
    public class ProgramInfo
    {
        public List<Statement> Statements = new List<Statement>();

        public CodePack ToCodePack()
        {
            List<Word32b> code = new List<Word32b>();
            foreach (var stat in this.Statements)
            {
                string binary = stat.Instruction.ToBinaryString();
                Word32b word = new Word32b(binary, CodingSystem.Binary);
                code.Add(word);
            }
            CodePack codePack = new CodePack(code);
            return codePack;
        }
    }
}
