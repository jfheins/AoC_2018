using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using static MoreLinq.Extensions.PairwiseExtension;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace Day_06
{
    class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../example.txt");


            var coords = input.Select(ParseLine).ToArray();

            var sw = new Stopwatch();
            sw.Start();

            var innerNodes = new List<Vector2>();

            for (int i = 0; i < coords.Length; i++)
            {
                var currentNode = coords[i];
                var angles = coords.Where((t, j) => i != j)
                    .Select(otherNode => otherNode - currentNode)
                    .Select(differenceVector => Math.Atan2(differenceVector.Y, differenceVector.X))
                    .Select(NormalizeAngle)
                    .OrderBy(x => x)
                    .ToList();

                var differenceAngles = angles.Pairwise((a, b) => b - a)
                    .Append(2*Math.PI + angles[0] - angles.Last())
                    .ToList();
                if (!differenceAngles.Any(angle => angle >= Math.PI))
                {
                    // Node is on the inside
                    innerNodes.Add(currentNode);
                }
            }
            

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static Vector2 ParseLine(string arg)
        {
            var array = arg.Split(',').Select(x => x.Trim()).Select(int.Parse).ToArray();
            return new Vector2(array[0], array[1]);
        }

        private static double NormalizeAngle(double angle)
        {
            const double twoPi = Math.PI * 2;
            var result = (angle % twoPi + twoPi) % twoPi;
            return result <= Math.PI ? result : result - twoPi;
        }
    }
}
