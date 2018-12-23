using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using Core;
using MoreLinq;

namespace Day_23
{
   
    public struct Point3 : IEquatable<Point3>
    {
        public static readonly Point3 Empty = new Point3(0, 0, 0);
        
        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsEmpty => this.Equals(Point3.Empty);
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public bool Equals(Point3 other)
        {
            return (X == other.X) && (Y == other.Y) && (Z == other.Z);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            return (obj is Point3 p) && Equals(p);
        }
    }

    public class Nanobot
    {
        public Point3 Position { get; }
        public int Radius { get; }

        public Nanobot(Point3 pos, int radius)
        {
            Position = pos;
            Radius = radius;
        }

        private static int ManhattanDist3D(Point3 a, Point3 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }

        public bool IsPointInRange(Point3 p)
        {
            return ManhattanDist3D(p, Position) <= Radius;
        }
    }

    class Program
    {
        private static List<Nanobot> nanobots;
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");
            var sw = new Stopwatch();
            sw.Start();

            var origin = new Point3(0, 0, 0);

            nanobots = input.Select(ParseLine).ToList();

            var largestRadius = nanobots.MaxBy(b => b.Radius).First();
            var inRange = nanobots.Count(b => largestRadius.IsPointInRange(b.Position));

            Console.WriteLine($"Part 1: {inRange} are in Range");

            // Gradient descent
            Point3 currentPoint = origin;

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

            var bestReception = AllPointsInDiamond((currentPoint.X, currentPoint.Y, currentPoint.Z, 17))
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

        private static Point3 GradientDescent(Point3 start,
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

        private static Point3 TranslateTowardsOrigin(Point3 p, int distance)
        {
            var x = p.X > 0 ? p.X - distance : p.X + distance;
            var y = p.Y > 0 ? p.Y - distance : p.Y + distance;
            var z = p.Z > 0 ? p.Z - distance : p.Z + distance;
            return new Point3(x, y, z);
        }

        private static IEnumerable<Point3> GetCoarseNeightbors(Point3 p, int coarsening)
        {
            yield return new Point3(p.X + coarsening, p.Y, p.Z);
            yield return new Point3(p.X - coarsening, p.Y, p.Z);
            yield return new Point3(p.X, p.Y + coarsening, p.Z);
            yield return new Point3(p.X, p.Y - coarsening, p.Z);
            yield return new Point3(p.X, p.Y, p.Z + coarsening);
            yield return new Point3(p.X, p.Y, p.Z - coarsening);
        }

        private static IEnumerable<Point3> GetAdjacentPoints(Point3 p)
        {
            return GetCoarseNeightbors(p, 1);
        }

        private static long CalcNormalizedDistance(Point3 point)
        {
            return nanobots.Sum(bot =>
            {
                long dist = ManhattanDist3D(point, bot.Position);
                return dist > bot.Radius ? (dist - bot.Radius) + 1000 : 0L;
            });
        }

        private static int CountNanobotsInRange(Point3 p)
        {
            return nanobots.Count(b => IsInRange(b, p));
        }

        private static bool IsInRange(Nanobot bot, Point3 point)
        {
            return ManhattanDist3D(point, bot.Position) <= bot.Radius;
        }

        private static int ManhattanDist3D(Nanobot a, Nanobot b)
        {
            return ManhattanDist3D(a.Position, b.Position);
        }

        private static int ManhattanDist3D(Point3 a, Point3 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }

        private static Nanobot ParseLine(string arg)
        {
            var ints = arg.ParseInts(4);
            return new Nanobot(new Point3(ints[0], ints[1], ints[2]), ints[3]);
        }
    }
}
