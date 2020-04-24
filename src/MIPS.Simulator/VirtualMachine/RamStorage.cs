using System;
using System.Collections.Generic;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public class RamStorage : IStorage<uint>
    {
        public Dictionary<uint, Word32b> Memory = new Dictionary<uint, Word32b>();
        public RamStorage()
        {
        }

        public string ReadAsHex(uint address, uint length4Bytes = 1, Endian endian = Endian.LittleEndian)
        {
            StringBuilder strBuilder = new StringBuilder();
            int remaining = (int)length4Bytes;
            uint addressSoFar = address;
            while (remaining > 0)
            {
                strBuilder.Append(ReadAsHex(addressSoFar, endian));
                strBuilder.Append(" ");
                remaining -= 1;
            }
            return strBuilder.ToString();
        }

        public string ReadAsHex(uint address, Endian endian = Endian.LittleEndian)
        {
            StringBuilder strBuilder = new StringBuilder();
            byte[] value = this.Read(address).ToBytes();
            int i;
            for (i = 0; i < 4; i++)
            {
                int index = endian == Endian.BigEndian? i : 3 - i;
                string binary = Convert.ToString(value[index], 16).PadLeft(2, '0');
                strBuilder.Append(binary);
            }
            return strBuilder.ToString();
        }

        public void Write(uint address, Word32b newValue)
        {
            this.Memory[address] = newValue;
        }

        public Word32b Read(uint address)
        {
            if (!this.Memory.ContainsKey(address))
                return new Word32b(0);
            return this.Memory[address];
        }
    }
}
