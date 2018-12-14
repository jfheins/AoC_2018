using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Day_13
{
	internal class Program
	{
		private static Bitmap _trackMap;

		private static readonly Size offset = new Size(2, 2);
		private static readonly Size cartSize = new Size(1, 1);
		private static readonly Size unitSize = new Size(1, 1);
		private static readonly int scale = 3;

		private static readonly Pen _pathPen = new Pen(Color.Green);

		private static void Main()
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var sim = new TrackSimulator(input);
			GenerateTrackMap(sim);

			// initial
			Console.WriteLine($"{sim.Carts.Count} carts found on map.");

			while (sim.Carts.Count > 1)
			{
				var pic = PaintPicture(sim);
				pic.Save(@"E:\Temp\aoc\" + sim.tick.ToString("D5") + ".bmp", ImageFormat.Bmp);
				pic.Dispose();
				sim.Step();

				if (sim.tick > 5000)
				{
					break;
				}
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

		private static void GenerateTrackMap(TrackSimulator sim)
		{
			_trackMap = new Bitmap(scale * sim.Width + offset.Width * 2, scale * sim.Height + offset.Height * 2);

			using (var graphics = Graphics.FromImage(_trackMap))
			{
				// Paint tracks
				for (var x = 0; x < sim.Width; x++)
				{
					for (var y = 0; y < sim.Height; y++)
					{
						var chr = sim._tracks[y][x];
						if ("-|/\\".Contains(chr))
						{
							_trackMap.SetPixel(scale * x + offset.Width, scale * y + offset.Height, Color.DarkGray);
						}
					}
				}
			}
		}

		private static Bitmap PaintPicture(TrackSimulator sim)
		{
			var bmp = new Bitmap(scale * sim.Width + offset.Width * 2, scale * sim.Height + offset.Height * 2);

			Point Scale(Point p)
			{
				return new Point(scale * p.X, scale * p.Y);
			}
			using (var graphics = Graphics.FromImage(bmp))
			{
				graphics.FillRectangle(Brushes.Black, graphics.ClipBounds);
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				// Paint tracks
				graphics.DrawImageUnscaled(_trackMap, 0, 0);

				// Paint traces
				foreach (var path in sim.Carts.Select(c => c.Path))
				{
					if (path.Count > 1)
					{
						var points = path.TakeLast(50).Select(Scale);
						graphics.DrawLines(_pathPen, points.ToArray());
					}
				}

				// Paint carts
				foreach (var cart in sim.Carts)
				{
					var cartRect = new Rectangle(Scale(cart.Position) - cartSize, cartSize * 2 + unitSize);
					graphics.FillRectangle(Brushes.Aqua, cartRect);
				}

				// Paint crashes
				foreach (var p in sim.crashLocations)
				{
					var crashRect = new Rectangle(Scale(p.location) - cartSize, cartSize * 2 + unitSize);
					graphics.FillRectangle(Brushes.Red, crashRect);
				}
			}

			return bmp;
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

		public readonly string[] _tracks;
		public readonly List<Cart> Carts;
		public int tick;

		public TrackSimulator(string[] input)
		{
			_tracks = input.Select(ParseLineToTrack).ToArray();
			Carts = ParseCarts(input).ToList();
			Width = _tracks[0].Length;
			Height = _tracks.Length;
		}

		public int Width { get; }
		public int Height { get; }

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
				Path.Add(Position);
				MovingDirection = DirectionFromSymbol(symbol);
				TurnCounter = -1;
			}

			public int TurnCounter { get; private set; }
			public Point Position { get; private set; }
			public Direction MovingDirection { get; private set; }
			public List<Point> Path { get; } = new List<Point>();

			public Size Velocity => _mapDirectionToSize[MovingDirection];

			private static Direction DirectionFromSymbol(char symbol)
			{
				return _mapSymbolToDir[symbol];
			}

			public void Move()
			{
				Position += Velocity;
				Path.Add(Position);
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