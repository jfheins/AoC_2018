using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using MoreLinq;

namespace Day_12
{
    internal class Program
    {
        private static char[] state;
        private static int offset;

        static HashSet<int> plantIndicies;

        private static void Main(string[] args)
        {
            var input =
                @"###....#..#..#......####.#..##..#..###......##.##..#...#.##.###.##.###.....#.###..#.#.##.#..#.#";
            var transitionInput = File.ReadAllLines(@"../../../transitions.txt");

            var transitions = new Dictionary<int, char>(transitionInput.Select(ParseLine));

            long generations = 20;
            
            var sw = new Stopwatch();
            sw.Start();

            plantIndicies = new HashSet<int>(input.IndexWhere(c => c == '#'));

            for (long gen = 1; gen <= generations; gen++)
            {
                var newIdx = plantIndicies.SelectMany(NextPossibleIndicies).Select(GetChunk).Select(transitions[]);
            }

            for (int gen = 1; gen <= generations; gen++)
            {

                for (int i = 2; i < state.Length-2; i++)
                {
                    nextState[i] = transitions[GetChunk(i)];
                }

                state = nextState.ToArray();

                Console.Write($"After generation {gen}:  ");
                Console.WriteLine(string.Concat(state));
            }

            var part1 = state.IndexWhere(c => c == '#').Select(ArrayIndexToPlantIndex).Sum();
            Console.WriteLine($"Part1 solution: {part1}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static IEnumerable<int> NextPossibleIndicies(int plantIndex)
        {
            return Enumerable.Range(plantIndex - 2, 5);
        }

        private static KeyValuePair<int, char> ParseLine(string arg)
        {
            var keyStr = arg.Substring(0, 5);
            var key = keyStr.EquiZip(new[] {1, 2, 4, 8, 16}, (c, pow) => (c == '#') ? pow : 0).Sum();
            return new KeyValuePair<int, char>(key, arg.Trim().Last());
        }

        private static int PlantIndexToArrayIndex(int plantIndex)
        {
            return plantIndex + offset;
        }

        private static int ArrayIndexToPlantIndex(int arrIndex)
        {
            return arrIndex - offset;
        }

        private static string GetChunk(int arrIndex)
        {
            //return state.AsSpan(currentPlant - 2, 5);
            return new string(state.AsSpan(arrIndex - 2, 5));
        }

        private static char GetNextTransistion(int plantIndex)
        {
            
        }
    }
}
