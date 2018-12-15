using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;

namespace Day_15
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();


			Console.WriteLine($"Part 1: ");
			Console.WriteLine($"Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}


	public enum Direction
	{
		Left,
		Up,
		Right,
		Down
	}

	public class BattleSimulator
	{
		private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
		{
			{Direction.Left, new Size(-1, 0)},
			{Direction.Up, new Size(0, -1)},
			{Direction.Right, new Size(1, 0)},
			{Direction.Down, new Size(0, 1)}
		};

		private readonly string[] _cave;
		private BreadthFirstSearch<Point, Direction> _bfs;

		public BattleSimulator(string[] input)
		{
			_cave = input.Select(ParseLineToCave).ToArray();
			Players = ParsePlayers(input).ToList();
			CaveBounds = new Rectangle(0, 0, _cave[0].Length, _cave.Length);
			_bfs = new BreadthFirstSearch<Point, Direction>(
				EqualityComparer<Point>.Default,
				point => GetAdjacentPoints(point).Where(IsWalkable));
		}

		public Rectangle CaveBounds { get; }
		public List<Player> Players { get; set; }

		public int Round { get; private set; }

		private string ParseLineToCave(string arg)
		{
			return arg.Replace('E', '.')
				.Replace('G', '.');
		}

		private static IEnumerable<Player> ParsePlayers(string[] input)
		{
			for (var y = 0; y < input.Length; y++)
			{
				for (var x = 0; x < input[y].Length; x++)
				{
					if ("GE".Contains(input[y][x]))
					{
						yield return new Player(x, y, input[y][x]);
					}
				}
			}
		}

		private bool IsWalkable(Point p)
		{
			return CaveBounds.Contains(p) && _cave[p.Y][p.X] == '.';
		}

		private static Player[] CopyAndSortPlayers(IEnumerable<Player> players)
		{
			return players.OrderBy(c => c.Position.Y)
				.ThenBy(c => c.Position.X)
				.ToArray();
		}


		public bool Step()
		{
			foreach (var player in CopyAndSortPlayers(Players))
			{
				var possibleTargets = Players.Where(p => p.Symbol != player.Symbol).ToList();
				if (!possibleTargets.Any())
				{
					return false;
				}

				var adjacentTargets = possibleTargets.Where(target => AreAdjacent(player, target)).ToList();
				if (adjacentTargets.Any())
				{
					player.FightOneOf(adjacentTargets);
				}
				else
				{
					var targetPositions = possibleTargets
						.SelectMany(t => GetAdjacentPoints(t.Position))
						.Where(IsWalkable);
				}
			}

			Round++;
			return true;
		}

		private bool AreAdjacent(Player a, Player b)
		{
			return GetDistance(a.Position, b.Position) == 1;
		}

		private IEnumerable<Point> GetAdjacentPoints(Point p)
		{
			yield return p + _mapDirectionToSize[Direction.Left];
			yield return p + _mapDirectionToSize[Direction.Up];
			yield return p + _mapDirectionToSize[Direction.Right];
			yield return p + _mapDirectionToSize[Direction.Down];
		}

		private int GetDistance(Point a, Point b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		public class Player
		{
			public Player(int x, int y, char symbol)
			{
				Debug.Assert(symbol == 'E' || symbol == 'G');
				Symbol = symbol;
				Position = new Point(x, y);
				HitPoints = 200;
			}

			public char Symbol { get; }

			public bool IsElf => Symbol == 'E';
			public bool IsGoblin => Symbol == 'G';

			public int HitPoints { get; set; }

			public Point Position { get; set; }

			public void FightOneOf(List<Player> adjacentTargets)
			{
				throw new NotImplementedException();
			}
		}
	}
}