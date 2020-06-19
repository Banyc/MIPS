namespace MIPS.Shared.Models
{
    public enum RegisterType
    {
        zero,
        // reserved for assembler
        at,
        // values
        // Stores results
        v0 = 2,
        v1,
        // arguments
        a0,
        a1,
        a2,
        a3,
        // temp, not saved
        t0,
        t1,
        t2,
        t3,
        t4,
        t5,
        t6,
        t7,
        t8 = 24,
        t9,
        // Contents saved for use later
        s0 = 16,
        s1,
        s2,
        s3,
        s4,
        s5,
        s6,
        s7,
        // Reserved by operating system
        k0 = 26,
        k1,
        // Global pointer
        gp = 28,
        // Stack pointer
        sp = 29,
        // Frame pointer
        fp = 30,
        // return address
        ra = 31,
    }
}
