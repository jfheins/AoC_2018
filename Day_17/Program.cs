using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;

namespace Day_17
{
	public class Program
	{
		private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
		{
			{Direction.Left, new Size(-1, 0)},
			{Direction.Up, new Size(0, -1)},
			{Direction.Right, new Size(1, 0)},
			{Direction.Down, new Size(0, 1)}
		};

		private static Dictionary<Point, char> ground;

		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var clayCoords = input.SelectMany(ParseLine);
			var spring = new Point(500, 0);

			ground = new Dictionary<Point, char>();
			foreach (var clayCoord in clayCoords)
			{
				ground[clayCoord] = '#';
			}


			var maxX = ground.Keys.Max(c => c.X);
			var minX = ground.Keys.Min(c => c.X);
			var maxY = ground.Keys.Max(c => c.Y);
			var minY = ground.Keys.Min(c => c.Y);
			ground[spring] = '+';

			FillSection(maxY, spring);

			for (var y = minY; y <= maxY; y++)
			{
				char[] line = new char[maxX - minX + 1];
				for (var x = minX; x <= maxX; x++)
				{
					if (!ground.TryGetValue(new Point(x, y), out var p))
					{
						p = '.';
					}

					line[x-minX] = p;
				}

				Console.WriteLine(y + ": " + string.Concat(line));
			}

			var part1 = ground
				.Where(kvp => kvp.Key.Y >= minY && kvp.Key.Y <= maxY)
				.Count(kvp => "|~".Contains(kvp.Value));

			Console.WriteLine($"Part 1: {part1}");
			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static void FillSection(int maxY, Point tip)
		{
			while (tip.Y <= maxY)
			{
				if (SymbolAt(tip, Direction.Down) == '.')
				{
					// Tip can move down
					tip += _mapDirectionToSize[Direction.Down];
					ground[tip] = '|';
				}
				else
				{
					if (SymbolAt(tip, Direction.Down) == '|')
					{
						break;
					}
					Debug.Assert("#~".Contains(SymbolAt(tip, Direction.Down)));
					// Expand left & right
					var boundedLeft = Probe(tip, Direction.Left);
					var boundedRight = Probe(tip, Direction.Right);
					if (boundedRight && boundedLeft)
					{
						FillToClay(tip, Direction.Left, '~');
						FillToClay(tip, Direction.Right, '~');
						tip += _mapDirectionToSize[Direction.Up];
					}
					else
					{
						if (boundedLeft)
						{
							FillToClay(tip, Direction.Left, '|');
						}
						else
						{
							//if (SymbolAt(tip) == '|')
							{
								var newTip = FillToOverflow(tip, Direction.Left, '|');
								FillSection(maxY, newTip);
							}
						}

						if (boundedRight)
						{
							FillToClay(tip, Direction.Right, '|');
						}
						else
						{
							if (SymbolAt(tip) != '~')
							{
								var newTip = FillToOverflow(tip, Direction.Right, '|');
								FillSection(maxY, newTip);
							}
						}

						break;
					}
				}
			}
		}

		private static void FillToClay(Point tip, Direction dir, char c)
		{
			while (SymbolAt(tip) != '#')
			{
				ground[tip] = c;
				tip += _mapDirectionToSize[dir];
			}
		}

		private static Point FillToOverflow(Point tip, Direction dir, char c)
		{
			while ("#~".Contains(SymbolAt(tip, Direction.Down)))
			{
				ground[tip] = c;
				tip += _mapDirectionToSize[dir];
			}
			ground[tip] = c;
			return tip;
		}

		private static bool Probe(Point tip, Direction dir)
		{
			while (true)
			{
				if (SymbolAt(tip, dir) == '#')
				{
					return true;
				}

				if (".|".Contains(SymbolAt(tip, Direction.Down)))
				{
					return false;
				}

				tip += _mapDirectionToSize[dir];
			}
		}

		private static char SymbolAt(Point p, Direction? d = null)
		{
			var delta = d.HasValue ? _mapDirectionToSize[d.Value] : Size.Empty;
			return ground.GetOrAdd(p + delta, () => '.');
		}

		private static IEnumerable<Point> ParseLine(string arg)
		{
			var xStr = Regex.Match(arg, @"x=([-0-9]+)(?:(?:\.\.)([-0-9]+))?").Groups;
			var minX = int.Parse(xStr[1].Value);
			if (!int.TryParse(xStr[2].Value, out var maxX))
			{
				maxX = minX;
			}

			var yStr = Regex.Match(arg, @"y=([-0-9]+)(?:(?:\.\.)([-0-9]+))?").Groups;
			var minY = int.Parse(yStr[1].Value);

			if (!int.TryParse(yStr[2].Value, out var maxY))
			{
				maxY = minY;
			}

			for (var x = minX; x <= maxX; x++)
			{
				for (var y = minY; y <= maxY; y++)
				{
					yield return new Point(x, y);
				}
			}
		}
	}

	public enum Direction
	{
		Left,
		Up,
		Right,
		Down
	}
}