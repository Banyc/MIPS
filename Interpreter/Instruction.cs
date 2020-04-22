
using System;

namespace MIPS_interpreter.Interpreter
{
    public enum FormatType
    {
        Register,
        Immediate,
        Jump,
        Halt
    }

    public enum Funct
    {
        add = 0b100000,
        addu = 0b100001,
        and = 0b100100,
        break_ = 0b001101,
        div = 0b011010,
        divu = 0b011011,
        jalr = 0b001001,
        jr = 0b001000,
        mfhi = 0b010000,
        mflo = 0b010010,
        mthi = 0b010001,
        mtlo = 0b010011,
        mult = 0b011000,
        multu = 0b011001,
        nor = 0b100111,
        or = 0b100101,
        sll = 0b000000,
        sllv = 0b000100,
        slt = 0b101010,
        sltu = 0b101011,
        sra = 0b000011,
        srav = 0b000111,
        srl = 0b000010,
        srlv = 0b000110,
        sub = 0b100010,
        subu = 0b100011,
        syscall = 0b001100,
        xor = 0b100110,
    }

    public enum Opcode
    {
        RType = 0b000000,
        addi = 0b001000,
        addiu = 0b001000,
        andi = 0b001100,
        beq = 0b000100,
        bgez = 0b000001,
        bgtz = 0b000111,
        blez = 0b000110,
        bltz = 0b000001,
        bne = 0b000101,
        lb = 0b100000,
        lbu = 0b100100,
        lh = 0b100001,
        lhu = 0b100101,
        lui = 0b001111,
        lw = 0b100011,
        lwel = 0b110001,
        ori = 0b001101,
        sb = 0b101000,
        slti = 0b001010,
        sltiu = 0b001011,
        sh = 0b101001,
        sw = 0b101011,
        swel = 0b111001,
        xori = 0b001110,
        j = 0b000010,
        jal = 0b000011,
    }

    public class Instruction
    {
        public FormatType Type;

        // R Format
        // I Format
        // J Format
        public Opcode Opcode;
        // R Format
        // I Format
        public Register Rs;
        // R Format
        // I Format
        public Register Rt;
        // R Format
        public Register Rd;
        // R Format
        public uint Shamt = 0;  // shift amount
        // R Format
        public Funct Funct;
        // I Format
        // set at the second round of tree traversal if it is an offset of addresses
        public int Immediate = 0;
        // J Format
        public uint WordAddress = 0;  // set at the second round of tree traversal
        public string TargetLabelIdentity = null;  // set by j, beq, and others
        // only allowed in pseudo instruction
        // pseudo instruction could be split to several intructions
        public Instruction NextInstruction = null;

        public string ToBinaryString()
        {
            string binary = "";
            binary += Convert.ToString((int)this.Opcode, 2).PadLeft(6, '0');
            string tmp;
            switch (this.Type)
            {
                case FormatType.Register:
                    binary += Convert.ToString((uint)this.Rs, 2).PadLeft(5, '0');
                    binary += Convert.ToString((uint)this.Rt, 2).PadLeft(5, '0');
                    binary += Convert.ToString((uint)this.Rd, 2).PadLeft(5, '0');
                    tmp = Convert.ToString((int)this.Shamt, 2).PadLeft(5, '0');
                    binary += tmp.Substring(tmp.Length - 5);  // negative number has a fixed length
                    binary += Convert.ToString((uint)this.Funct, 2).PadLeft(6, '0');
                    break;
                case FormatType.Immediate:
                    binary += Convert.ToString((uint)this.Rs, 2).PadLeft(5, '0');
                    binary += Convert.ToString((uint)this.Rt, 2).PadLeft(5, '0');
                    tmp = Convert.ToString((int)this.Immediate, 2).PadLeft(16, '0');
                    binary += tmp.Substring(tmp.Length - 16);  // negative number has a fixed length
                    break;
                case FormatType.Jump:
                    binary += Convert.ToString((uint)this.WordAddress, 2).PadLeft(26, '0');
                    break;
            }
            return binary;
        }
    }
}
