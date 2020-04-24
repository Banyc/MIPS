using System;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    // responsible for machine code query and disassembly based on an existing IStorage
    public class CodeReader
    {
        private IStorage<uint> _storage;
        // write machine codes to storage
        public CodeReader(IStorage<uint> storage, MachineCodePack machineCodes)
        {
            _storage = storage;
            uint addr = 0;
            foreach (Word32b word in machineCodes)
            {
                this._storage.Write(addr, word);
                addr += 4;
            }
        }

        // disassemble
        public string GetMipsString(uint address, uint length4Bytes)
        {
            StringBuilder builder = new StringBuilder();
            uint i;
            for (i = 0; i < length4Bytes; i++)
            {
                string binaryStr = this.GetOneMachineCode(address + i * 4).ToBinaryString();
                Instruction instruction = new Instruction(binaryStr);
                builder.Append(instruction.ToMipsString());
            }
            return builder.ToString();
        }

        // query
        public Word32b GetOneMachineCode(uint addr)
        {
            return this._storage.Read(addr);
        }
    }
}
