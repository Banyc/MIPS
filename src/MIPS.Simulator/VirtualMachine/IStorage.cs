using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public interface IStorage<T>
    {
        void Write(T address, Word32b newValue);
        Word32b Read(T address);  // return 4 bytes (32 bits)
    }
}
