using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_17
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var coords = input.Select(ParseLine)


			Console.WriteLine("Part 1: ");
			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static IEnumerable<Point> ParseLine(string arg)
		{
			var xStr = Regex.Match(arg, @"x=([-0-9]+)(?:(?:\.\.)([-0-9]+))?").Groups;
			var minX = int.Parse(xStr[1].Value);
			var maxX = int.Parse(xStr.Count > 2 ? xStr[2].Value : xStr[1].Value);

			var yStr = Regex.Match(arg, @"y=([-0-9]+)(?:(?:\.\.)([-0-9]+))?").Groups;
			var minY = int.Parse(yStr[1].Value);
			var maxY = int.Parse(yStr.Count > 2 ? yStr[2].Value : yStr[1].Value);

			for (var x = minX; x <= maxX; x++)
			{
				for (var y = minY; y <= maxY; y++)
				{
					yield return new Point(x, y);
				}
			}
		}
	}
}