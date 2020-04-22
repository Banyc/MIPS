using System;
using System.Collections.Generic;

namespace MIPS_simulator.VirtualMachine
{
    public class RamStorage : IStorage<uint>
    {
        public Dictionary<uint, byte> Memory = new Dictionary<uint, byte>();
        public RamStorage()
        {
        }
        public void Write(uint address, uint newValue)
        {
            Write(address, BitConverter.GetBytes(newValue));
        }
        public void Write(uint address, int newValue)
        {
            Write(address, BitConverter.GetBytes(newValue));
        }
        public void Write(uint address, byte[] newValue)
        {
            uint i;
            for (i = 0; i < 4; i++)
            {
                this.Memory[address + i] = newValue[i];
            }
        }
        public byte[] Read(uint address)
        {
            byte[] copy = new byte[4];
            uint i;
            for (i = 0; i < 4; i++)
            {
                if (this.Memory.ContainsKey(address + i))
                    copy[i] = this.Memory[address + i];
                // else use default value
            }
            return copy;
        }
        public int ReadAsInt(uint address)
        {
            byte[] regValue = Read(address);
            return BitConverter.ToInt32(regValue);
        }
        public uint ReadAsUInt(uint address)
        {
            byte[] regValue = Read(address);
            return BitConverter.ToUInt32(regValue);
        }
    }
}
