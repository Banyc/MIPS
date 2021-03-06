
using System;
using System.Text;

namespace MIPS.Shared.Models
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
        addiu = 0b001001,
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
        halt = 0b111111
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
        public RegisterType Rs;
        // R Format
        // I Format
        public RegisterType Rt;
        // R Format
        public RegisterType Rd;
        // R Format
        public uint Shamt = 0;  // shift amount
        // R Format
        public Funct Funct;
        // I Format
        // set at the second round of tree traversal if it is an offset of addresses
        public int Immediate = 0;  // sign extended
        public int ImmediateZeroExtended = 0;
        // J Format
        public uint WordAddress = 0;  // set at the second round of tree traversal
        public string TargetLabelIdentity = null;  // set by j, beq, and others
        // only allowed in pseudo instruction
        // pseudo instruction could be split to several intructions
        public Instruction NextInstruction = null;

        public Instruction()
        {

        }
        public Instruction(string binary)
        {
            Initialize(binary);
        }

        private void Initialize(string binary)
        {
            // Parsing binary code is based on Opcode
            this.Opcode = (Opcode)Convert.ToUInt32(binary.Substring(0, 6), 2);
            if (this.Opcode == Opcode.RType)
                this.Type = FormatType.Register;
            else if (this.Opcode == Opcode.j || this.Opcode == Opcode.jal)
                this.Type = FormatType.Jump;
            else if (this.Opcode == Opcode.halt)
                this.Type = FormatType.Halt;
            else
                this.Type = FormatType.Immediate;
            switch (this.Type)
            {
                case FormatType.Register:
                    this.Rs = (RegisterType)Convert.ToUInt32(binary.Substring(6, 5), 2);
                    this.Rt = (RegisterType)Convert.ToUInt32(binary.Substring(11, 5), 2);
                    this.Rd = (RegisterType)Convert.ToUInt32(binary.Substring(16, 5), 2);
                    this.Shamt = Convert.ToUInt32(binary.Substring(21, 5), 2);
                    this.Funct = (Funct)Convert.ToUInt32(binary.Substring(26, 6), 2);
                    break;
                case FormatType.Immediate:
                    this.Rs = (RegisterType)Convert.ToUInt32(binary.Substring(6, 5), 2);
                    this.Rt = (RegisterType)Convert.ToUInt32(binary.Substring(11, 5), 2);
                    string immBinary = binary.Substring(16, 16);
                    immBinary = new string(immBinary[0], 16) + immBinary;
                    this.Immediate = Convert.ToInt32(immBinary, 2);
                    this.ImmediateZeroExtended = this.Immediate & 0x0000ffff;
                    break;
                case FormatType.Jump:
                    this.WordAddress = Convert.ToUInt32(binary.Substring(6, 26), 2);
                    break;
            }
        }

        public string ToBinaryString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Convert.ToString((int)this.Opcode, 2).PadLeft(6, '0'));
            string tmp;
            switch (this.Type)
            {
                case FormatType.Halt:
                    builder.Append(Convert.ToString((uint)this.WordAddress, 2).PadLeft(26, '0'));
                    break;
                case FormatType.Register:
                    builder.Append(Convert.ToString((uint)this.Rs, 2).PadLeft(5, '0'));
                    builder.Append(Convert.ToString((uint)this.Rt, 2).PadLeft(5, '0'));
                    builder.Append(Convert.ToString((uint)this.Rd, 2).PadLeft(5, '0'));
                    tmp = Convert.ToString((int)this.Shamt, 2).PadLeft(5, '0');
                    builder.Append(tmp.Substring(tmp.Length - 5));  // negative number has a fixed length
                    builder.Append(Convert.ToString((uint)this.Funct, 2).PadLeft(6, '0'));
                    break;
                case FormatType.Immediate:
                    builder.Append(Convert.ToString((uint)this.Rs, 2).PadLeft(5, '0'));
                    builder.Append(Convert.ToString((uint)this.Rt, 2).PadLeft(5, '0'));
                    tmp = Convert.ToString((int)this.Immediate, 2).PadLeft(16, '0');
                    builder.Append(tmp.Substring(tmp.Length - 16));  // negative number has a fixed length
                    break;
                case FormatType.Jump:
                    builder.Append(Convert.ToString((uint)this.WordAddress, 2).PadLeft(26, '0'));
                    break;
            }
            return builder.ToString();
        }

        // result appended with "\n"
        public string ToMipsString()
        {
            StringBuilder builder = new StringBuilder();
            switch (this.Type)
            {
                case FormatType.Halt:
                    builder.Append(this.Opcode.ToString("g"));
                    break;
                case FormatType.Register:
                    // Handle Shamt
                    if (this.Funct == Funct.srl
                        || this.Funct == Funct.sll)
                    {
                        builder.Append(this.Funct.ToString("g"));
                        builder.Append(" $");
                        builder.Append(this.Rd.ToString("g"));
                        builder.Append(", $");
                        builder.Append(this.Rt.ToString("g"));
                        builder.Append(", ");
                        builder.Append(this.Shamt.ToString("g"));
                        break;
                    }
                    if (this.Funct == Funct.sllv
                        || this.Funct == Funct.srlv)
                    {
                        builder.Append(this.Funct.ToString("g"));
                        builder.Append(" $");
                        builder.Append(this.Rd.ToString("g"));
                        builder.Append(", $");
                        builder.Append(this.Rt.ToString("g"));
                        builder.Append(", $");
                        builder.Append(this.Rs.ToString("g"));
                        break;
                    }
                    // others
                    builder.Append(this.Funct.ToString("g"));
                    builder.Append(" $");
                    builder.Append(this.Rd.ToString("g"));
                    builder.Append(", $");
                    builder.Append(this.Rs.ToString("g"));
                    builder.Append(", $");
                    builder.Append(this.Rt.ToString("g"));
                    break;
                case FormatType.Immediate:
                    if (this.Opcode == Opcode.beq
                        || this.Opcode == Opcode.bne)
                    {
                        // beq $s, $t, offset
                        builder.Append(this.Opcode.ToString("g"));
                        builder.Append(" $");
                        builder.Append(this.Rs.ToString("g"));
                        builder.Append(", $");
                        builder.Append(this.Rt.ToString("g"));
                        builder.Append(", ");
                        builder.Append(this.Immediate);
                        break;
                    }
                    // others
                    builder.Append(this.Opcode.ToString("g"));
                    builder.Append(" $");
                    builder.Append(this.Rt.ToString("g"));
                    if (this.Opcode == Opcode.lw
                        || this.Opcode == Opcode.sw)
                    {
                        builder.Append(", ");
                        builder.Append(this.Immediate);
                        builder.Append("($");
                        builder.Append(this.Rs.ToString("g"));
                        builder.Append(")");
                        break;
                    }
                    builder.Append(", $");
                    builder.Append(this.Rs.ToString("g"));
                    builder.Append(", ");
                    builder.Append(this.Immediate);
                    break;
                case FormatType.Jump:
                    builder.Append(this.Opcode.ToString("g"));
                    builder.Append(" ");
                    builder.Append(this.WordAddress);
                    break;
            }
            builder.Append("\n");
            return builder.ToString();
        }
    }
}
