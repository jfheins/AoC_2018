using Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day_10
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var lights = input.Select(Light.FromLine).ToList();

			var minimalArea = long.MaxValue;
            var minRect = Rectangle.Empty;
		    var resultTime = -1;

			for (var i = 0; i < 1000000; i++)
			{
				foreach (var light in lights)
					light.Move();

                var currentRect = new Rectangle(
                    lights.Min(l => l.Pos.X),
                    lights.Min(l => l.Pos.Y),
                    lights.Max(l => l.Pos.X) + 1,
                    lights.Max(l => l.Pos.Y) + 1
                    );

			    long currentArea = (long)currentRect.Width * (long)currentRect.Height;

				if (currentArea < minimalArea)
				{
				    minRect = currentRect;
				    minimalArea = currentArea;
				}
				else
				{
				    resultTime = i;

                    break;
				}
			}

			// Move back one sec
			foreach (var light in lights)
				light.Move(-1);

			var minX = minRect.Left;
			var minY = minRect.Top;

		    var result = new StringBuilder();

			for (var y = minY; y < minRect.Bottom; y++)
			{
				var line = new char[minRect.Width];
				for (var x = minX; x < minRect.Right; x++)
				{
				    var idx = x - minX;
					if (lights.Any(l => l.IsAt(x, y)))
						line[idx] = '#';
					else
						line[idx] = '.';
				}

			    result.AppendLine(string.Concat(line));
			}

            File.WriteAllText(@"../../../output.txt", result.ToString());
		    Console.WriteLine($"Finish after {resultTime} secs :-)");
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
			var numbers = s.ParseInts();
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