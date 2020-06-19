using MIPS.Shared;
using MIPS.Shared.Models;

namespace MIPS.Simulator.Interfaces
{
    public interface IStorage<T>
    {
        void Write(T address, Word32b newValue);
        Word32b Read(T address);  // return 4 bytes (32 bits)
    }
}
