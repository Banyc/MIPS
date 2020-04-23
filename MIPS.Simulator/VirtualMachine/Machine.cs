using System;
using System.Text;
using MIPS.Shared;

namespace MIPS.Simulator.VirtualMachine
{
    public class Machine
    {
        private bool _isDebug = false;  // step by step debug
        public uint Pc = 0;
        public bool IsHalt = false;  // program has exited

        public CodeReader Code;
        public RegisterStorage Register;
        public RamStorage Ram;

        public Machine(string binary)
        {
            Reset(binary);
        }

        // start from the beginning
        public void Run()
        {
            Reset(this.Code.Binary);
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
        public void Reset(string binary)
        {
            this.IsHalt = false;
            this.Code = new CodeReader(binary);
            this.Register = new RegisterStorage();
            this.Ram = new RamStorage();
        }
        // query Ram
        public string QueryRamAsHex(uint address, uint length4Bytes = 1, Endian endian = Endian.LittleEndian)
        {
            return this.Ram.ReadAsHex(address, length4Bytes, endian);
        }

        public byte[] QueryRamAsBytes(uint address)
        {
            return this.Ram.Read(address);
        }

        public string QueryRegisterAsHex(RegisterType reg, Endian endian = Endian.LittleEndian)
        {
            return this.Register.ReadAsHex(reg, endian);
        }

        // the main loop happens here
        private void LoopContainer()
        {
            RunNextInstruction();
            while (!_isDebug)
            {
                RunNextInstruction();
            }
        }
        // interpret the next instruction
        private void RunNextInstruction()
        {
            #region instruction fetch
            string code = this.Code.GetCodeString(this.Pc);
            if (code == null)
            {
                this.IsHalt = true;
                _isDebug = true;  // hit EOF. Stop running
                return;
            }
            // auto increment
            this.Pc += 4;
            #endregion

            #region instruction decode
            Instruction instruction = new Instruction(code);
            #endregion

            #region execute (ALU) + (write/read) memory + write back (to register)
            // modify register and/or RAM
            ModifyMemory(instruction);
            // direct jump (modify PC)
            if (instruction.Type == FormatType.Jump)
            {
                this.Pc = (this.Pc & 0xf0000000) | (instruction.WordAddress << 2);  // *= 4
                return;
            }
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
                            tmpInt = this.Register.ReadAsInt(instruction.Rs)
                                + this.Register.ReadAsInt(instruction.Rt);
                            this.Register.Write(instruction.Rd, tmpInt);
                            break;
                        case Funct.addu:
                            tmpUInt = this.Register.ReadAsUInt(instruction.Rs)
                                + this.Register.ReadAsUInt(instruction.Rt);
                            this.Register.Write(instruction.Rd, tmpUInt);
                            break;
                        case Funct.and:
                            tmpUInt = this.Register.ReadAsUInt(instruction.Rs)
                                & this.Register.ReadAsUInt(instruction.Rt);
                            this.Register.Write(instruction.Rd, tmpUInt);
                            break;
                        case Funct.sub:
                            tmpInt = this.Register.ReadAsInt(instruction.Rs)
                                - this.Register.ReadAsInt(instruction.Rt);
                            this.Register.Write(instruction.Rd, tmpInt);
                            break;
                        case Funct.subu:
                            tmpUInt = this.Register.ReadAsUInt(instruction.Rs)
                                - this.Register.ReadAsUInt(instruction.Rt);
                            this.Register.Write(instruction.Rd, tmpUInt);
                            break;
                    }
                    break;
                case Opcode.addi:
                    tmpInt = this.Register.ReadAsInt(instruction.Rs);
                    tmpInt = instruction.Immediate + tmpInt;
                    this.Register.Write(instruction.Rt, tmpInt);
                    break;
                case Opcode.lw:
                    tmpUInt = this.Register.ReadAsUInt(instruction.Rs);
                    tmpBytes = this.Ram.Read((uint)((int)tmpUInt + instruction.Immediate));
                    this.Register.Write(instruction.Rt, tmpBytes);
                    break;
                case Opcode.sw:
                    tmpBytes = this.Register.Read(instruction.Rt);
                    tmpUInt = this.Register.ReadAsUInt(instruction.Rs);
                    this.Ram.Write((uint)((int)tmpUInt + instruction.Immediate), tmpBytes);
                    break;
                case Opcode.jal:
                    // https://chortle.ccsu.edu/AssemblyTutorial/Chapter-26/ass26_4.html
                    this.Register.Write(RegisterType.ra, this.Pc + 4);
                    break;
            }
        }
        // jump to a relative address
        private void PcRelativeJump(Instruction instruction)
        {
            bool isJump = false;
            switch (instruction.Opcode)
            {
                case Opcode.beq:
                    if (this.Register.ReadAsInt(instruction.Rs) == this.Register.ReadAsInt(instruction.Rt))
                        isJump = true;
                    break;
                case Opcode.bne:
                    if (this.Register.ReadAsInt(instruction.Rs) != this.Register.ReadAsInt(instruction.Rt))
                        isJump = true;
                    break;
            }
            if (isJump)
                this.Pc = Convert.ToUInt32((int)this.Pc + (instruction.Immediate << 2));
        }
    }
}
