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
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var sim = new TrackSimulator(input);

			// initial
			Console.WriteLine($"{sim.carts.Count} carts found on map.");

			while (sim.carts.Count > 1)
			{
				sim.Step();
			}

			Console.WriteLine($"First crash was at {sim.crashLocations.First().location}");
			Console.WriteLine($"Last cart is at {sim.carts.First().Key}");

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

		private readonly string[] tracks;
		public Dictionary<Point, Cart> carts;
		private int tick;

		public TrackSimulator(string[] input)
		{
			tracks = input.Select(ParseLineToTrack).ToArray();
			carts = ParseCarts(input).ToDictionary(cart => cart.Position);
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

		protected char TrackSymbolAt(Point p)
		{
			return tracks[p.Y][p.X];
		}

		public void Step()
		{
			tick++;
			foreach (var cart in CopyAndSortCarts(carts.Values))
			{
				if (carts.ContainsValue(cart))
				{
					var newCart = Move(cart);
					AddCartOrCrash(newCart);
				}
			}
		}

		private void AddCartOrCrash(Cart cart)
		{
			if (carts.ContainsKey(cart.Position))
			{
				carts.Remove(cart.Position);
				crashLocations.Add((tick, cart.Position));
			}
			else
			{
				carts.Add(cart.Position, cart);
			}
		}


		internal Cart Move(Cart cart)
		{
			var nextPosition = cart.Position + cart.Velocity;
			var trackSymbol = TrackSymbolAt(nextPosition);
			var direction = cart.MovingDirection;
			var turnCounter = CalcTurn(cart, trackSymbol, ref direction);
			return new Cart
			{
				Position = nextPosition,
				MovingDirection = direction,
				TurnCounter = turnCounter
			};
		}


		private static int CalcTurn(Cart cart, char trackSymbol, ref Direction direction)
		{
			Func<Direction, Direction> turnLeft = dir => (Direction) (((int) dir + 3) % 4);
			Func<Direction, Direction> turnRight = dir => (Direction) (((int) dir + 1) % 4);

			if (trackSymbol == '/')
			{
				switch (direction)
				{
					case Direction.Left:
					case Direction.Right:
						direction = turnLeft(direction);
						break;
					case Direction.Up:
					case Direction.Down:
						direction = turnRight(direction);
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
						direction = turnRight(direction);
						break;
					case Direction.Up:
					case Direction.Down:
						direction = turnLeft(direction);
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
					direction = turnLeft(direction);
					return 0;
				case 0:
					return 1;
				case 1:
					direction = turnRight(direction);
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
				TurnCounter = -1;
				MovingDirection = DirectionFromSymbol(symbol);
			}

			public Cart() { }

			public int TurnCounter { get; set; }
			public Point Position { get; set; }
			public Direction MovingDirection { get; set; }

			public Size Velocity => _mapDirectionToSize[MovingDirection];

			private Direction DirectionFromSymbol(char symbol)
			{
				return _mapSymbolToDir[symbol];
			}
		}
	}
}