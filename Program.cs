using System;
using MIPS_simulator.VirtualMachine;

namespace MIPS_simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input binary code here");
            Console.Write("> ");

            string binary = Console.ReadLine();
            Machine vm = new Machine(binary);
            vm.Continue();
        }
    }
}
