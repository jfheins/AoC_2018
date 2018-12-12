using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace Day_11
{
	internal class Program
	{
		private static int _serialNumber;
		private static Dictionary<(int, int, int), int> _rectPowers = new Dictionary<(int, int, int), int>();


		public static void Main(string[] args)
		{
			_serialNumber = 7315;
			var gridsize = 300;

			var sw = new Stopwatch();
			sw.Start();

			var squareSizes = Enumerable.Range(1, 50);
			foreach (var sqsize in squareSizes)
			{
				Console.WriteLine($"Squaresize: {sqsize}");

				var allRects = GetAllSquares(gridsize, sqsize);
				Console.WriteLine($"{allRects.Count} squares");

				if (sqsize % 7 == 0)
				{
					foreach (var rect in allRects)
					{
						var parts = Divide(rect, 7);
						_rectPowers[(rect.X, rect.Y, sqsize)] = parts.Select(lookup).Sum();
					}
				}
				else if (sqsize % 5 == 0)
				{
					foreach (var rect in allRects)
					{
						var parts = Divide(rect, 5);
						_rectPowers[(rect.X, rect.Y, sqsize)] = parts.Select(lookup).Sum();
					}
				}
				else if (sqsize % 3 == 0)
				{
					foreach (var rect in allRects)
					{
						var parts = Divide(rect, 3);
						_rectPowers[(rect.X, rect.Y, sqsize)] = parts.Select(lookup).Sum();
					}
				}
				else if (sqsize % 2 == 0)
				{
					foreach (var rect in allRects)
					{
						var parts = Divide(rect, 2);
						_rectPowers[(rect.X, rect.Y, sqsize)] = parts.Select(lookup).Sum();
					}
				}
				else
				{
					foreach (var rect in allRects)
					{
						_rectPowers[(rect.X, rect.Y, sqsize)] = CalcPowerLevelForRect(rect);
					}
				}
			}

			var max = _rectPowers.MaxBy(kvp => kvp.Value).First();

			Console.WriteLine(max.Key);

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

			Console.ReadLine();
		}

		private static int lookup(Rectangle r)
		{
			return _rectPowers[(r.X, r.Y, r.Width)];
		}

		private static IEnumerable<Rectangle> GetQuarters(Rectangle rect)
		{
			var quarterSize = new Size(rect.Width / 2, rect.Height / 2);
			yield return new Rectangle(rect.Location, quarterSize);
			yield return new Rectangle(Point.Add(rect.Location, new Size(rect.Width / 2, 0)), quarterSize);
			yield return new Rectangle(Point.Add(rect.Location, new Size(0, rect.Width / 2)), quarterSize);
			yield return new Rectangle(Point.Add(rect.Location, quarterSize), quarterSize);
		}

		private static IEnumerable<Rectangle> Divide(Rectangle rect, int divisor)
		{
			var partSize = new Size(rect.Width / divisor, rect.Height / divisor);

			var xSize = new Size(rect.Width / divisor, 0);
			var ySize = new Size(0, rect.Height / divisor);

			var xRange = Enumerable.Range(0, divisor);
			var yRange = Enumerable.Range(0, divisor);

			return xRange.Cartesian(yRange, (x, y) => new Rectangle(rect.Location + xSize * x + ySize * y, partSize));
		}

		private static List<Rectangle> GetAllSquares(int gridSize, int squareSize)
		{
			var xRange = Enumerable.Range(1, gridSize - squareSize + 1);
			var yRange = Enumerable.Range(1, gridSize - squareSize + 1);

			return xRange.Cartesian(yRange, (x, y) => new Rectangle(x, y, squareSize, squareSize)).ToList();
		}

		private static int CalcPowerLevelForRect(Rectangle rect)
		{
			var sum = 0;
			for (var x = rect.Left; x < rect.Right; x++)
			{
				for (var y = rect.Top; y < rect.Bottom; y++)
				{
					sum += GetPowerLevelForCell(x, y);
				}
			}

			return sum;
		}

		private static int CalcPowerLevelForRect(int x, int y, int size)
		{
			var power = 0;
			for (var i = 1; i <= size; i++)
			{
				for (var j = 1; j <= size; j++)
				{
					power += GetPowerLevelForCell(x + i, y + j);
				}
			}

			return power;
		}

		private static int GetPowerLevelForCell(int x, int y)
		{
			var rackid = x + 10;
			var power = rackid * y;
			power += _serialNumber;
			power *= rackid;
			power = power % 1000 / 100;
			return power - 5;
		}
	}
}