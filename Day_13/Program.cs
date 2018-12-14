using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day_13
{
	internal class Program
	{
		private static void Main()
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var sim = new TrackSimulator(input);

			// initial
			Console.WriteLine($"{sim.Carts.Count} carts found on map.");

			while (sim.Carts.Count > 1)
			{
				sim.Step();
			}

			Console.WriteLine($"First crash was at {sim.crashLocations.First().location}");
			if (sim.Carts.Any())
			{
				Console.WriteLine($"Last cart is at {sim.Carts.First().Key}");
			}

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}

	public class TrackSimulator
	{
		public enum Direction
		{
			Left,
			Up,
			Right,
			Down
		}

		private readonly string[] _tracks;
		public readonly Dictionary<Point, Cart> Carts;
		private int tick;

		public TrackSimulator(string[] input)
		{
			_tracks = input.Select(ParseLineToTrack).ToArray();
			Carts = ParseCarts(input).ToDictionary(cart => cart.Position);
		}

		public ICollection<(int tick, Point location)> crashLocations { get; } = new List<(int, Point)>();


		private static IEnumerable<Cart> ParseCarts(string[] input)
		{
			for (var y = 0; y < input.Length; y++)
			{
				for (var x = 0; x < input[y].Length; x++)
				{
					if ("<^>v".Contains(input[y][x]))
					{
						yield return new Cart(x, y, input[y][x]);
					}
				}
			}
		}

		private static Cart[] CopyAndSortCarts(IEnumerable<Cart> carts)
		{
			return carts.OrderBy(c => c.Position.X)
				.ThenBy(c => c.Position.Y)
				.ToArray();
		}

		private static string ParseLineToTrack(string arg)
		{
			return arg.Replace('<', '-')
				.Replace('^', '|')
				.Replace('>', '-')
				.Replace('v', '|');
		}

		private char TrackSymbolAt(Point p)
		{
			return _tracks[p.Y][p.X];
		}

		public void Step()
		{
			tick++;
			foreach (var cart in CopyAndSortCarts(Carts.Values))
			{
				if (Carts.Remove(cart.Position))
				{
					var newCart = Move(cart);
					AddCartOrCrash(newCart);
				}
			}
		}

		private void AddCartOrCrash(Cart cart)
		{
			if (Carts.ContainsKey(cart.Position))
			{
				Carts.Remove(cart.Position);
				crashLocations.Add((tick, cart.Position));
			}
			else
			{
				Carts.Add(cart.Position, cart);
			}
		}


		private Cart Move(Cart cart)
		{
			var nextPosition = cart.Position + cart.Velocity;
			var trackSymbol = TrackSymbolAt(nextPosition);
			var direction = cart.MovingDirection;
			var turnCounter = CalcTurn(cart, trackSymbol, ref direction);
			return new Cart
			(
				nextPosition,
				direction,
				turnCounter
			);
		}


		private static int CalcTurn(Cart cart, char trackSymbol, ref Direction direction)
		{
			Direction TurnLeft(Direction dir)
			{
				return (Direction) (((int) dir + 3) % 4);
			}

			Direction TurnRight(Direction dir)
			{
				return (Direction) (((int) dir + 1) % 4);
			}

			if (trackSymbol == '/')
			{
				switch (direction)
				{
					case Direction.Left:
					case Direction.Right:
						direction = TurnLeft(direction);
						break;
					case Direction.Up:
					case Direction.Down:
						direction = TurnRight(direction);
						break;
					default:
						throw new InvalidOperationException();
				}
			}

			if (trackSymbol == '\\')
			{
				switch (direction)
				{
					case Direction.Left:
					case Direction.Right:
						direction = TurnRight(direction);
						break;
					case Direction.Up:
					case Direction.Down:
						direction = TurnLeft(direction);
						break;
					default:
						throw new InvalidOperationException();
				}
			}

			if (trackSymbol != '+')
			{
				return cart.TurnCounter;
			}

			switch (cart.TurnCounter)
			{
				case -1:
					direction = TurnLeft(direction);
					return 0;
				case 0:
					return 1;
				case 1:
					direction = TurnRight(direction);
					return -1;
				default:
					throw new InvalidOperationException();
			}
		}

		public class Cart
		{
			private static readonly Dictionary<char, Direction> _mapSymbolToDir = new Dictionary<char, Direction>
			{
				{'<', Direction.Left},
				{'^', Direction.Up},
				{'>', Direction.Right},
				{'v', Direction.Down}
			};


			private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
			{
				{Direction.Left, new Size(-1, 0)},
				{Direction.Up, new Size(0, -1)},
				{Direction.Right, new Size(1, 0)},
				{Direction.Down, new Size(0, 1)}
			};


			public Cart(int x, int y, char symbol)
			{
				Position = new Point(x, y);
				MovingDirection = DirectionFromSymbol(symbol);
				TurnCounter = -1;
			}

			public Cart(Point pos, Direction direction, int turnCounter)
			{
				Position = pos;
				MovingDirection = direction;
				TurnCounter = turnCounter;
			}

			public int TurnCounter { get; }
			public Point Position { get; }
			public Direction MovingDirection { get; }

			public Size Velocity => _mapDirectionToSize[MovingDirection];

			private static Direction DirectionFromSymbol(char symbol)
			{
				return _mapSymbolToDir[symbol];
			}
		}
	}
}