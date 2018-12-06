using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using static MoreLinq.Extensions.PairwiseExtension;
using static MoreLinq.Extensions.MaxByExtension;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace Day_06
{
    class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../example.txt");


            var nodes = input.Select(ParseLine).ToArray();

            var sw = new Stopwatch();
            sw.Start();

            var innerNodes = GetInnerNodes(nodes);

            var bmp = new int[400, 400];

            for (int x = 0; x < bmp.Length; x++)
            {
                for (int y = 0; y < bmp.Length; y++)
                {
                    var dist = nodes.Select(c => (c, GetManhattanDistance(x, y, c))).MaxBy(tuple => tuple.Item2);
                    if (dist.Count() == 1 && innerNodes.Contains(dist.First().Item1))
                    {
                        bmp[x, y] = dist.First();
                    }
                }
            }


            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static int GetManhattanDistance(int x, int y, Node vec)
        {
            var xDist = (int)Math.Abs(Math.Round(x - vec.Coords.X));
            var yDist = (int)Math.Abs(Math.Round(y - vec.Coords.Y));
            return xDist + yDist;
        }

        private static List<Node> GetInnerNodes(Node[] coords)
        {
            var innerNodes = new List<Node>();

            for (int i = 0; i < coords.Length; i++)
            {
                var currentNode = coords[i];
                var angles = coords.Where((t, j) => i != j)
                    .Select(otherNode => otherNode.Coords - currentNode.Coords)
                    .Select(differenceVector => Math.Atan2(differenceVector.Y, differenceVector.X))
                    .Select(NormalizeAngle)
                    .OrderBy(x => x)
                    .ToList();

                var differenceAngles = angles.Pairwise((a, b) => b - a)
                    .Append(2 * Math.PI + angles[0] - angles.Last())
                    .ToList();
                if (!differenceAngles.Any(angle => angle >= Math.PI))
                {
                    // Node is on the inside
                    innerNodes.Add(currentNode);
                }
            }

            return innerNodes;
        }

        private static Node ParseLine(string arg, int idx)
        {
            var array = arg.Split(',').Select(x => x.Trim()).Select(int.Parse).ToArray();
            return new Node {Coords = new Vector2(array[0], array[1]), Name = idx.ToString()};
        }

        private static double NormalizeAngle(double angle)
        {
            const double twoPi = Math.PI * 2;
            var result = (angle % twoPi + twoPi) % twoPi;
            return result <= Math.PI ? result : result - twoPi;
        }
    }

    public class Node
    {
        public Vector2 Coords { get; set; }
        public string Name { get; set; }
    }
}
