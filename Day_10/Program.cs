using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Day_10
{
	class Program
	{
		static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");

			for (int i = 0; i < rounds; i++)
			{
				game.PlaceNextMarble(i % players);
			}

			//Console.WriteLine(game.ToString());
			Console.ReadLine();
		}
	}

	class Light
	{
		public Point Pos { get; private set; }
		public Size Velocity { get; private set; }

		public Light()
		{
			
		}

		private static Regex regex = new Regex(@"position=<\s*(\d+),\s*(\d+)> velocity=<\s*(\d+),\s*(\d+)>");

		public static Light FromLine(string s)
		{
			var groups = regex.Match(s).Groups;
			var numbers = groups.Cast<string>().Skip(1).Select(int.Parse).ToArray();
			Debug.Assert(groups.Count == 5);
			return new Light
			{
				Pos = new Point(numbers[0], numbers[1]),
				Velocity = new Size(numbers[2], numbers[3])
			};
		}
	}
}

