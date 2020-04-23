using System;
using System.Collections.Generic;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
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
            byte[] value = this.Read(address);
            int i;
            for (i = 0; i < 4; i++)
            {
                int index = endian == Endian.BigEndian? i : 3 - i;
                string binary = Convert.ToString(value[index], 16).PadLeft(2, '0');
                strBuilder.Append(binary);
            }
            return strBuilder.ToString();
        }
    }
}
