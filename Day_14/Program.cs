using System;
using System.Diagnostics;
using System.IO;

namespace Day_14
{
	class Program
	{
		private static void Main()
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();



			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}
