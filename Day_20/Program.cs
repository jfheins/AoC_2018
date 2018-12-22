using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using MoreLinq;

namespace Day_20
{
	public class Program
	{
		private static readonly Regex groupMatcher = new Regex(@"(.+)\(([NEWS|]+)\)(.+)");

		private static readonly Dictionary<Point, bool[]> RoomsWithDoors = new Dictionary<Point, bool[]>();

		private static readonly Walker elf = new Walker();

		public static readonly Dictionary<char, Size> _mapDirectionToSize = new Dictionary<char, Size>
		{
			{'W', new Size(-1, 0)},
			{'N', new Size(0, -1)},
			{'E', new Size(1, 0)},
			{'S', new Size(0, 1)}
		};

		public static readonly Dictionary<int, Size> _mapIndexToSize = new Dictionary<int, Size>
		{
			{0, new Size(-1, 0)},
			{1, new Size(0, -1)},
			{2, new Size(1, 0)},
			{3, new Size(0, 1)}
		};

		private static void Main(string[] args)
		{
            var input = File.ReadAllText(@"../../../input.txt");
            //var input = "^abc(xyz)ccc(sdf)abc$";
			//input = @"^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
			//input = @"^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
			var sw = new Stopwatch();
			sw.Start();

			var tree = ParseThing(input.AsSpan(1, input.Length - 2));
			var paths = WalkAllRooms(tree as Sequence);
            Console.WriteLine("parsing done, walking prepared");

            RoomsWithDoors.Add(new Point(0, 0), new bool[4]);
            var i = 0;
			foreach (var path in paths)
			{
				WalkPath(path);
                i++;
                if (i%10000 == 0)
                {
                    Console.WriteLine($"Walked {i}");
                }
            }

            Console.WriteLine("walking done");

			var bfs = new BreadthFirstSearch<Point, int>(EqualityComparer<Point>.Default, Expander);

			var rooms = bfs.FindAll(new Point(0, 0), p => RoomsWithDoors[p].Count(x => x) == 1);

			var cornerRoom = rooms.MaxBy(r => r.Length).First();

			Console.WriteLine($"Part 1: Room {cornerRoom.Target} is {cornerRoom.Length} away");
			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static IEnumerable<Point> Expander(Point arg)
		{
			return RoomsWithDoors[arg].IndexWhere(x => x).Select(idx => arg + _mapIndexToSize[idx]);
		}

		private static void WalkPath(string path)
		{
			elf.Position = new Point(0, 0);
			foreach (var c in path)
				elf.Walk(c);
		}


		private static IEnumerable<string> WalkAllRooms(Sequence input)
		{
			return WalkAllRooms(Enumerable.Empty<string>(), input.Parts);
		}

		private static IEnumerable<string> WalkAllRooms(IEnumerable<string> prefix, IEnumerable<RegexComponent> suffix)
		{
			var first = suffix.FirstOrDefault();
			if (first == null)
			{
				return string.Concat(prefix).ToEnumerable();
			}

			if (first is Literal literal)
			{
				prefix = prefix.Concat(literal.Value.ToEnumerable());
				return WalkAllRooms(prefix, suffix.Skip(1));
			}

			if (first is Alternatives a)
			{
				return a.Parts.SelectMany(option => WalkAllRooms(prefix, option.ToEnumerable().Concat(suffix.Skip(1))));
			}

			if (first is Sequence s)
			{
				suffix = s.Parts.Concat(suffix.Skip(1));
				return WalkAllRooms(prefix, suffix);
			}

			throw new NotImplementedException();
		}


		private static RegexComponent ParseThing(ReadOnlySpan<char> window)
		{
			var level = 0;
			var isLiteral = true;
			foreach (var c in window)
			{
				if (c == '|' && level == 0)
				{
					return ParseOption(window);
				}

				if (c == '(')
				{
					isLiteral = false;
					level++;
				}

				if (c == ')')
				{
					level--;
				}
			}

			return isLiteral ? new Literal(window) : ParseSequence(window);
		}

		private static RegexComponent ParseSequence(ReadOnlySpan<char> str)
		{
			var result = new Sequence();
			var level = 0;
			var segmentStart = 0;

			var i = 0;
			while (i < str.Length)
			{
				if (str[i] == '(')
				{
                    if (level == 0 && i > segmentStart)
                    {
                        // Prefix
                        result.Add(new Literal(str.Slice(segmentStart, i - segmentStart)));
                    }
                    if (level == 0)
                    {
						segmentStart = i + 1;
					}

					level++;
				}
				else if (str[i] == ')')
				{
					level--;
					if (level == 0)
					{
						// Middle
						result.Add(ParseThing(str.Slice(segmentStart, i - segmentStart)));
						segmentStart = i + 1;
					}
				}

				i++;
			}

			if (segmentStart < str.Length)
			{
                if(str[segmentStart] == '(' && str[str.Length - 1] == ')')
                    result.Add(ParseThing(str.Slice(segmentStart+1, str.Length - segmentStart - 2)));
                else
                    result.Add(new Literal(str.Slice(segmentStart, str.Length - segmentStart)));
			}

			return result;
		}

		private static RegexComponent ParseOption(ReadOnlySpan<char> str)
		{
			var result = new Alternatives();
			var level = 0;
			var segmentStart = 0;
			var isLiteral = true;

			var i = 0;
			while (i < str.Length)
			{
				if (str[i] == '|' && level == 0)
				{
					if (isLiteral)
					{
						result.Add(new Literal(str.Slice(segmentStart, i - segmentStart)));
					}
					else
					{
						result.Add(ParseThing(str.Slice(segmentStart, i - segmentStart)));
					}

					segmentStart = i + 1;
					isLiteral = true;
				}
				else if (str[i] == '(')
				{
					level++;
					isLiteral = false;
				}
				else if (str[i] == ')')
				{
					level--;
				}

				i++;
			}

			if (segmentStart <= str.Length)
			{
				if (isLiteral)
				{
					result.Add(new Literal(str.Slice(segmentStart, str.Length - segmentStart)));
				}
				else
				{
					result.Add(ParseThing(str.Slice(segmentStart, str.Length - segmentStart)));
				}
			}

			return result;
		}

		private class Walker
		{
			public Point Position { get; set; } = new Point(0, 0);

			private static int mapDirToOppositeIndex(char c)
			{
				return "ESWN".IndexOf(c);
			}

			public static int mapDirToIndex(char c)
			{
				return "WNES".IndexOf(c);
			}

			public void Walk(char direction)
			{
				RoomsWithDoors[Position][mapDirToIndex(direction)] = true;
				Position += _mapDirectionToSize[direction];
				if (!RoomsWithDoors.ContainsKey(Position))
				{
					RoomsWithDoors[Position] = new bool[4];
				}

				RoomsWithDoors[Position][mapDirToOppositeIndex(direction)] = true;
			}
		}
	}

	public class RegexComponent { }


	public class Literal : RegexComponent
	{
		public Literal(ReadOnlySpan<char> value)
		{
			Value = value.ToString();
			if (Value.Contains('('))
			{
				throw new InvalidOperationException();
			}
		}

		public Literal()
		{
			Value = "";
		}

		public string Value { get; set; }

		public override string ToString()
		{
			return Value;
		}
	}

	public class Alternatives : RegexComponent
	{
		public List<RegexComponent> Parts { get; } = new List<RegexComponent>();

		public void Add(RegexComponent item)
		{
			Parts.Add(item);
		}
	}

	public class Sequence : RegexComponent
	{
		public List<RegexComponent> Parts { get; } = new List<RegexComponent>();

		public void Add(RegexComponent item)
		{
			Parts.Add(item);
		}
	}
}