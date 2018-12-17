using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Core;
using MoreLinq.Extensions;

namespace Day_15
{
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
		private readonly Dictionary<Point, bool> _walkable;
		public string TerminationReason { get; private set; } = "";
		public string Winners { get; private set; } = "";


		public BattleSimulator(string[] input, int elfAttack = 3)
		{
			_cave = input.Select(ParseLineToCave).ToArray();
			Players = ParsePlayers(input, elfAttack).ToList();
			CaveBounds = new Rectangle(0, 0, _cave[0].Length, _cave.Length);

			_walkable = new Dictionary<Point, bool>(Enumerable.Range(0, CaveBounds.Width)
				.Cartesian(Enumerable.Range(0, CaveBounds.Height), (x, y) =>
				{
					var p = new Point(x, y);
					return new KeyValuePair<Point, bool>(p, IsWalkable(p));
				}));

			_bfs = new BreadthFirstSearch<Point, Direction>(
				EqualityComparer<Point>.Default,
				point => GetAdjacentPoints(point)
					.Where(p => _walkable[p]));
			_bfs.PerformParallelSearch = false;
		}

		public Rectangle CaveBounds { get; }
		public List<Player> Players { get; }

		public int Rounds { get; private set; }
		public int HitPointSum => Players.Sum(p => p.HitPoints);

		private string ParseLineToCave(string arg)
		{
			return arg.Replace('E', '.')
				.Replace('G', '.');
		}

		private static IEnumerable<Player> ParsePlayers(string[] input, int elfAttack)
		{
			for (var y = 0; y < input.Length; y++)
			{
				for (var x = 0; x < input[y].Length; x++)
				{
					if (input[y][x] == 'G')
					{
						yield return new Player(x, y, input[y][x]);
					}
					else if (input[y][x] == 'E')
					{
						yield return new Player(x, y, input[y][x], elfAttack);
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
		public bool Step(bool stopOnElfDeath = false)
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
					TerminationReason = $"Player from team {player.Symbol} found no target.";
					Winners = player.Symbol.ToString();
					return false;
				}

				var adjacentTargets = possibleTargets.Where(target => AreAdjacent(player, target)).ToList();
				if (!adjacentTargets.Any())
				{
					var positionsInRange = new HashSet<Point>(possibleTargets
						.SelectMany(t => GetAdjacentPoints(t.Position))
						.Where(IsWalkable));

					var nearReachablePositions = GetAdjacentPoints(player.Position)
						.Where(IsWalkable)
						.AsParallel()
						.SelectMany(firstStep => _bfs.FindAll(firstStep,
							p => positionsInRange.Contains(p), null, 1))
						.ToList();

					if (nearReachablePositions.Any())
					{
						// As the first step was expanded manually, we have to select the shortest path from those
						var firstInReadingOrder = nearReachablePositions
							.OrderBy(path => path.Length)
							.ThenBy(path => path.Target.Y)
							.ThenBy(path => path.Target.X)
							.First();
						var targetSquare = firstInReadingOrder.Steps[0];
						var oldPosition = player.Position;
						player.StepTo(targetSquare);
						adjacentTargets = possibleTargets.Where(target => AreAdjacent(player, target)).ToList();

						RefreshCache(oldPosition);
						RefreshCache(player.Position);
					}
				}

				if (adjacentTargets.Any())
				{
					var victim = player.AttackOneOf(adjacentTargets);
					if (victim.HitPoints <= 0)
					{
						Players.Remove(victim);
						if (victim.IsElf && stopOnElfDeath)
						{
							TerminationReason = $"An elf died in round {Rounds+1}.";
							Winners = player.Symbol.ToString();
							return false;
						}

						RefreshCache(victim.Position);
					}
				}
			}

			Rounds++;
			return true;
		}

		private void RefreshCache(Point p)
		{
			_walkable[p] = IsWalkable(p);
		}

		private bool AreAdjacent(Player a, Player b)
		{
			return GetDistance(a.Position, b.Position) == 1;
		}

		private IEnumerable<Point> GetAdjacentPoints(Point p)
		{
			yield return p + _mapDirectionToSize[Direction.Up];
			yield return p + _mapDirectionToSize[Direction.Left];
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
			public Player(int x,
						  int y,
						  char symbol,
						  int attack = 3)
			{
				Debug.Assert(symbol == 'E' || symbol == 'G');
				Symbol = symbol;
				Position = new Point(x, y);
				HitPoints = 200;
				AttackPower = attack;
			}

			public int AttackPower { get; }

			public char Symbol { get; }

			public bool IsElf => Symbol == 'E';
			public bool IsGoblin => Symbol == 'G';

			public int HitPoints { get; set; }

			public Point Position { get; set; }

			public Player AttackOneOf(List<Player> adjacentTargets)
			{
				if (!adjacentTargets.Any())
				{
					// Nothing to attack
					return null;
				}

				var victim = adjacentTargets
					.OrderBy(t => t.HitPoints)
					.ThenBy(t => t.Position.Y)
					.ThenBy(t => t.Position.X)
					.First();
				victim.HitPoints -= AttackPower;
				return victim;
			}

			public void StepTo(Point target)
			{
				Position = target;
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