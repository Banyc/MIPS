namespace MIPS_interpreter.Interpreter
{
    public enum Register
    {
        zero,
        // reserved for assembler
        at,
        // values
        v0 = 2,
        v1,
        // arguments
        a0,
        a1,
        a2,
        a3,
        // temp
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
        // saved
        s0 = 16,
        s1,
        s2,
        s3,
        s4,
        s5,
        s6,
        s7,
        sp = 29,
        fp = 30,
        // return address
        ra = 31,
    }
}
