using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			Console.ReadLine();
		}
	}
}
