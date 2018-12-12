using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using MoreLinq.Extensions;

namespace Day_12
{
    internal class Program
    {
        private static char[] state;
        private static int offset;

        static HashSet<int> plantIndicies;
        static Dictionary<int, char> transitions;

        private static void Main(string[] args)
        {
            var input =
                @"###....#..#..#......####.#..##..#..###......##.##..#...#.##.###.##.###.....#.###..#.#.##.#..#.#";
            var transitionInput = File.ReadAllLines(@"../../../transitions.txt");

            transitions = new Dictionary<int, char>(transitionInput.Select(ParseLine));

            long generations = 50000000000;
            
            var sw = new Stopwatch();
            sw.Start();

            plantIndicies = new HashSet<int>(input.IndexWhere(c => c == '#'));
            var generationSums = new Dictionary<long, long>();

            for (long gen = 1; gen <= generations; gen++)
            {
                if (gen % 1 == 0)
                {
                    Console.WriteLine($"Generation {gen}");
                }

                var lastDiffs = TakeLastExtension.TakeLast(generationSums, 4).PairwiseWithOverlap().Select(x => x.Item2.Value - x.Item1.Value).ToArray();
                if (lastDiffs.Length == 3 && lastDiffs.All(x => x== lastDiffs.First()))
                {
                    // No change any more
                    var diff = lastDiffs.First();
                    var lastSum = generationSums[gen - 1];
                    generationSums.Add(gen, lastSum + diff);
                    var remaining = generations - gen + 1;
                    generationSums.Add(generations, lastSum + remaining * diff);
                    break;
                }

                var newIdx = plantIndicies
                    .SelectMany(NextPossibleIndicies)
                    .Select(GetNextTransition)
                    .Where(i => i.HasValue)
                    .Select(i => i.Value);
                plantIndicies = new HashSet<int>(newIdx);
                generationSums.Add(gen, plantIndicies.Sum());
            }

            var part1 = plantIndicies.Sum();
            var part2 = generationSums[generations];
            Console.WriteLine($"Part1 solution: {part1}");
            Console.WriteLine($"Part2 solution: {part2}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static IEnumerable<int> NextPossibleIndicies(int plantIndex)
        {
            return Enumerable.Range(plantIndex - 1, 3);
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

        private static int? GetNextTransition(int plantIndex)
        {
            var temp_1 = plantIndicies.Contains(plantIndex - 2) ? 1 : 0;
            var temp_2 = plantIndicies.Contains(plantIndex - 1) ? 2 : 0;
            var temp_3 = plantIndicies.Contains(plantIndex + 0) ? 4 : 0;
            var temp_4 = plantIndicies.Contains(plantIndex + 1) ? 8 : 0;
            var temp_5 = plantIndicies.Contains(plantIndex + 2) ? 16 : 0;
            var key = temp_1 + temp_2 + temp_3 + temp_4 + temp_5;

            return transitions[key] == '#' ? (int?)plantIndex : null;
        }
    }
}
