using System;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public class RegisterStorage : IStorage<RegisterType>
    {
        // public byte[,] Registers = new byte[32, 4];
        public byte[,] Registers = new byte[32, 4];
        public RegisterStorage()
        {
            int i;
            for (i = 0; i < 32; i++)
            {
                Write((RegisterType)i, BitConverter.GetBytes(0));
            }
        }
        public void Write(RegisterType reg, uint newValue)
        {
            Write(reg, BitConverter.GetBytes(newValue));
        }
        public void Write(RegisterType reg, int newValue)
        {
            Write(reg, BitConverter.GetBytes(newValue));
        }
        public void Write(RegisterType reg, byte[] newValue)
        {
            int i; 
            for (i = 0; i < 4; i++)
            {
                this.Registers[(int)reg, i] = newValue[i];
            }
        }
        public byte[] Read(RegisterType reg)
        {
            byte[] copy = new byte[4];
            int i;
            for (i = 0; i < 4; i++)
            {
                copy[i] = this.Registers[(int)reg, i];
            }
            return copy;
        }
        public int ReadAsInt(RegisterType reg)
        {
            byte[] regValue = Read(reg);
            return BitConverter.ToInt32(regValue);
        }
        public uint ReadAsUInt(RegisterType reg)
        {
            byte[] regValue = Read(reg);
            return BitConverter.ToUInt32(regValue);
        }

        public string ReadAsHex(RegisterType reg, Endian endian = Endian.LittleEndian)
        {
            StringBuilder strBuilder = new StringBuilder();
            byte[] value = this.Read(reg);
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
