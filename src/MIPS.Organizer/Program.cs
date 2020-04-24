using System;
using MIPS.Simulator.VirtualMachine;
using MIPS.Interpreter.Interpreter;
using MIPS.Shared;
using System.Text;

namespace MIPS.Organizer
{
    class Program
    {
        static void Main(string[] args)
        {
            MIPS.Simulator.VirtualMachine.Machine vm = new Simulator.VirtualMachine.Machine();
            MIPS.Interpreter.Interpreter.MipsToBinary converter = new MIPS.Interpreter.Interpreter.MipsToBinary();

            ProgramInfo prog = new ProgramInfo();
        
            // interactive
            while (true)
            {
                bool isBigEndian = false;
                uint address;
                uint length4Bytes = 1;

                Console.Write("> ");
                string[] input = Console.ReadLine().Split();
                switch (input[0])
                {
                    case "s":  // go to next step
                        if (vm.IsHalt)
                            break;
                        Console.WriteLine($"{(new Instruction(vm.QueryMachineCode(vm.Pc).ToBinaryString())).ToMipsString()}");  // print the executing instruction
                        vm.Step();
                        if (vm.IsHalt)
                            Console.WriteLine("Program Exited");
                        else
                        {
                            Console.WriteLine($"PC: {vm.Pc} (At #{vm.Pc / 4} instruction)");
                            Console.WriteLine($"{(new Instruction(vm.QueryMachineCode(vm.Pc).ToBinaryString())).ToMipsString()}");  // print the next instruction
                        }
                        break;
                    case "r":  // read register
                               // `r t0`
                        RegisterType reg = (RegisterType)Enum.Parse(typeof(RegisterType), input[1]);
                        if (input.Length >= 3)
                            bool.TryParse(input[2], out isBigEndian);
                        Console.WriteLine(vm.QueryRegister(reg).ToBinaryString(isBigEndian ? Endian.BigEndian : Endian.LittleEndian));
                        break;
                    case "d":  // read RAM
                               // `d <address> <length/4Bytes> <0:little-endian/1:big-endian>`
                        uint.TryParse(input[1], out address);
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        if (input.Length >= 4)
                            bool.TryParse(input[3], out isBigEndian);
                        Console.WriteLine(vm.QueryRamAsHex(address, length4Bytes, isBigEndian ? Endian.BigEndian : Endian.LittleEndian));
                        break;
                    case "u":  // 指令方式看内存，
                               // binarty -> MIPS asm
                               // `u <address> <length/4Bytes>`
                        uint.TryParse(input[1], out address);
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        Console.WriteLine(vm.GetMipsString(address, length4Bytes));
                        
                        break;
                    case "a":  // 写汇编指令到内存，
                               // FileStream file = File.Open(input[1], FileMode.Open);
                        string mips = AskForMipsCode();
                        prog = converter.ParseMips(mips);
                        MachineCodePack machineCode = prog.ToCodePack();
                        vm.Reset(machineCode);
                        Console.WriteLine("Done reading");
                        break;
                }
            }
        }

        private static string AskForMipsCode()
        {
            StringBuilder builder = new StringBuilder();
            Console.WriteLine("Input MIPS code here. End with continuous enters");
            Console.Write("> ");

            while (true)
            {
                string code = Console.ReadLine();
                if (code == "")
                    break;
                builder.Append(code);
                builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}
