using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                Console.WriteLine($"Squaresize: {sqsize}");
                var yRange = Enumerable.Range(1, gridsize - sqsize + 1);
                var xRange = Enumerable.Range(1, gridsize - sqsize + 1);

                var allCoords = xRange.Cartesian(yRange, (x, y) => (x, y));

                for (var x = 0; x < gridsize - sqsize + 1; x++)
                {
                    for (var y = 0; y < gridsize - sqsize + 1; y++)
                    {
                        powers[(x + 1, y + 1, sqsize)] = CalcPowerLevelForSquare(x, y, sqsize);
                    }
                }
            }

            var max = powers.MaxBy(kvp => kvp.Value).First();

            Console.WriteLine(max.Key);

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static int CalcPowerLevelForSquare(int x, int y, int size)
        {
            var power = 0;
            for (var i = 1; i <= size; i++)
            {
                for (var j = 1; j <= size; j++)
                {
                    power += GetPowerLevelForCell(x + i, y + j);
                }
            }

            return power;
        }

        private static int GetPowerLevelForCell(int x, int y)
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
