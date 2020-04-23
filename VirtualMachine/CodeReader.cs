using System;

namespace MIPS.Simulator.VirtualMachine
{
    public class CodeReader
    {
        public string Binary = "";
        public CodeReader(string binary)
        {
            this.Binary = binary;
        }

        public string GetCodeString(uint addr)
        {
            if (addr >= this.Binary.Length / 8)
                return null;
            return this.Binary.Substring((int)addr * 8, 32);
        }
    }
}
