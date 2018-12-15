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
			var input = File.ReadAllLines(@"../../../demo.txt");
			var sw = new Stopwatch();
			sw.Start();

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 49)
			{
				Console.WriteLine($"Step {sim.Rounds} fought, {sim.Players.Count} players left");
				Console.WriteLine(sim.ToString());
			}

			Console.WriteLine($"Part 1: {sim.Rounds}");
			Console.WriteLine("Part 2: ");

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

		private readonly BreadthFirstSearch<Point, Direction> _bfs;

		private readonly string[] _cave;

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
		public List<Player> Players { get; }

		public int Rounds { get; private set; }

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
			return CaveBounds.Contains(p)
				   && Players.All(player => player.Position != p)
				   && _cave[p.Y][p.X] == '.';
		}

		private static Player[] CopyAndSortPlayers(IEnumerable<Player> players)
		{
			return players.OrderBy(c => c.Position.Y)
				.ThenBy(c => c.Position.X)
				.ToArray();
		}

		/// <summary>
		///     Does a time step in the combat
		/// </summary>
		/// <returns>Whether this was a "full step" or not</returns>
		public bool Step()
		{
			foreach (var player in CopyAndSortPlayers(Players))
			{
				if (player.HitPoints <= 0)
				{
					continue;
				}

				var possibleTargets = Players.Where(p => p.Symbol != player.Symbol).ToList();
				if (!possibleTargets.Any())
				{
					return false;
				}

				var adjacentTargets = possibleTargets.Where(target => AreAdjacent(player, target)).ToList();
				if (!adjacentTargets.Any())
				{
					var positionsInRange = new HashSet<Point>(possibleTargets
						.SelectMany(t => GetAdjacentPoints(t.Position))
						.Where(IsWalkable));

					var nearReachablePositions = _bfs.FindAll(player.Position,
						p => positionsInRange.Contains(p), null, 1);

					if (nearReachablePositions.Any())
					{
						var firstInReadingOrder = nearReachablePositions
							.OrderBy(path => path.Target.Y)
							.ThenBy(path => path.Target.X)
							.First();
						player.StepTowards(firstInReadingOrder.Target);
						adjacentTargets = possibleTargets.Where(target => AreAdjacent(player, target)).ToList();
					}
				}
				if (adjacentTargets.Any())
				{
					player.AttackOneOf(adjacentTargets);
				}

				Players.RemoveAll(p => p.HitPoints <= 0);
			}

			Rounds++;
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

		public override string ToString()
		{
			var res = _cave.Select(s => s.ToCharArray()).ToArray();
			foreach (var player in Players)
			{
				res[player.Position.Y][player.Position.X] = player.Symbol;
			}

			return string.Concat(res.Select(arr => string.Concat(arr) + "\r\n"));
		}

		public class Player
		{
			public Player(int x, int y, char symbol)
			{
				Debug.Assert(symbol == 'E' || symbol == 'G');
				Symbol = symbol;
				Position = new Point(x, y);
				HitPoints = 200;
				AttackPower = 3;
			}

			public int AttackPower { get; }

			public char Symbol { get; }

			public bool IsElf => Symbol == 'E';
			public bool IsGoblin => Symbol == 'G';

			public int HitPoints { get; set; }

			public Point Position { get; set; }

			public void AttackOneOf(List<Player> adjacentTargets)
			{
				if (!adjacentTargets.Any())
				{
					// Nothing to attack
					return;
				}

				var victim = adjacentTargets
					.OrderBy(t => t.HitPoints)
					.ThenBy(t => t.Position.Y)
					.ThenBy(t => t.Position.X)
					.First();
				victim.HitPoints -= AttackPower;
			}

			public void StepTowards(Point target)
			{
				// Move up
				if (target.Y < Position.Y)
				{
					Position += _mapDirectionToSize[Direction.Up];
				}

				// Move left
				if (target.X < Position.X)
				{
					Position += _mapDirectionToSize[Direction.Left];
				}

				// Move right
				if (target.X > Position.X)
				{
					Position += _mapDirectionToSize[Direction.Right];
				}

				// Move down
				if (target.Y > Position.Y)
				{
					Position += _mapDirectionToSize[Direction.Down];
				}
			}
		}
	}
}