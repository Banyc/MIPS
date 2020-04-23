using System;
using System.IO;
using System.Text;
using MIPS.Simulator.VirtualMachine;
using MIPS.Shared;

namespace MIPS.Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input binary code here");
            Console.Write("> ");

            string binary = Console.ReadLine();
            Machine vm = new Machine(binary);
            // vm.Continue();  // for developping debug only

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
                        vm.Step();
                        if (vm.IsHalt)
                            Console.WriteLine("Program Exited");
                        else
                            Console.WriteLine($"PC: {vm.Pc} (At #{vm.Pc / 4} instruction)");
                        break;
                    case "r":  // read register
                               // `r t0`
                        RegisterType reg = (RegisterType)Enum.Parse(typeof(RegisterType), input[1]);
                        if (input.Length >= 3)
                            bool.TryParse(input[2], out isBigEndian);
                        Console.WriteLine(vm.QueryRegisterAsHex(reg, isBigEndian ? Endian.BigEndian : Endian.LittleEndian));
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
                        Console.WriteLine(InstructionToMipsString(vm, address, length4Bytes));
                        break;
                    case "a":  // 写汇编指令到内存，
                               // FileStream file = File.Open(input[1], FileMode.Open);
                        break;
                }
            }
        }
        private static string InstructionToMipsString(Machine vm, uint address, uint length4Bytes)
        {
            StringBuilder builder = new StringBuilder();
            uint i;
            for (i = 0; i < length4Bytes; i++)
            {
                byte[] binary = vm.QueryRamAsBytes(address + i * 4);
                Instruction instruction = new Instruction(binary);
                builder.Append(instruction.ToMipsString());
            }
            return builder.ToString();
        }
    }
}
