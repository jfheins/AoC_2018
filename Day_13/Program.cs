using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;

namespace Day_13
{
	internal class Program
	{
		public enum Direction
		{
			Left,
			Up,
			Right,
			Down
		}

		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var tracks = input.Select(ParseLineToTrack).ToArray();
			var carts = SortCarts(input.SelectMany(ParseCarts);

			// initial
			Console.WriteLine($"{carts.Length} carts found on map.");

			while (carts.Length > 1)
			{
				// carts are sorted
				foreach (var cart in carts.Where(c => !c.IsBroken))
				{
					cart.Move();
					foreach (var other in carts.Except(cart.ToEnumerable())
						.Where(other => cart.Position == other.Position))
					{
						cart.IsBroken = true;
						other.IsBroken = true;
						Console.WriteLine($"Crash at {cart.Position}");
					}
				}

				carts = carts.Where(c => !c.IsBroken).ToArray();

				// Move around corners
				foreach (var cart in carts)
				{
					var trackSymbol = tracks[cart.Position.Y][cart.Position.X];
					cart.Turn(trackSymbol);
				}
			}

			Console.WriteLine("Only one cart left!");
			Console.WriteLine(carts[0].Position);

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static Cart[] SortCarts(IEnumerable<Cart> carts)
		{
			return carts.OrderBy(c => c.Position.X)
				.ThenBy(c => c.Position.Y)
				.ToArray();
		}

		private static IEnumerable<Cart> ParseCarts(string line, int y)
		{
			for (var i = 0; i < line.Length; i++)
			{
				if ("<^>v".Contains(line[i]))
				{
					yield return new Cart(i, y, line[i]);
				}
			}
		}

		private static string ParseLineToTrack(string arg)
		{
			return arg.Replace('<', '-')
				.Replace('^', '|')
				.Replace('>', '-')
				.Replace('v', '|');
		}

		public class Cart
		{
			public Cart(int x, int y, char v)
			{
				Position = new Point(x, y);
				Symbol = v;
				TurnCounter = -1;
			}

			public int TurnCounter { get; set; }
			public Point Position { get; set; }
			public char Symbol { get; set; }
			public bool IsBroken { get; set; }

			public void Move()
			{
				switch (Symbol)
				{
					case '<':
						Position = new Point(Position.X - 1, Position.Y);
						return;
					case '^':
						Position = new Point(Position.X, Position.Y - 1);
						return;
					case '>':
						Position = new Point(Position.X + 1, Position.Y);
						return;
					case 'v':
						Position = new Point(Position.X, Position.Y + 1);
						return;
				}
			}


			private void TurnLeft()
			{
				switch (Symbol)
				{
					case '<':
						Symbol = 'v';
						return;
					case '^':
						Symbol = '<';
						return;
					case '>':
						Symbol = '^';
						return;
					case 'v':
						Symbol = '>';
						return;
				}
			}

			private void TurnRight()
			{
				switch (Symbol)
				{
					case '<':
						Symbol = '^';
						return;
					case '^':
						Symbol = '>';
						return;
					case '>':
						Symbol = 'v';
						return;
					case 'v':
						Symbol = '<';
						return;
				}
			}

			public void Turn(char trackSymbol)
			{
				if (trackSymbol == '/')
				{
					switch (Symbol)
					{
						case '<':
							Symbol = 'v';
							return;
						case '^':
							Symbol = '>';
							return;
						case '>':
							Symbol = '^';
							return;
						case 'v':
							Symbol = '<';
							return;
					}
				}

				if (trackSymbol == '\\')
				{
					switch (Symbol)
					{
						case '<':
							Symbol = '^';
							return;
						case '^':
							Symbol = '<';
							return;
						case '>':
							Symbol = 'v';
							return;
						case 'v':
							Symbol = '>';
							return;
					}
				}

				if (trackSymbol == '+')
				{
					switch (TurnCounter)
					{
						case -1:
							TurnLeft();
							break;
						case 0:
							break;
						case 1:
							TurnRight();
							break;
					}

					TurnCounter++;
					if (TurnCounter > 1)
					{
						TurnCounter = -1;
					}
				}
			}
		}
	}
}