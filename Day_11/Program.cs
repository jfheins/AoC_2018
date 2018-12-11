using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Day_11
{
	internal class Program
	{
		private static int _serialNumber = 0;

		public static void Main(string[] args)
		{
			_serialNumber = 7315;
			var gridsize = 300;

			var powers = new Dictionary<(int, int, int), int>();

			for (int x = 0; x < gridsize-2; x++)
			{
				for (int y = 0; y < gridsize-2; y++)
				{
					var sqsize = 3;
					var power = 0;
					for (int i = 1; i <= sqsize; i++)
					{
						for (int j = 1; j <= sqsize; j++)
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
					}
					powers[(x + 1, y + 1, sqsize)] = power;
				}
			}

			var max = powers.MaxBy(kvp => kvp.Value).First();

			Console.WriteLine(max.Key);

			Console.ReadLine();
		}

		private static int GetPowerLevel(int x, int y)
		{
			var rackid = x + 10;
			var power = rackid * y;
			power += _serialNumber;
			power *= rackid;
			power = (power % 1000) / 100;
			return power - 5;
		}
	}
}