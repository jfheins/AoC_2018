using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core.Combinatorics;

namespace Day_02
{
	class Program
	{
		static void Main()
		{
			var input = new [] { "abcdef", "bababc", "abbcde", "abcccd", "aabcdd", "abcdee", "ababab" };
			input = File.ReadAllLines(@"../../../input.txt");

			var BoxIdList = input.Select(id => id
				.ToLookup(c => c)
				.Select(chargroup => chargroup.Count())
				.Where(count => count > 1)
				).Select(group => new HashSet<int>(group)).ToList();

			var checksum = BoxIdList.Count(boxid => boxid.Contains(2)) * BoxIdList.Count(boxid => boxid.Contains(3));
			Console.WriteLine($"Checksum für this list is {checksum}");

			var pairGen = new Combinations<string>(input, 2);

			foreach (var pair in pairGen)
			{
				Debug.Assert(pair.Count == 2);

				var letterIsEqual = pair[0].Zip(pair[1], (a, b) => a == b).ToArray();
				var diffCount = letterIsEqual.Sum(b => b ? 0 : 1);

				if (diffCount == 1)
				{
					var commonLetters = string.Concat(pair[0].Zip(letterIsEqual, (letter, ise) => ise ? letter.ToString() : ""));
					Console.WriteLine($"Pair found: {commonLetters}");
				}
			}



			Console.ReadLine();
		}
	}
}
