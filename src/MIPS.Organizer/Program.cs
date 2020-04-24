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

            Prologue();
        
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
                        Console.WriteLine("-----");
                        // print the executing instruction
                        if (vm.Pc > 8)
                            Console.Write($"{(vm.Pc / 4 - 1).ToString().PadLeft(2, ' ')}    {vm.GetMipsString(vm.Pc - 8, 1)}");
                        if (vm.Pc > 4)
                            Console.Write($"{(vm.Pc / 4 - 0).ToString().PadLeft(2, ' ')}    {vm.GetMipsString(vm.Pc - 4, 1)}");
                        Console.Write($"{(vm.Pc / 4 + 1).ToString().PadLeft(2, ' ')}    {vm.GetMipsString(vm.Pc, 1)}");
                        // go to next step
                        vm.Step();
                        // print the next instruction
                        if (vm.IsHalt)
                            Console.WriteLine("Program Exited");
                        else
                        {
                            Console.Write($"{(vm.Pc / 4 + 1).ToString().PadLeft(2, ' ')} -> {vm.GetMipsString(vm.Pc, 1)}");  // print the next instruction
                            Console.Write($"{(vm.Pc / 4 + 2).ToString().PadLeft(2, ' ')}    {vm.GetMipsString(vm.Pc + 4, 1)}");
                            Console.Write($"{(vm.Pc / 4 + 3).ToString().PadLeft(2, ' ')}    {vm.GetMipsString(vm.Pc + 8, 1)}");
                        }
                        break;
                    case "r":  // read register
                               // `r t0`
                        RegisterType reg = (RegisterType)Enum.Parse(typeof(RegisterType), input[1]);
                        if (input.Length >= 3)
                            bool.TryParse(input[2], out isBigEndian);
                        Console.WriteLine($"0x{vm.QueryRegister(reg).ToHexString(isBigEndian ? Endian.BigEndian : Endian.LittleEndian)}");
                        break;
                    case "d":  // read RAM
                               // `d <address> <length/4Bytes> <0:little-endian/1:big-endian>`
                        uint.TryParse(input[1], out address);
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        if (input.Length >= 4)
                            bool.TryParse(input[3], out isBigEndian);
                        Console.WriteLine($"0x {vm.QueryRamAsHex(address, length4Bytes, isBigEndian ? Endian.BigEndian : Endian.LittleEndian)}");
                        break;
                    case "u":  // read RAM as instructions
                               // binarty -> MIPS asm
                               // `u <address> <length/4Bytes>`
                        uint.TryParse(input[1], out address);
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        Console.WriteLine(vm.GetMipsString(address, length4Bytes));
                        
                        break;
                    case "a":  // write MIPS codes to RAM
                               // `a`
                        string mips = AskForMipsCode();
                        ProgramInfo prog = converter.ParseMips(mips);
                        MachineCodePack machineCode = prog.ToMachineCode();
                        vm.Reset(machineCode);
                        break;
                }
            }
        }

        private static void Prologue()
        {
            Console.WriteLine("case \"s\":  go to next step");
            Console.WriteLine("- `s`");
            Console.WriteLine("case \"r\":  read register");
            Console.WriteLine("- `r t0`");
            Console.WriteLine("case \"d\":  read RAM");
            Console.WriteLine("- `d <address> <length/4Bytes> <0:little-endian/1:big-endian>`");
            Console.WriteLine("case \"u\":  read RAM as instructions");
            Console.WriteLine("- `u <address> <length/4Bytes>`");
            Console.WriteLine("case \"a\":  write MIPS codes to RAM");
            Console.WriteLine("- `a`");
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
            Console.WriteLine("Done reading");
            return builder.ToString();
        }
    }
}
