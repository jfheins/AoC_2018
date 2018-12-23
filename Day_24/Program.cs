using System;
using System.Diagnostics;
using System.IO;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");
            var sw = new Stopwatch();
            sw.Start();



            Console.WriteLine($"Part 1: Point");
            Console.WriteLine($"Part 2: x");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }
    }
}
