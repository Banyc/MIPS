using System;
using System.Collections.Generic;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public class RegisterStorage : IStorage<RegisterType>
    {
        // public byte[,] Registers = new byte[32, 4];
        public Dictionary<RegisterType, Word32b> Registers = new Dictionary<RegisterType, Word32b>();

        public RegisterStorage()
        {
        }

        public void Write(RegisterType address, Word32b newValue)
        {
            this.Registers[address] = newValue;
        }

        public Word32b Read(RegisterType address)
        {
            if (!this.Registers.ContainsKey(address))
                return new Word32b(0);
            return this.Registers[address];
        }
    }
}
