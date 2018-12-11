using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core;
using MoreLinq;

namespace Day_11
{
	internal class Program
	{
		private static int _serialNumber;

		public static void Main(string[] args)
		{
			_serialNumber = 7315;
			var gridsize = 300;

			var sw = new Stopwatch();
			sw.Start();

			var powers = new Dictionary<(int, int, int), int>();
			
			for (var sqsize = 1; sqsize <= 20; sqsize++)
			{
				Console.WriteLine($"Square: {sqsize}");
				for (var x = 0; x < gridsize - sqsize + 1; x++)
				for (var y = 0; y < gridsize - sqsize + 1; y++)
				{
					var power = 0;
					for (var i = 1; i <= sqsize; i++)
					for (var j = 1; j <= sqsize; j++)
					{
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
						power += GetPowerLevel(x + i, y + j);
					}

					powers[(x + 1, y + 1, sqsize)] = power;
				}
			}

			var max = powers.MaxBy(kvp => kvp.Value).First();

			Console.WriteLine(max.Key);

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

			Console.ReadLine();
		}

		//private static int Get2PowerLevel(int x, int y)
		//{
		//	return cache.GetOrAdd((x, y), () => CalcPowerLevel(x, y));
		//}

		private static int GetPowerLevel(int x, int y)
		{
			var rackid = x + 10;
			var power = rackid * y;
			power += _serialNumber;
			power *= rackid;
			power = power % 1000 / 100;
			return power - 5;
		}
	}
}