using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_10
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var lights = input.Select(Light.FromLine).ToList();

			var minSize = new Size(44000, 44000);

			for (var i = 0; i < 100; i++)
			{
				foreach (var light in lights)
					light.Move();

				var width = (long)lights.Max(l => l.Pos.X) - lights.Min(l => l.Pos.X) + 1;
				var height = (long)lights.Max(l => l.Pos.Y) - lights.Min(l => l.Pos.Y) + 1;
				if (width * height < minSize.Width * minSize.Height)
				{
					Debug.Assert(width < int.MaxValue);
					minSize.Width = (int)width;
					minSize.Height = (int)height;
				}
				else
				{
					break;
				}
			}

			// Move back one sec
			foreach (var light in lights)
				light.Move(-1);

			var minX = lights.Min(l => l.Pos.X);
			var minY = lights.Min(l => l.Pos.Y);

			for (var y = minY; y < minSize.Height; y++)
			{
				var line = new char[minSize.Width];
				for (var x = minX; x < minSize.Width; x++)
				{
					if (lights.Any(l => l.IsAt(x, y)))
						line[x-minX] = '#';
					else
						line[x - minX] = '.';
				}

				Console.WriteLine(line);
			}

			//Console.WriteLine(game.ToString());
			Console.ReadLine();
		}
	}

	internal class Light
	{
		private static readonly Regex regex = new Regex(@"position=<\s*([\-0-9]+),\s*([\-0-9]+)> velocity=<\s*([\-0-9]+),\s*([\-0-9]+)>");

		public Point Pos { get; private set; }
		public Size Velocity { get; private set; }

		public static Light FromLine(string s)
		{
			var groups = regex.Match(s).Groups;
			var numbers = groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
			Debug.Assert(groups.Count == 5);
			return new Light
			{
				Pos = new Point(numbers[0], numbers[1]),
				Velocity = new Size(numbers[2], numbers[3])
			};
		}

		public void Move(int seconds = 1)
		{
			Pos += Velocity * seconds;
		}

		public bool IsAt(int x, int y)
		{
			return Pos.X == x && Pos.Y == y;
		}
	}
}