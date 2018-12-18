using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;

namespace Day_18
{
	public enum Direction
	{
		Left,
		Up,
		Right,
		Down
	}

	public class Program
	{
		private static char[][] _arr;
		private static Rectangle Bounds;

		private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
		{
			{Direction.Left, new Size(-1, 0)},
			{Direction.Up, new Size(0, -1)},
			{Direction.Right, new Size(1, 0)},
			{Direction.Down, new Size(0, 1)}
		};

		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var generations = 1000000000;

			_arr = input.Select(l => l.ToCharArray()).ToArray();
			Bounds = new Rectangle(0, 0, _arr[0].Length, _arr.Length);
			var generationSums = new Dictionary<long, long>();

			for (var i = 1; i <= generations; i++)
			{
				Console.WriteLine($"Round {i}");

				if (i == 4200)
				{
					// Period of 28
					foreach (var pair in generationSums.TakeLast(100))
					{
						Console.WriteLine($"{pair.Key}; {pair.Value}");
					}
					break;
				}

				_arr = _arr.Select((line, y) => line.Select((symbol, x) => GetNextContent(x, y, symbol))
						.ToArray())
					.ToArray();

			var value = _arr.SelectMany(l => l).Count(c => c == '#') * _arr.SelectMany(l => l).Count(c => c == '|');
				generationSums.Add(i, value);

			}


			Console.WriteLine($"Part 1: {generationSums[10]}");
			Console.WriteLine($"Part 2: {generationSums[4108]}");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static char GetNextContent(int x, int y, char symbol)
		{
			var localEnvironment = GetAdjacentPoints(new Point(x, y)).Where(InBounds).Select(CharAt).ToList();

			switch (symbol)
			{
				case '.': // Open
					return localEnvironment.Count(c => c == '|') >= 3 ? '|' : '.';

				case '|': // Trees
					return localEnvironment.Count(c => c == '#') >= 3 ? '#' : '|';

				case '#': // Lumberyard
					if (localEnvironment.Count(c => c == '#') > 0 && localEnvironment.Count(c => c == '|') > 0)
					{
						return '#';
					}

					return '.';
				default:
					return '?';
			}
		}

		private static char CharAt(Point p)
		{
			return _arr[p.Y][p.X];
		}

		private static char CharAt(int x, int y)
		{
			return _arr[y][x];
		}

		private static bool InBounds(Point p)
		{
			return Bounds.Contains(p);
		}

		private static IEnumerable<Point> GetAdjacentPoints(Point p)
		{
			yield return p + new Size(-1, 0);
			yield return p + new Size(-1, -1);
			yield return p + new Size(0, -1);
			yield return p + new Size(1, -1);
			yield return p + new Size(1, 0);
			yield return p + new Size(1, 1);
			yield return p + new Size(0, 1);
			yield return p + new Size(-1, 1);
		}
	}
}