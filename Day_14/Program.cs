using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_14
{
	class Program
	{
		private static void Main()
		{
			var input = 503761;//503761;
			var sampleSize = 10;
			var sw = new Stopwatch();
			sw.Start();

			var scoreboard = new List<int>(503861) {3, 7};

			var elf1 = 0;
			var elf2 = 1;
			while (scoreboard.Count < input + sampleSize)
			{
				//foreach (var score in scoreboard)
				//{
				//	Console.Write($"{score} ");
				//}
				//Console.WriteLine();
				var newScore = scoreboard[elf1] + scoreboard[elf2];
				var digits = newScore.ToString().Select(t => int.Parse(t.ToString())).ToArray();
				scoreboard.AddRange(digits);
				elf1 = (elf1 + 1 +scoreboard[elf1]) % scoreboard.Count;
				elf2 = (elf2 + 1 +scoreboard[elf2]) % scoreboard.Count;
			}

			var result = string.Concat(scoreboard.Skip(input).Take(sampleSize).Select(score => score.ToString()));
			Console.WriteLine(result);

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}
