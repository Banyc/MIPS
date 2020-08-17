using System;
using System.Text;

namespace MIPS.Shared.Models
{
    public enum CodingSystem
    {
        Binary,
        Hex
    }

    public enum Endian
    {
        LittleEndian,
        BigEndian,
    }

    // the class store a word of 32 bits (4 bytes)
    // A word := one machine code
    // this class responsible for any type convertion in a word
    public class Word32b
    {
        // stored in little-endian
        public readonly byte[] Value = new byte[4];

        public Word32b(string value, CodingSystem system)
        {
            switch (system)
            {
                case CodingSystem.Binary:
                    if (value.Length < 32)
                        throw new Exception("Length of binary string should be 32");
                    break;
                case CodingSystem.Hex:
                    if (value.Length < 8)
                        throw new Exception("Length of Hex string should be 8");
                    break;
                default:
                    break;
            }
            int i;
            for (i = 0; i < 4; i++)
            {
                switch (system)
                {
                    case CodingSystem.Binary:
                        this.Value[3 - i] = Convert.ToByte(value.Substring(i * 8, 8), 2);
                        break;
                    case CodingSystem.Hex:
                        this.Value[3 - i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
                        break;
                    default:
                        break;
                }
            }
        }

        public Word32b(uint value)
        {
            this.Value = BitConverter.GetBytes(value);
        }

        public Word32b(int value)
        {
            this.Value = BitConverter.GetBytes(value);
        }

        public Word32b(byte[] value)
        {
            if (value.Length != 4)
                throw new Exception("Length of Byte array should be 4");
            this.Value = value;
        }

        public string ToHexString(Endian endian = Endian.LittleEndian)
        {
            StringBuilder strBuilder = new StringBuilder();
            int i;
            for (i = 0; i < 4; i++)
            {
                int index = endian == Endian.BigEndian ? i : 3 - i;
                string hexString = Convert.ToString(this.Value[index], 16).PadLeft(2, '0');
                strBuilder.Append(hexString);
            }
            return strBuilder.ToString();
        }

        public string ToBinaryString(Endian endian = Endian.LittleEndian)
        {
            StringBuilder strBuilder = new StringBuilder();
            int i;
            for (i = 0; i < 4; i++)
            {
                int index = endian == Endian.BigEndian ? i : 3 - i;
                string binary = Convert.ToString(this.Value[index], 2).PadLeft(8, '0');
                strBuilder.Append(binary);
            }
            return strBuilder.ToString();
        }

        public string ToMipsString()
        {
            Instruction instruction = new Instruction(ToBinaryString());
            return instruction.ToMipsString();
        }

        public int ToInt()
        {
            return BitConverter.ToInt32(this.Value, 0);
        }

        public uint ToUInt()
        {
            return BitConverter.ToUInt32(this.Value, 0);
        }

        public byte[] ToBytes()
        {
            return this.Value;
        }
    }
}
