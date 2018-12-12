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

        private static HashSet<int> _plantIndicies;
        private static Dictionary<string, char> _transitions;

        private static void Main(string[] args)
        {
            var input =
                @"###....#..#..#......####.#..##..#..###......##.##..#...#.##.###.##.###.....#.###..#.#.##.#..#.#";
            var transitionInput = File.ReadAllLines(@"../../../transitions.txt");
            _transitions = new Dictionary<string, char>(transitionInput.Select(ParseLine));

            var generations = 50000000000;

            var sw = new Stopwatch();
            sw.Start();

            _plantIndicies = new HashSet<int>(input.IndexWhere(c => c == '#'));
            var generationSums = new Dictionary<long, long>();

            for (long gen = 1; gen <= generations; gen++)
            {
                Console.WriteLine($"Generation {gen}");

                var lastDiffs = generationSums
                    .TakeLast(4)
                    .Select(kvp => kvp.Value)
                    .PairwiseWithOverlap()
                    .Select(x => x.Diff())
                    .ToArray();
                if (lastDiffs.Length == 3 && lastDiffs.All(x => x == lastDiffs[0]))
                {
                    // No change any more
                    var diff = lastDiffs.First();
                    var lastSum = generationSums[gen - 1];
                    var remaining = generations - gen + 1;
                    generationSums.Add(generations, lastSum + remaining * diff);
                    break;
                }

                var newIdx = _plantIndicies
                    .SelectMany(NextPossibleIndicies)
                    .Select(GetNextTransition)
                    .Where(i => i.HasValue)
                    .Select(i => i.Value);
                _plantIndicies = new HashSet<int>(newIdx);
                generationSums.Add(gen, _plantIndicies.Sum());
            }

            Console.WriteLine($"Part1 solution: {generationSums[20]}");
            Console.WriteLine($"Part2 solution: {generationSums[generations]}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static IEnumerable<int> NextPossibleIndicies(int plantIndex)
        {
            return Enumerable.Range(plantIndex - 2, 5);
        }

        private static KeyValuePair<string, char> ParseLine(string arg)
        {
            var keyStr = arg.Substring(0, 5);
            return new KeyValuePair<string, char>(keyStr, arg.Trim().Last());
        }

        private static int? GetNextTransition(int plantIndex)
        {
            var chunk = Enumerable.Range(-2, 5)
                .Select(i => _plantIndicies.Contains(plantIndex + i))
                .Select(isPlant => isPlant ? '#' : '.');
            var key = string.Concat(chunk);

            return _transitions[key] == '#' ? (int?) plantIndex : null;
        }
    }
}
