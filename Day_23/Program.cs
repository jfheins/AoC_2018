using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Core;
using MoreLinq;

namespace Day_23
{
    class Program
    {
        private static List<(int x, int y, int z, int r)> nanobots;
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");
            var sw = new Stopwatch();
            sw.Start();

            var origin = (0, 0, 0);

            nanobots = input.Select(ParseLine).ToList();

            var largestRadius = nanobots.MaxBy(b => b.r).First();
            var inRange = nanobots.Count(b => ManhattanDist3D(b, largestRadius) <= largestRadius.r);

            Console.WriteLine($"Part 1: {inRange} are in Range");

            // Gradient descent
            (int x, int y, int z) currentPoint = origin;

            currentPoint = GradientDescent(currentPoint, 3200000);
            currentPoint = GradientDescent(currentPoint, 160000);
            currentPoint = GradientDescent(currentPoint, 8000);
            currentPoint = GradientDescent(currentPoint, 400);
            currentPoint = GradientDescent(currentPoint, 20);
            currentPoint = GradientDescent(currentPoint);

            Console.WriteLine($"Gradient descent landed at {currentPoint}");

            long bestscore = CalcNormalizedDistance(currentPoint);
            Console.WriteLine($"Part 2: Point {currentPoint} has cost of {bestscore} :-)");
            Console.WriteLine($"It is in Range of {CountNanobotsInRange(currentPoint)} bots.");

            var bestReception = AllPointsInDiamond((currentPoint.x, currentPoint.y, currentPoint.z, 17))
                .MaxBy(p => CountNanobotsInRange(p))
                .OrderBy(p => ManhattanDist3D(p, origin))
                .First();

            bestscore = CalcNormalizedDistance(bestReception);

            Console.WriteLine($"Part 2: Point {bestReception} has cost of {bestscore} :-)");
            Console.WriteLine($"It is in Range of {CountNanobotsInRange(bestReception)} bots.");
            Console.WriteLine($"Distance: {ManhattanDist3D(origin, currentPoint)}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static (int x, int y, int z) GradientDescent((int x, int y, int z) start,
            int coarsening = 1)
        {
            var currentPoint = start;
            long bestscore = long.MaxValue;
            while (true)
            {
                var gradients = GetCoarseNeightbors(currentPoint, coarsening)
                    .Select(p => (p, cost: CalcNormalizedDistance(p)))
                    .ToList();
                var next = gradients.MinBy(x => x.cost).First();
                if (next.cost >= bestscore)
                {
                    if (coarsening > 1)
                    {
                        return TranslateTowardsOrigin(currentPoint, 2*coarsening);
                    }
                    return currentPoint;
                }
                currentPoint = next.p;
                bestscore = next.cost;
            }
        }

        private static (int, int, int) TranslateTowardsOrigin((int x, int y, int z) p, int distance)
        {
            var x = p.x > 0 ? p.x - distance : p.x + distance;
            var y = p.y > 0 ? p.y - distance : p.y + distance;
            var z = p.z > 0 ? p.z - distance : p.z + distance;
            return (x, y, z);
        }

        private static IEnumerable<(int x, int y, int z)> GetCoarseNeightbors((int x, int y, int z) p, int coarsening)
        {
            yield return (p.x + coarsening, p.y, p.z);
            yield return (p.x - coarsening, p.y, p.z);
            yield return (p.x, p.y + coarsening, p.z);
            yield return (p.x, p.y - coarsening, p.z);
            yield return (p.x, p.y, p.z + coarsening);
            yield return (p.x, p.y, p.z - coarsening);
        }

        private static IEnumerable<(int x, int y, int z)> GetAdjacentPoints((int x, int y, int z) p)
        {
            return GetCoarseNeightbors(p, 1);
        }

        private static long CalcNormalizedDistance((int x, int y, int z) point)
        {
            return nanobots.Sum(bot =>
            {
                long dist = ManhattanDist3D((point.x, point.y, point.z, 0), bot);
                return dist > bot.r ? (dist - bot.r) + 1000 : 0L;
            });
        }

        private static int CountNanobotsInRange((int x, int y, int z) p)
        {
            return nanobots.Count(b => IsInRange(b, p));
        }

        private static bool IsInRange((int x, int y, int z, int r) bot, (int x, int y, int z) point)
        {
            return ManhattanDist3D((point.x, point.y, point.z, 0), bot) <= bot.r;
        }

        private static IEnumerable<(int x, int y, int z)> AllPointsInDiamond((int x, int y, int z, int r) bot)
        {
            var xRange = Enumerable.Range(bot.x - bot.r, 2 * bot.r + 1);
            var yRange = Enumerable.Range(bot.y - bot.r, 2 * bot.r + 1);
            var zRange = Enumerable.Range(bot.z - bot.r, 2 * bot.r + 1);
            return xRange.Cartesian(yRange, (x, y) => (x, y))
                .Cartesian(zRange, (t, z) => (t.x, t.y, z))
                .Where(p => IsInRange(bot, p));
        }

        private static int ManhattanDist3D((int x, int y, int z, int r) a, (int x, int y, int z, int r) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        private static int ManhattanDist3D((int x, int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        private static (int x, int y, int z, int r) ParseLine(string arg)
        {
            var ints = arg.ParseInts(4);
            return (ints[0], ints[1], ints[2], ints[3]);
        }
    }
}
