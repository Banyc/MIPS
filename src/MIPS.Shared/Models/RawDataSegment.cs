using System.Collections.Generic;
namespace MIPS.Shared.Models
{
    public class RawDataSegment
    {
        public uint StartAddress;
        public List<byte> Data = new List<byte>();
    }
}
