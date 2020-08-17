using System;
using System.Collections.Generic;
using System.Text;
using MIPS.Shared;
using MIPS.Shared.Models;
using MIPS.Simulator.Interfaces;

namespace MIPS.Simulator.Controllers.VirtualMachine
{
    public class RamArrayStorage : IStorage<uint>
    {
        // data size for RAM := word = 32 bits (4 bytes)
        // address unit := 4 bits (1 byte)
        // public Dictionary<uint, byte> Memory = new Dictionary<uint, byte>();
        public uint[] Memory = new uint[1024];
        private readonly StorageLogger<uint> _logger;

        public RamArrayStorage(StorageLogger<uint> logger)
        {
            _logger = logger;
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
                addressSoFar += 4;
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
                int index = endian == Endian.BigEndian ? i : 3 - i;
                string binary = Convert.ToString(value[index], 16).PadLeft(2, '0');
                strBuilder.Append(binary);
            }
            return strBuilder.ToString();
        }

        // address in byte, not word
        public void Write(uint address, Word32b newValue)
        {
            _logger.LogChange(address, this.Read(address), newValue);

            int offset = (int)address % 4;
            int index = (int)address / 4;

            int currentOffset = offset;
            int currentIndex = index;
            int i;
            for (i = 0; i < 4; i++)
            {
                byte[] word = BitConverter.GetBytes(this.Memory[currentIndex]);
                word[currentOffset] = newValue.ToBytes()[i];
                this.Memory[currentIndex] = BitConverter.ToUInt32(word);

                currentOffset++;
                if (currentOffset >= 4)
                {
                    currentOffset = 0;
                    currentIndex++;
                }
            }
        }

        // address in byte, not word
        public Word32b Read(uint address)
        {
            byte[] read = new byte[4];

            int offset = (int)address % 4;
            int index = (int)address / 4;

            int currentOffset = offset;
            int currentIndex = index;
            int i;
            for (i = 0; i < 4; i++)
            {
                read[i] = BitConverter.GetBytes(this.Memory[currentIndex])[currentOffset];

                currentOffset++;
                if (currentOffset >= 4)
                {
                    currentOffset = 0;
                    currentIndex++;
                }
            }

            return new Word32b(read);
        }
    }
}
