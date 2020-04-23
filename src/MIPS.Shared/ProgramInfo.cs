using System.Collections.Generic;
using System.Text;

namespace MIPS.Shared
{
    public class ProgramInfo
    {
        public List<Statement> Statements = new List<Statement>();

        public string ToBinaryString(bool isWithNewLines)
        {
            StringBuilder binary = new StringBuilder();
            foreach (var stat in this.Statements)
            {
                binary.Append(stat.Instruction.ToBinaryString());
                if (isWithNewLines)
                    binary.Append("\n");
            }
            return binary.ToString();
        }
    }
}
