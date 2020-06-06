using System;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public class Machine
    {
        private bool _isDebug = false;  // step by step debug
        public uint Pc;
        public bool IsHalt;  // program has exited

        private MachineCodePack _machineCode;
        public CodeReader Codes;
        public RegisterStorage Register;
        // code is stored in RAM, starting from 0
        public RamStorage Ram;
        public StorageLogger<RegisterType> RegisterLogger { get; private set; } = new StorageLogger<RegisterType>();
        public StorageLogger<uint> RamLogger { get; private set; } = new StorageLogger<uint>();

        public Machine()
        {
            Reset(new MachineCodePack());
        }

        public Machine(MachineCodePack machineCode)
        {
            Reset(machineCode);
        }

        // start from the beginning
        public void Run()
        {
            Reset(_machineCode);
            Continue();
        }
        // go to next step in debug mode
        public void Step()
        {
            _isDebug = true;
            LoopContainer();
        }
        // continue running without debug
        public void Continue()
        {
            _isDebug = false;
            LoopContainer();
        }
        // set the new binary
        public void Reset(MachineCodePack machineCode)
        {
            _machineCode = machineCode;
            this.Pc = 0;
            this.IsHalt = false;
            this.Register = new RegisterStorage(this.RegisterLogger);
            this.Ram = new RamStorage(this.RamLogger);
            this.Codes = new CodeReader(this.Ram, machineCode);
        }
        // query Ram
        public string QueryRamAsHex(uint address, uint length4Bytes = 1, Endian endian = Endian.LittleEndian)
        {
            return this.Ram.ReadAsHex(address, length4Bytes, endian);
        }

        public Word32b QueryRamWord(uint address)
        {
            return this.Ram.Read(address);
        }

        public Word32b QueryMachineCode(uint address)
        {
            return this.Codes.GetOneMachineCode(address);
        }

        public Word32b QueryRegister(RegisterType reg)
        {
            return this.Register.Read(reg);
        }

        // TODO: encapsulate
        public string GetMipsString(uint address, uint length4Bytes)
        {
            return this.Codes.GetMipsString(address, length4Bytes);
        }

        // the main loop happens here
        private void LoopContainer()
        {
            this.RamLogger.Clear();
            this.RegisterLogger.Clear();
            RunNextInstruction();
            while (!_isDebug)
            {
                this.RamLogger.Clear();
                this.RegisterLogger.Clear();
                RunNextInstruction();
            }
        }
        // interpret the next instruction
        private void RunNextInstruction()
        {
            #region instruction fetch
            Word32b code = this.Codes.GetOneMachineCode(this.Pc);
            // auto increment
            this.Pc += 4;
            #endregion

            #region instruction decode
            Instruction instruction = new Instruction(code.ToBinaryString());
            #endregion

            #region execute (ALU) + (write/read) memory + write back (to register)
            if (instruction.Type == FormatType.Halt || instruction.Opcode == Opcode.halt)
            {
                this.IsHalt = true;
                return;
            }
            // modify register and/or RAM
            ModifyMemory(instruction);
            // direct jump (modify PC)
            if (PcDirectJump(instruction))
                return;
            // jump to relative address (modify PC)
            PcRelativeJump(instruction);
            #endregion
        }
        // modify register and/or RAM
        private void ModifyMemory(Instruction instruction)
        {
            int tmpInt;
            uint tmpUInt;
            byte[] tmpBytes;
            switch (instruction.Opcode)
            {
                case Opcode.RType:
                    switch (instruction.Funct)
                    {
                        case Funct.add:
                            tmpInt = this.Register.Read(instruction.Rs).ToInt()
                                + this.Register.Read(instruction.Rt).ToInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpInt));
                            break;
                        case Funct.addu:
                            tmpUInt = this.Register.Read(instruction.Rs).ToUInt()
                                + this.Register.Read(instruction.Rt).ToUInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.and:
                            tmpUInt = this.Register.Read(instruction.Rs).ToUInt()
                                & this.Register.Read(instruction.Rt).ToUInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.nor:
                            tmpUInt = ~(this.Register.Read(instruction.Rs).ToUInt()
                                | this.Register.Read(instruction.Rt).ToUInt());
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.or:
                            tmpUInt = this.Register.Read(instruction.Rs).ToUInt()
                                | this.Register.Read(instruction.Rt).ToUInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        // Unsigned left shift
                        case Funct.sll:
                            tmpUInt = this.Register.Read(instruction.Rt).ToUInt() << (int)instruction.Shamt;
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        // // Unsigned left shift
                        // case Funct.sllv:
                        //     tmpUInt = this.Register.Read(instruction.Rt).ToUInt()
                        //         // workaround : shift right operant has to be int
                        //         << (int)this.Register.Read(instruction.Rs).ToUInt();
                        //     this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                        //     break;
                        case Funct.slt:
                            if (this.Register.Read(instruction.Rs).ToInt()
                                < this.Register.Read(instruction.Rt).ToInt())
                                tmpUInt = 1;
                            else
                                tmpUInt = 0;
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.sltu:
                            if (this.Register.Read(instruction.Rs).ToUInt()
                                < this.Register.Read(instruction.Rt).ToUInt())
                                tmpUInt = 1;
                            else
                                tmpUInt = 0;
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        // // signed right shift
                        // TODO: make shamt a full negative number if the fifth bit is 1
                        // case Funct.sra:
                        //     tmpUInt = this.Register.Read(instruction.Rt).ToUInt() >> (int)instruction.Shamt;
                        //     this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                        //     break;
                        // unsigned right shift
                        case Funct.srl:
                            tmpUInt = this.Register.Read(instruction.Rt).ToUInt() >> (int)instruction.Shamt;
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.sub:
                            tmpInt = this.Register.Read(instruction.Rs).ToInt()
                                - this.Register.Read(instruction.Rt).ToInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpInt));
                            break;
                        case Funct.subu:
                            tmpUInt = this.Register.Read(instruction.Rs).ToUInt()
                                - this.Register.Read(instruction.Rt).ToUInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                        case Funct.xor:
                            tmpUInt = this.Register.Read(instruction.Rs).ToUInt()
                                ^ this.Register.Read(instruction.Rt).ToUInt();
                            this.Register.Write(instruction.Rd, new Word32b(tmpUInt));
                            break;
                    }
                    break;
                case Opcode.addi:
                    tmpInt = this.Register.Read(instruction.Rs).ToInt();
                    tmpInt = instruction.Immediate + tmpInt;
                    this.Register.Write(instruction.Rt, new Word32b(tmpInt));
                    break;
                case Opcode.addiu:
                    tmpUInt = this.Register.Read(instruction.Rs).ToUInt();
                    tmpUInt = (uint)instruction.Immediate + tmpUInt;
                    this.Register.Write(instruction.Rt, new Word32b(tmpUInt));
                    break;
                case Opcode.andi:
                    tmpInt = this.Register.Read(instruction.Rs).ToInt();
                    tmpInt = instruction.Immediate & tmpInt;
                    this.Register.Write(instruction.Rt, new Word32b(tmpInt));
                    break;
                case Opcode.lui:  // $t = (imm << 16);
                    tmpInt = instruction.Immediate << 16;
                    this.Register.Write(instruction.Rt, new Word32b(tmpInt));
                    break;
                case Opcode.lw:  // offset unit := 1 byte
                    tmpUInt = this.Register.Read(instruction.Rs).ToUInt();
                    tmpBytes = this.Ram.Read((uint)((int)tmpUInt + instruction.Immediate)).ToBytes();
                    this.Register.Write(instruction.Rt, new Word32b(tmpBytes));
                    break;
                case Opcode.ori:
                    tmpInt = this.Register.Read(instruction.Rs).ToInt();
                    tmpInt = instruction.Immediate | tmpInt;
                    this.Register.Write(instruction.Rt, new Word32b(tmpInt));
                    break;
                case Opcode.slti:
                    if (this.Register.Read(instruction.Rs).ToInt() < instruction.Immediate)
                        tmpUInt = 1;
                    else
                        tmpUInt = 0;
                    this.Register.Write(instruction.Rt, new Word32b(tmpUInt));
                    break;
                case Opcode.sw:
                    tmpBytes = this.Register.Read(instruction.Rt).ToBytes();
                    tmpUInt = this.Register.Read(instruction.Rs).ToUInt();
                    this.Ram.Write((uint)((int)tmpUInt + instruction.Immediate), new Word32b(tmpBytes));
                    break;
                case Opcode.jal:
                    // https://chortle.ccsu.edu/AssemblyTutorial/Chapter-26/ass26_4.html
                    this.Register.Write(RegisterType.ra, new Word32b(this.Pc));
                    break;
            }
        }
        // return := is performed
        private bool PcDirectJump(Instruction instruction)
        {
            if (instruction.Type == FormatType.Jump)
            {
                this.Pc = (this.Pc & 0xf0000000) | (instruction.WordAddress << 2);  // *= 4
                return true;
            }
            if (instruction.Type == FormatType.Register && instruction.Funct == Funct.jr)
            {
                this.Pc = this.Register.Read(instruction.Rs).ToUInt();
                return true;
            }
            return false;
        }

        // jump to a relative address
        private void PcRelativeJump(Instruction instruction)
        {
            bool isJump = false;
            switch (instruction.Opcode)
            {
                case Opcode.beq:
                    if (this.Register.Read(instruction.Rs).ToInt() == this.Register.Read(instruction.Rt).ToInt())
                        isJump = true;
                    break;
                case Opcode.bgtz:
                    if (this.Register.Read(instruction.Rs).ToInt() > 0)
                        isJump = true;
                    break;
                case Opcode.bne:
                    if (this.Register.Read(instruction.Rs).ToInt() != this.Register.Read(instruction.Rt).ToInt())
                        isJump = true;
                    break;
            }
            if (isJump)  // jump unit := 1 word = 4 bytes = 32 bits
                this.Pc = Convert.ToUInt32((int)this.Pc + (instruction.Immediate << 2));
        }
    }
}
