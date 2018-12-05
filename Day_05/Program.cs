using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_05
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllText(@"../../../input.txt").Trim();
            input = "dabAcCaCBAcCcaDA";

            var result = ReduceAllPairs(input);
            Console.WriteLine(result.Length);

            Console.ReadLine();
        }

        private static string ReduceAllPairs(string input)
        {
            var output = new Stack<char>(input.Length);
            foreach (var letter in input)
            {
                if (output.TryPeek(out var prev) && DoReact(prev, letter))
                    output.Pop();
                else
                    output.Push(letter);
            }
            return string.Concat(output.Reverse());
        }

        private static bool DoReact(char a, char b)
        {
            return a != b && a.ToString().Equals(b.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
