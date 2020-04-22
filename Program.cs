using System;
using System.IO;
using MIPS_interpreter.Interpreter;

namespace MIPS_interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            MipsToBinary mips = new MipsToBinary();

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
                }
                if (mipsFile == "" || binPath == "")
                {
                    Console.WriteLine("Input error!");
                    return;
                }
                string input = File.ReadAllText(mipsFile);
                string binary = mips.MipsToBinaryMethod(input, false);
                File.WriteAllText(binPath, binary);
                Console.WriteLine("Done");
                return;
            }

            // interactive mode
            Console.WriteLine("Usage:");
            Console.WriteLine("    ./MIPS_interpreter.exe -s [file of MIPS code] -o [path to save binary]");
            Console.WriteLine("Please input a complete instruction for each line.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                string binary = mips.MipsToBinaryMethod(input, true);
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
    }
}
