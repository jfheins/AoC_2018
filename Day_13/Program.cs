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
				Console.WriteLine($"Last cart is at {sim.Carts.First().Position}");
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
		public readonly List<Cart> Carts;
		private int tick;

		public TrackSimulator(string[] input)
		{
			_tracks = input.Select(ParseLineToTrack).ToArray();
			Carts = ParseCarts(input).ToList();
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
			foreach (var cart in CopyAndSortCarts(Carts))
			{
				cart.Move();
				if (CartCrashes(cart))
				{
					Carts.RemoveAll(c => c.Position == cart.Position);
					crashLocations.Add((tick, cart.Position));
				}
				else
				{
					cart.Turn(TrackSymbolAt(cart.Position));
				}
			}
		}

		private bool CartCrashes(Cart cart)
		{
			return Carts
				.Where(c => c != cart)
				.Any(c => c.Position == cart.Position);
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

			public int TurnCounter { get; private set; }
			public Point Position { get; private set; }
			public Direction MovingDirection { get; private set; }

			public Size Velocity => _mapDirectionToSize[MovingDirection];

			private static Direction DirectionFromSymbol(char symbol)
			{
				return _mapSymbolToDir[symbol];
			}

			public void Move()
			{
				Position += Velocity;
			}

			private void TurnLeft()
			{
				MovingDirection = (Direction) (((int) MovingDirection + 3) % 4);
			}

			private void TurnRight()
			{
				MovingDirection = (Direction) (((int) MovingDirection + 1) % 4);
			}

			public void Turn(char trackSymbol)
			{
				if (trackSymbol == '/')
				{
					switch (MovingDirection)
					{
						case Direction.Left:
						case Direction.Right:
							TurnLeft();
							break;
						case Direction.Up:
						case Direction.Down:
							TurnRight();
							break;
						default:
							throw new InvalidOperationException();
					}
				}

				if (trackSymbol == '\\')
				{
					switch (MovingDirection)
					{
						case Direction.Left:
						case Direction.Right:
							TurnRight();
							break;
						case Direction.Up:
						case Direction.Down:
							TurnLeft();
							break;
						default:
							throw new InvalidOperationException();
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
						default:
							throw new InvalidOperationException();
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