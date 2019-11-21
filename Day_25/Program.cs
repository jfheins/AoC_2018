using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_25
{
    public class Program
    {
        private static List<List<Point4>> Constellations;

        static void Main()
        {
            var input = File.ReadAllLines(@"../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            Constellations = new List<List<Point4>>();

            foreach (var line in input)
            {
                Point4 newPoint = PointFromLine(line);
                var matching = FindIntersectingConstellations(newPoint, Constellations).ToList();

                if (matching.Count == 0)
                {
                    Constellations.Add(new List<Point4> { newPoint });
                }
                else if (matching.Count == 1)
                {
                    matching[0].Add(newPoint);
                }
                else
                {
                    _ = Constellations.RemoveAll(it => matching.Contains(it));
                    var merged = matching.SelectMany(x => x).ToList();
                    merged.Add(newPoint);
                    Constellations.Add(merged);
                }
            }

            sw.Stop();

            Console.WriteLine($"Part 1: {Constellations.Count} constellations");

            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static IEnumerable<List<Point4>> FindIntersectingConstellations(Point4 point, List<List<Point4>> constellations)
        {
            foreach (var constellation in constellations)
            {
                if (constellation.Any(p => ManhattanDist4D(p, point) <= 3))
                {
                    yield return constellation;
                }
            }
        }

        private static Point4 PointFromLine(string line)
        {
            var num = line.ParseInts(4);
            return new Point4(num[0], num[1], num[2], num[3]);
        }
        

        private static int ManhattanDist4D(Point4 a, Point4 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z) + Math.Abs(a.T - b.T);
        }
    }
}
