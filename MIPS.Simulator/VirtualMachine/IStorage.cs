namespace MIPS.Simulator.VirtualMachine
{
    public enum Endian
    {
        LittleEndian,
        BigEndian,
    }

    public interface IStorage<T>
    {
        void Write(T address, uint newValue);
        void Write(T address, int newValue);
        void Write(T address, byte[] newValue);
        byte[] Read(T address);  // return 4 bytes (32 bits)
        int ReadAsInt(T address);
        uint ReadAsUInt(T address);
        string ReadAsHex(T address, Endian endian = Endian.LittleEndian);
    }
}
