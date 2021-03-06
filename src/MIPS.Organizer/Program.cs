﻿using System;
using System.IO;
using MIPS.Interpreter.Interpreter;
using MIPS.Shared;
using System.Text;
using MIPS.Shared.Models;
using MIPS.Simulator.Controllers.VirtualMachine;

namespace MIPS.Organizer
{
    class Program
    {
        static void Main(string[] args)
        {
            MipsMachine vm = new MipsMachine();
            MipsToBinary converter = new MipsToBinary();

            Prologue();

            // interactive
            while (true)
            {
                bool isLittleEndian = true;
                uint address;
                uint length4Bytes = 1;

                Console.Write("> ");
                string[] input = Console.ReadLine().Split();
                switch (input[0])
                {
                    case "c":  // continue
                        Continue(vm);
                        break;
                    case "s":  // go to next step
                        Step(vm);
                        break;
                    case "r":  // read register
                               // `r t0`
                        RegisterType reg = (RegisterType)Enum.Parse(typeof(RegisterType), input[1]);
                        if (input.Length >= 3)
                            bool.TryParse(input[2], out isLittleEndian);
                        Console.WriteLine($"0x{vm.QueryRegister(reg).ToHexString(isLittleEndian ? Endian.LittleEndian : Endian.BigEndian)}");
                        break;
                    case "d":  // read RAM
                               // `d <address> <length/4Bytes> <0:little-endian/1:big-endian>`
                        try
                        {
                            address = (uint)new System.ComponentModel.UInt32Converter().ConvertFromString(input[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("[Error] Invalid Address");
                            continue;
                        }
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        if (input.Length >= 4)
                            bool.TryParse(input[3], out isLittleEndian);
                        Console.WriteLine($"0x {vm.QueryRamAsHex(address, length4Bytes, isLittleEndian ? Endian.LittleEndian : Endian.BigEndian)}");
                        break;
                    case "u":  // read RAM as instructions
                               // binarty -> MIPS asm
                               // `u <address> <length/4Bytes>`
                        try
                        {
                            address = (uint)new System.ComponentModel.UInt32Converter().ConvertFromString(input[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("[Error] Invalid Address");
                            continue;
                        }
                        if (input.Length >= 3)
                            uint.TryParse(input[2], out length4Bytes);
                        Console.WriteLine(vm.GetMipsString(address, length4Bytes));

                        break;
                    case "a":  // write MIPS codes to RAM
                               // `a`
                               // `a <path to asm>`
                        string mips;
                        if (input.Length > 1)
                            mips = File.ReadAllText(input[1]);
                        else
                            mips = AskForMipsCode();
                        ProgramInfo prog = converter.ParseMips(mips);
                        MachineCodePack machineCode = prog.ToMachineCode();
                        vm.Reset(machineCode);
                        // print the next instructions
                        PrintNextInstructions(vm, 16);
                        // do not log the changes caused by program loading
                        vm.CleanLog();
                        break;
                    case "h":  // write hex to RAM
                               // `h <path to hex>`
                        WriteRawTextCodeToRam(input[1], vm, CodingSystem.Hex);
                        break;
                    case "b":  // write binary to RAM
                               // `b <path to binary>`
                        WriteRawTextCodeToRam(input[1], vm, CodingSystem.Binary);
                        break;
                }
            }
        }

        private static void WriteRawTextCodeToRam(string filePath, MipsMachine vm, CodingSystem coding)
        {
            string rawTextCode;
            try
            {
                rawTextCode = File.ReadAllText(filePath);
            }
            catch (Exception)
            {
                Console.WriteLine("[Error] Invalid file");
                return;
            }
            MachineCodePack machineCode = new MachineCodePack(rawTextCode, coding);
            vm.Reset(machineCode);

            // print the next instructions
            PrintNextInstructions(vm, 16);
            // // print changes
            // PrintChanges(vm);
            vm.CleanLog();
        }

        private static void Continue(MipsMachine vm)
        {
            vm.Continue();
            PrintChanges(vm);
            vm.CleanLog();
        }

        private static void Step(MipsMachine vm, int contextSize = 8)
        {
            if (vm.IsHalt)
                return;
            Console.WriteLine("-----");
            // print the executing instructions
            int i;
            for (i = contextSize - 1; i >= 0; i--)
            {
                if (vm.Pc >= i * 4)
                    Console.Write($"{(vm.Pc / 4 - i + 1).ToString().PadLeft(2, ' ')}    {vm.GetMipsString((uint)(vm.Pc - i * 4), 1)}");
            }
            // go to next step
            vm.Step();
            // print the next instructions
            PrintNextInstructions(vm, contextSize);
            // print changes
            PrintChanges(vm);
            vm.CleanLog();
        }

        private static void PrintNextInstructions(MipsMachine vm, int contextSize = 8)
        {
            int i;
            // print the next instructions
            if (vm.IsHalt)
                Console.WriteLine("Program Exited");
            else
            {
                Console.Write($"{(vm.Pc / 4 + 1).ToString().PadLeft(2, ' ')} -> {vm.GetMipsString(vm.Pc, 1)}");  // print the next instruction
                for (i = 1; i < contextSize; i++)
                {
                    Console.Write($"{(vm.Pc / 4 + i + 1).ToString().PadLeft(2, ' ')}    {vm.GetMipsString((uint)(vm.Pc + i * 4), 1)}");
                }
            }
        }

        private static void PrintChanges(MipsMachine vm)
        {
            // print changes
            // RAM
            if (!vm.RamLogger.IsEmpty)
            {
                Console.WriteLine();
                // Console.WriteLine("RAM changes: ");
                foreach (var pair in vm.RamLogger.GetLog())
                {
                    Console.Write($"0x{pair.Key.ToString("x")}: 0x{pair.Value.oldValue.ToHexString()} -> 0x{pair.Value.newValue.ToHexString()} ");
                }
                Console.WriteLine();
            }
            // Registers
            if (!vm.RegisterLogger.IsEmpty)
            {
                Console.WriteLine();
                // Console.WriteLine("Register changes: ");
                foreach (var pair in vm.RegisterLogger.GetLog())
                {
                    Console.Write($"{pair.Key}: 0x{pair.Value.oldValue.ToHexString()} -> 0x{pair.Value.newValue.ToHexString()} ");
                }
                Console.WriteLine();
            }
        }

        private static void Prologue()
        {
            Console.WriteLine("\nHello MIPS simulator\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("    case \"c\":  continue until instruction `halt` is hit");
            Console.WriteLine("    - `c`");
            Console.WriteLine("    case \"s\":  go to next step");
            Console.WriteLine("    - `s`");
            Console.WriteLine("    case \"r\":  read register");
            Console.WriteLine("    - `r t0`");
            Console.WriteLine("    case \"d\":  read RAM");
            Console.WriteLine("    - `d <address> <length in 4 Bytes>`");
            Console.WriteLine("    - `d <address> <length in 4 Bytes> <True:little-endian/False:big-endian>`");
            Console.WriteLine("    case \"u\":  read RAM as instructions");
            Console.WriteLine("    - `u <address> <length in 4 Bytes>`");
            Console.WriteLine("    case \"a\":  write MIPS codes to RAM");
            Console.WriteLine("    - `a`");
            Console.WriteLine("    - `a <path to asm>`");
            Console.WriteLine("    case \"h\":  write hex to RAM");
            Console.WriteLine("    - `h <path to hex>`");
            Console.WriteLine("    case \"b\":  write binary text to RAM");
            Console.WriteLine("    - `b <path to binary text file>`");
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
