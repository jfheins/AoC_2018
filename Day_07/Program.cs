using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_07
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");

            

            var sw = new Stopwatch();
            sw.Start();


            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }
    }
}
