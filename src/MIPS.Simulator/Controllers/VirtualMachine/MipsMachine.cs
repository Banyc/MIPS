using System;
using System.Text;
using MIPS.Shared;
using MIPS.Shared.Models;

namespace MIPS.Simulator.Controllers.VirtualMachine
{
    public partial class MipsMachine
    {
        private bool _isDebug = false;  // step by step debug
        public uint Pc { get; set; }
        public bool IsHalt { get; set; }  // program has exited

        private MachineCodePack machineCode;
        public CodeReader Codes { get; set; }
        public RegisterStorage Register { get; set; }
        // code is stored in RAM, starting from 0
        // public RamStorage Ram { get; set; }
        public RamArrayStorage Ram { get; set; }
        public StorageLogger<RegisterType> RegisterLogger { get; } = new StorageLogger<RegisterType>();
        public StorageLogger<uint> RamLogger { get; } = new StorageLogger<uint>();

        public MipsMachine()
        {
            Reset(new MachineCodePack());
        }

        public MipsMachine(MachineCodePack machineCode)
        {
            Reset(machineCode);
        }

        // start from the beginning
        public void Run()
        {
            Reset(this.machineCode);
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
            this.machineCode = machineCode;
            this.Pc = 0;
            this.IsHalt = false;
            this.Register = new RegisterStorage(this.RegisterLogger);
            // this.Ram = new RamStorage(this.RamLogger);
            this.Ram = new RamArrayStorage(this.RamLogger);
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

        public void CleanLog()
        {
            this.RamLogger.Clear();
            this.RegisterLogger.Clear();
        }

        // TODO: encapsulate
        public string GetMipsString(uint address, uint length4Bytes)
        {
            return this.Codes.GetMipsString(address, length4Bytes);
        }

        // the main loop happens here
        private void LoopContainer()
        {
            // CleanLog();
            RunNextInstruction();
            while (!_isDebug && !this.IsHalt)
            {
                // CleanLog();
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
    }
}
