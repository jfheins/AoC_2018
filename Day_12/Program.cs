using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;

namespace Day_12
{
    internal class Program
    {
        private static char[] state;
        private static int offset;
        private static Dictionary<string, char> _transitions;

        private static void Main(string[] args)
        {
            var input =
                @"###....#..#..#......####.#..##..#..###......##.##..#...#.##.###.##.###.....#.###..#.#.##.#..#.#";
            var transistionInput = File.ReadAllLines(@"../../../transitions.txt");
            _transitions = new Dictionary<string, char>(transistionInput.Select(ParseLine));
            var generations = 20;
            
            var sw = new Stopwatch();
            sw.Start();


            offset = 6 + 2 * generations;
            var max = input.Length + offset;
            state = new char[offset + max + 1];

            for (var i = 0; i < state.Length; i++)
            {
                state[i] = '.';
            }

            var plantIndicies = new HashSet<int>(input.IndexWhere(c => c == '#'));

            foreach (var plantIndex in plantIndicies)
            {
                state[PlantIndexToArrayIndex(plantIndex)] = '#';
            }

            Console.Write("Initial:             ");
            Console.WriteLine(string.Concat(state));

            var nextState = state.ToArray();
            for (int gen = 1; gen <= generations; gen++)
            {

                for (int i = 2; i < state.Length-2; i++)
                {
                    nextState[i] = GetNextState(GetChunk(i));
                }

                state = nextState.ToArray();

                Console.Write($"After generation {gen}:  ");
                Console.WriteLine(string.Concat(state));
            }

            var part1 = state.IndexWhere(c => c == '#').Select(ArrayIndexToPlantIndex).Sum();
            Console.WriteLine($"Part1 solution: {part1}"); // 3742 too high

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static char GetNextState(string s)
        {
            return _transitions.TryGetValue(s, out var result) ? result : '.';
        }

        private static KeyValuePair<string, char> ParseLine(string arg)
        {
            return new KeyValuePair<string, char>(arg.Substring(0, 5), arg.Trim().Last());
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
    }
}
