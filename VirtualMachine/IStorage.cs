namespace MIPS_simulator.VirtualMachine
{
    public interface IStorage<T>
    {
        void Write(T address, uint newValue);
        void Write(T address, int newValue);
        void Write(T address, byte[] newValue);
        byte[] Read(T address);
        int ReadAsInt(T address);
        uint ReadAsUInt(T address);
    }
}
