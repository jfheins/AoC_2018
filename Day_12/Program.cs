using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Day_12
{
    class Program
	{
		private static char[] state;
		static int offset;

        static void Main(string[] args)
        {
            var input = @"###....#..#..#......####.#..##..#..###......##.##..#...#.##.###.##.###.....#.###..#.#.##.#..#.#";
			var transistions = File.ReadAllText(@"../../../transitions.txt");
			var generations = 20;

			offset = 6 + 2 * generations;
			var max = input.Length + offset;
			state = new char[offset + max + 1];




			var sw = new Stopwatch();
            sw.Start();


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

		private static Span<char> GetChunk(int plantIndex)
		{
			var currentPlant = plantIndex + offset;
			return state.AsSpan(currentPlant - 2, 5);
		}
    }
}
