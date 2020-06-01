using System;
using System.IO;
using MIPS.Interpreter.Interpreter;

namespace MIPS.Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            MipsToBinary mips = new MipsToBinary();

            InterpretFile(mips, args);

            // instruction
            Console.WriteLine("Usage:");
            Console.WriteLine("    ./MIPS.Interpreter.exe -s <file of MIPS code> -o <path to save binary> [-n]");
            Console.WriteLine("    [-n] := (optional) enable new line for each instruction.");
            Console.WriteLine("    [-h] := (optional) hex.");
            Console.WriteLine("    [-p] := (optional) (hex only) add space partion for each byte.");

            bool isInteractiveMode = false;

            while (!isInteractiveMode)
            {
                // read file or interactive
                Console.WriteLine("[R]Read instructions from file or [I]Interactively interpret instruction?");
                Console.Write("> ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "r":
                    case "R":
                        Console.Write("> ");
                        string input = Console.ReadLine();
                        InterpretFile(mips, input.Split());
                        break;
                    case "I":
                    case "i":
                        isInteractiveMode = true;
                        break;
                }
            }

            // interactive mode
            Console.WriteLine("Please input a complete instruction for each line.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                string binary = mips.GetBinaryString(input, true);
                Console.WriteLine(binary);
            }

            // string input = "";
            // input += "# this is comment\n";
            // input += "add $t3, $t1, $zero\n";
            // input += "sw $t1, 0x86($t2)\n";
            // input += "           sw $t1          , 0x86($t2)\n";
            // input += "j label\n";
            // input += "bne $t3, $t1, label\n";
            // input += "move $t3, $t1\n";
            // input += "bgt $t3, $t1, label \n";
            // input += "addi $s3, $a1, -12 \n";
        }

        private static void InterpretFile(MipsToBinary mips, string[] args)
        {
            bool isWithNewLines = false;
            bool isHex = false;
            bool isBytePartition = false;

            // file operation mode
            if (args.Length > 1)
            {
                string mipsFile = "";
                string binPath = "";
                int i;
                for (i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-s" && i + 1 < args.Length)
                    {
                        mipsFile = args[i + 1];
                    }
                    else if (args[i] == "-o" && i + 1 < args.Length)
                    {
                        binPath = args[i + 1];
                    }
                    else if (args[i] == "-n")
                    {
                        isWithNewLines = true;
                    }
                    else if (args[i] == "-h")
                    {
                        isHex = true;
                    }
                    else if (args[i] == "-p")
                    {
                        isBytePartition = true;
                    }
                }
                if (mipsFile == "" || binPath == "")
                {
                    Console.WriteLine("Input error!");
                    return;
                }
                string input = File.ReadAllText(mipsFile);
                string binary;
                if (!isHex)
                    binary = mips.GetBinaryString(input, isWithNewLines);
                else
                    binary = mips.GetHexString(input, isWithNewLines, isBytePartition);
                File.WriteAllText(binPath, binary);
                Console.WriteLine("Done");
                return;
            }
        }
    }
}
