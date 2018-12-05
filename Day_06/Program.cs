using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_06
{
    class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllText(@"../../../input.txt").Trim();

            var sw = new Stopwatch();
            sw.Start();
            

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }
    }
}
