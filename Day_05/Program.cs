using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Day_05
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllText(@"../../../input.txt").Trim();
            //input = "dabAcCaCBAcCcaDA";

            var length = ReduceAllPairsAndMeasure(input);
            Console.WriteLine($"Part 1 answer: {length}");

            var distinctUnits = input.ToLowerInvariant().Distinct().ToArray();
            var lengthWithoutUnit = new Dictionary<char, int>();

            foreach (var unit in distinctUnits)
            {
                var filtered = input.Where(c => !IsSameUnit(c, unit));
                lengthWithoutUnit[unit] = ReduceAllPairsAndMeasure(filtered, input.Length);
            }

            var victimUnit = lengthWithoutUnit.MinBy(kvp => kvp.Value).First();
            Console.WriteLine($"Part 2: Remove {victimUnit.Key} to reduce to {victimUnit.Value}");

            Console.ReadLine();
        }

        private static bool IsSameUnit(char a, char b)
        {
            return a.ToString().Equals(b.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        private static int ReduceAllPairsAndMeasure(IEnumerable<char> input, int maxLength = 10000)
        {
            var output = new Stack<char>(maxLength/2);
            foreach (var letter in input)
            {
                if (output.TryPeek(out var prev) && DoReact(prev, letter))
                    output.Pop();
                else
                    output.Push(letter);
            }
            return output.Count;
        }

        private static bool DoReact(char a, char b)
        {
            return a != b && IsSameUnit(a, b);
        }
    }
}
