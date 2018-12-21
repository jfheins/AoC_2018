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
			//input = @"^E|SSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
			var sw = new Stopwatch();
			sw.Start();

			var possiblePaths = new HashSet<string>(RegexToPath(input));

			while (didWork)
			{
				didWork = false;
				possiblePaths = new HashSet<string>(possiblePaths.SelectMany(RegexToPath));
			}
			// Top level option?
			Debug.Assert(possiblePaths.All(s => !s.Contains('|')), "No path shall contain |");



			Console.WriteLine(possiblePaths.Count);

			Console.WriteLine("Part 1: ");
			Console.WriteLine("Part 2: ");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static bool didWork = true;

		private static IEnumerable<string> RegexToPath(string str)
		{
			var match = groupMatcher.Match(str);

			if (match.Success)
			{
				didWork = true;
				Debug.Assert(match.Groups.Count == 4);

				var options = ParseOptions(match.Groups[2].Value);

				foreach (var option in options)
					yield return match.Groups[1].Value + option + match.Groups[3].Value;
			}
			else
			{
				yield return str;
			}
		}

		private static IEnumerable<string> ParseOptions(string str)
		{
			return str.Split('|');
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