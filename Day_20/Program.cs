using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_20
{
	public class Program
	{
		private static readonly Regex groupMatcher = new Regex(@"(.+)\(([NEWS|]+)\)(.+)");

		private static void Main(string[] args)
		{
			var input = File.ReadAllText(@"../../../input.txt");
			input = @"^E|SSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
			var sw = new Stopwatch();
			sw.Start();

			var tree = ParseThing(input);

			Console.WriteLine(tree);

			Console.WriteLine("Part 1: ");
			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static RegexComponent ParseThing(ReadOnlySpan<char> window)
		{
			foreach (var c in window)
			{
				if (c == '|')
				{
					return ParseOption(window);
				}
				if (c == '(')
				{
					return ParseSequence(window);
				}
			}
			return new Literal(window);
		}

		private static RegexComponent ParseSequence(ReadOnlySpan<char> str)
		{
			Console.WriteLine(str.ToString());
			var result = new Sequence();
			var level = 0;
			var segmentStart = 1;

			// Exclude first and last char
			var i = 1;
			while (i < str.Length - 1)
			{
				if (str[i] == '(')
				{
					if (i > segmentStart && level == 0)
					{
						// Prefix
						result.Add(new Literal(str.Slice(segmentStart + 1, i - segmentStart + 1)));
						segmentStart = i;
					}
					level++;
				}
				else if (str[i] == ')')
				{
					level--;
					if (level == 0)
					{
						// Middle
						result.Add(ParseThing(str.Slice(segmentStart, i - segmentStart + 2)));
						segmentStart = i+1;
					}
				}

				i++;
			}

			if (segmentStart < str.Length - 2)
			{
				// Suffix
				result.Add(new Literal(str.Slice(segmentStart, str.Length - segmentStart)));
			}

			return result;
		}

		private static RegexComponent ParseOption(ReadOnlySpan<char> str)
		{
			Console.WriteLine(str.ToString());
			var result = new Alternatives();
			var level = 0;
			var segmentStart = 0;
			var isLiteral = true;

			// Exclude first and last char
			var i = 1;
			while (i < str.Length - 1)
			{
				if (str[i] == '|' && level == 0)
				{
					result.Add(new Literal(str.Slice(segmentStart + 1, i - segmentStart - 1)));
					segmentStart = i;
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
					if (level == 0)
					{
						result.Add(ParseSequence(str.Slice(segmentStart, i - segmentStart + 2)));
						segmentStart = i;
					}
				}

				i++;
			}

			if (segmentStart < str.Length - 2)
			{
				result.Add(new Literal(str.Slice(segmentStart, str.Length - segmentStart)));
			}

			return result;
		}
	}

	public class RegexComponent { }


	public class Literal : RegexComponent
	{
		public Literal(ReadOnlySpan<char> value)
		{
			Value = value.ToString();
		}

		public Literal()
		{
			Value = "";
		}

		public string Value { get; set; }
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