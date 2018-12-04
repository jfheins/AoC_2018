using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_03
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            var sw = new Stopwatch();
            sw.Start();
			var input = File.ReadAllLines(@"../../../input.txt");
		    //input = new [] {"#1 @ 1,3: 4x4", "#2 @ 3,1: 4x4", "#3 @ 5,5: 2x2"};

		    var solver = ClaimIntersector.Parse(input);

		    var intersectionArea = solver.GetIntersectionArea();
		    var remaining = solver.GetNonIntersectingClaims().ToList();
		    

		    Console.WriteLine(intersectionArea);
		    Console.WriteLine();

		    foreach (var claim in remaining)
		    {
		        Console.WriteLine($"Non-intersecting: #{claim}");
		    }
            sw.Stop();
		    Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
		}


	}
}