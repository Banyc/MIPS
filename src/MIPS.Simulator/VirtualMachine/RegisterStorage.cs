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
        private readonly StorageLogger<RegisterType> _logger;

        public RegisterStorage(StorageLogger<RegisterType> logger)
        {
            _logger = logger;
        }

        public void Write(RegisterType address, Word32b newValue)
        {
            _logger.LogChange(address, this.Read(address), newValue);
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
