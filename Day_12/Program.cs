using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_12
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText(@"../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }
    }
}
