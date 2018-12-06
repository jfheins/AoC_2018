using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using static MoreLinq.Extensions.PairwiseExtension;
using static MoreLinq.Extensions.MaxByExtension;
using static MoreLinq.Extensions.MinByExtension;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace Day_06
{
    class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");


            var nodes = input.Select((line, idx) => ParseLine(line, idx, 5)).ToArray();

            var sw = new Stopwatch();
            sw.Start();

            var innerNodes = GetInnerNodes(nodes);
            var size = 480;
            var bmp = new int[size, size];


            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var dist = nodes.Select(c => (c, GetManhattanDistance(x, y, c))).MinBy(tuple => tuple.Item2);
                    if (dist.Count() == 1)
                    {
                        bmp[x, y] = dist.First().Item1.Index;
                    }
                    else
                    {
                        bmp[x, y] = -1;
                    }
                }
            }
            var edgeNodes = new HashSet<int>();
            for (int y = 0; y < size; y++)
            {
                edgeNodes.Add(bmp[0, y]);
                edgeNodes.Add(bmp[size - 1, y]);
            }
            for (int x = 0; x < size; x++)
            {
                edgeNodes.Add(bmp[x, 0]);
                edgeNodes.Add(bmp[x, size - 1]);
            }
            //for (int y = 0; y < size; y++)
            //{
            //    for (int x = 0; x < size; x++)
            //    {
            //        Console.Write($"{bmp[x, y],3:##0}");
            //    }

            //    Console.WriteLine();
            //}


            var histogram = bmp.Cast<int>()
                .ToLookup(x => x)
                .Select(group => (index: group.Key, count: group.Count()))
                .ToList();
            
            var maxxed = histogram
                .Where(t => !edgeNodes.Contains(t.index))
                .Where(x => x.index > -1)
                .MaxBy(x => x.count).First();

            Console.WriteLine($"Index {maxxed.index} occurs {maxxed.count} times."); //6822 too high

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

        private static Node ParseLine(string arg, int idx, int offset = 0)
        {
            var array = arg.Split(',').Select(x => x.Trim()).Select(int.Parse).ToArray();
            return new Node
            {
                Coords = new Vector2(array[0]+ offset, array[1]+ offset),
                Index = idx,
                Name = ((char)('A'+idx)).ToString()
            };
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
        public int Index { get; set; }
        public string Name { get; set; }
    }
}
