using System;
using System.Diagnostics;
using System.Drawing;

namespace Day_22
{
	public class Program
	{
		private static ulong[,] erosionLevel;

		static void Main(string[] args)
		{
			ulong depth = 5913;
			var target = new Point(8, 701);
			var sw = new Stopwatch();
			sw.Start();

			erosionLevel = new ulong[target.X+1, target.Y+1];
			var origin = new Point(0, 0);

			const ulong xFactor = 16807;
			const ulong yFactor = 48271;

			for (int x = 0; x <= target.X; x++)
			{
				for (int y = 0; y <= target.Y; y++)
				{
					var p = new Point(x, y);
					ulong geoIndex = 0;

					if (p == origin || p == target)
					{
						geoIndex = 0;
					}
					else if (y == 0)
					{
						geoIndex = (ulong)x * xFactor;
					}
					else if (x == 0)
					{
						geoIndex = (ulong)y * yFactor;
					}
					else
					{
						geoIndex = erosionLevel[x - 1, y] * erosionLevel[x, y - 1];
					}
					erosionLevel[x, y] = (geoIndex + depth) % 20183;
				}
			}

			var risk = 0;
			for (int x = 0; x <= target.X; x++)
			{
				for (int y = 0; y <= target.Y; y++)
				{
					risk += (int)(erosionLevel[x, y] % 3);
				}
			}

			Console.WriteLine($"Part 1: {risk}");
			Console.WriteLine($"Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}
