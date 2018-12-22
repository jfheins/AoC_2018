using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Core;

namespace Day_22
{
	public class Program
	{
		private static ulong[,] erosionLevel;

		private static void Main(string[] args)
		{
			ulong depth = 5913;
			var target = new Point(8, 701);
			var sw = new Stopwatch();
			sw.Start();

			var padding = 100;

			erosionLevel = new ulong[target.X + padding, target.Y + padding];
			var origin = new Point(0, 0);

			const ulong xFactor = 16807;
			const ulong yFactor = 48271;

			for (var x = 0; x < target.X + padding; x++)
			{
				for (var y = 0; y < target.Y + padding; y++)
				{
					var p = new Point(x, y);
					ulong geoIndex = 0;

					if (p == origin || p == target)
					{
						geoIndex = 0;
					}
					else if (y == 0)
					{
						geoIndex = (ulong) x * xFactor;
					}
					else if (x == 0)
					{
						geoIndex = (ulong) y * yFactor;
					}
					else
					{
						geoIndex = erosionLevel[x - 1, y] * erosionLevel[x, y - 1];
					}

					erosionLevel[x, y] = (geoIndex + depth) % 20183;
				}
			}

			var risk = 0;
			for (var x = 0; x <= target.X; x++)
			{
				for (var y = 0; y <= target.Y; y++)
					risk += (int) (erosionLevel[x, y] % 3);
			}

			Console.WriteLine($"Part 1: {risk}");

			var search = new DijkstraSearch<ValueTuple<Point, Tool>, Direction>(EqualityComparer<(Point, Tool)>.Default, Expander);







			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static IEnumerable<((Point, Tool) node, float cost)> Expander((Point pos, Tool tool) state)
		{
			var newPosition = new List<Point>();

			if (state.pos.X > 0)
			{
				newPosition.Add(state.pos + _mapDirectionToSize[Direction.Left]);
			}
			if (state.pos.Y > 0)
			{
				newPosition.Add(state.pos + _mapDirectionToSize[Direction.Up]);
			}

			newPosition.Add(state.pos + _mapDirectionToSize[Direction.Right]);
			newPosition.Add(state.pos + _mapDirectionToSize[Direction.Down]);

			foreach (var point in newPosition)
			{
				
			}
		}

		private static bool IsToolValidAt(Tool t, Point p)
		{
			return erosionLevel % 3 == (int) t;
		}

		private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
		{
			{Direction.Left, new Size(-1, 0)},
			{Direction.Up, new Size(0, -1)},
			{Direction.Right, new Size(1, 0)},
			{Direction.Down, new Size(0, 1)}
		};
	}

	public enum Tool
	{
		None = 0,
		Torch = 1,
		Gear = 2
	}

	public enum Direction
	{
		Left,
		Up,
		Right,
		Down
	}
}