﻿using System;
using System.Buffers.Text;
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
			var input = File.ReadAllLines(@"../../../input.txt");
		    //input = new [] {"#1 @ 1,3: 4x4", "#2 @ 3,1: 4x4", "#3 @ 5,5: 2x2"};

		    var claimsById = input.Select(ParseClaim).ToDictionary(t => t.id, t => t.claim);
		    var claims = claimsById.Values.ToList();


            var maxX = claims.Max(c => c.Right);
		    var maxY = claims.Max(c => c.Bottom);

		    int intersectionArea = 0;
		    for (int x = 0; x < maxX; x++)
		    {
		        for (int y = 0; y < maxY; y++)
		        {
		            if (claims.Count(claim => claim.Contains(x, y)) > 1)
		            {
                        // Intersection !
		                intersectionArea++;

		            }
		        }
		    }

		    Console.WriteLine(intersectionArea);
            Console.ReadLine();
		}


	    private static  (int id, Rectangle claim) ParseClaim(string line)
	    {
	        // Format: #1 @ 1,3: 4x4
	        var regex = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");

	        var groups = regex.Match(line).Groups;
	        Debug.Assert(groups.Count == 6);
	        var numbers = groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
	        var id = numbers[0];
	        return (id, new Rectangle(numbers[1], numbers[2], numbers[3], numbers[4]));
	    }
	}
}