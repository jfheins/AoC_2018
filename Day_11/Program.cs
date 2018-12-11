using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

                var allRects = GetAllSquares(gridsize, sqsize);
                Console.WriteLine($"{allRects.Count} squares");

                foreach (var rect in allRects)
                {
                    powers[(rect.X, rect.Y, sqsize)] = CalcPowerLevelForRect(rect);
                }
            }

            var max = powers.MaxBy(kvp => kvp.Value).First();

            Console.WriteLine(max.Key);

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static List<Rectangle> GetAllSquares(int gridSize, int squareSize)
        {
            var xRange = Enumerable.Range(1, gridSize - squareSize + 1);
            var yRange = Enumerable.Range(1, gridSize - squareSize + 1);

            return xRange.Cartesian(yRange, (x, y) => new Rectangle(x, y, squareSize, squareSize)).ToList();
        }

        private static int CalcPowerLevelForRect(Rectangle rect)
        {
            var sum = 0;
            for (var x = rect.Left; x < rect.Right; x++)
            {
                for (var y = rect.Top; y < rect.Bottom; y++)
                {
                    sum += GetPowerLevelForCell(x, y);
                }
            }

            return sum;
        }

        private static int CalcPowerLevelForRect(int x, int y, int size)
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
