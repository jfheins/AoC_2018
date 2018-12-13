using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using MoreLinq.Extensions;

namespace Day_13
{
	class Program
	{
		static void Main(string[] args)
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
