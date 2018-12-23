using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using C5;
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

        public Point3 TranslateBy(int dx, int dy, int dz)
        {
            return new Point3(X + dx, Y + dy, Z + dz);
        }
    }

    public class Cube : IComparable<Cube>
    {
        public Point3 BottomLeft { get; set; }
        public Point3 TopRight => BottomLeft.TranslateBy(SideLength, SideLength, SideLength);
        public int SideLength { get; set; }
        public IPriorityQueueHandle<Cube> Handle { get; set; } = null;

        public int NanobotsInRange { get; set; } = 0;

        public Cube(Point3 root, int side)
        {
            BottomLeft = root;
            SideLength = side;
        }

        public int CompareTo(Cube other)
        {
            return NanobotsInRange.CompareTo(other.NanobotsInRange);
        }

        public Point3 Center => BottomLeft.TranslateBy(SideLength/2, SideLength/2, SideLength/2);

        public IEnumerable<Point3> GetEdgePoints()
        {
            for (int x = BottomLeft.X; x < TopRight.X; x++)
            {
                yield return new Point3(x, BottomLeft.Y, BottomLeft.Z);
                yield return new Point3(x, BottomLeft.Y, TopRight.Z - 1);
                yield return new Point3(x, TopRight.Y - 1, BottomLeft.Z);
                yield return new Point3(x, TopRight.Y - 1, TopRight.Z - 1);
            }
            for (int y = BottomLeft.Y; y < TopRight.Y; y++)
            {
                yield return new Point3(BottomLeft.X, y, BottomLeft.Z);
                yield return new Point3(BottomLeft.X, y, TopRight.Z - 1);
                yield return new Point3(TopRight.X - 1, y, BottomLeft.Z);
                yield return new Point3(TopRight.X - 1, y, TopRight.Z - 1);
            }
            for (int z = BottomLeft.Z; z < TopRight.Z; z++)
            {
                yield return new Point3(BottomLeft.X, BottomLeft.Y, z);
                yield return new Point3(BottomLeft.X, TopRight.Y - 1, z);
                yield return new Point3(TopRight.X - 1, BottomLeft.Y, z);
                yield return new Point3(TopRight.X - 1, TopRight.Y - 1, z);
            }
        }

        internal static IEnumerable<Cube> Split(Cube c)
        {
            var halfSide = c.SideLength / 2;
            var halfX = c.BottomLeft.X + halfSide;
            var halfY = c.BottomLeft.Y + halfSide;
            var halfZ = c.BottomLeft.Z + halfSide;

            yield return new Cube(c.BottomLeft, c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(halfSide, 0, 0), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(0, halfSide, 0), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(0, 0, halfSide), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(halfSide, halfSide, 0), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(halfSide, 0, halfSide), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(0, halfSide, halfSide), c.SideLength / 2);
            yield return new Cube(c.BottomLeft.TranslateBy(halfSide, halfSide, halfSide), c.SideLength / 2);
        }

        public bool Contains (Point3 p)
        {
            return (p.X >= BottomLeft.X && p.X < TopRight.X)
                && (p.Y >= BottomLeft.Y && p.Y < TopRight.Y)
                && (p.Z >= BottomLeft.Z && p.Z < TopRight.Z);
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
            var minx = nanobots.Min(b => b.Position.X);
            var miny = nanobots.Min(b => b.Position.Y);
            var minz = nanobots.Min(b => b.Position.Z);
            var side = 1 << 30;
            var initialQuad = new Cube(new Point3(minx, miny, minz), side);

            var prioQueue = new C5.IntervalHeap<Cube>();

            initialQuad.NanobotsInRange = nanobots.Count(b => IntersectCubeWithOctogon(initialQuad, b));
            prioQueue.Add(initialQuad);

            var seenmax = 0;
            var maxPosition = Point3.Empty;
            while (true)
            {
                var next = prioQueue.DeleteMax();
                
                if (next.SideLength == 1)
                {
                    if (next.NanobotsInRange > seenmax)
                    { 
                        seenmax = next.NanobotsInRange;
                        maxPosition = next.BottomLeft;
                    }
                    else
                        break;
                }

                var smallerCubes = Cube.Split(next);
                foreach (var cube in smallerCubes)
                {
                    cube.NanobotsInRange = nanobots.Count(b => IntersectCubeWithOctogon(cube, b));
                    prioQueue.Add(initialQuad);
                }
            }

            var bestReception = maxPosition;
            var bestscore = CalcNormalizedDistance(bestReception);

            Console.WriteLine($"Part 2: Point {bestReception} has cost of {bestscore} :-)");
            Console.WriteLine($"It is in Range of {CountNanobotsInRange(bestReception)} bots.");
            Console.WriteLine($"Distance: {ManhattanDist3D(origin, maxPosition)}");

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

        private static bool IntersectCubeWithOctogon(Cube c, Nanobot bot)
        {
            var dist = ManhattanDist3D(c.Center, bot.Position);
            var halfLength = c.SideLength / 2;
            if (dist > bot.Radius + (halfLength * 3))
            {
                return false;
            }
            if (dist < bot.Radius + halfLength)
            {
                return false;
            }
            if (c.Contains(bot.Position))
            {
                return true;
            }
            foreach (var p in c.GetEdgePoints())
            {
                if (bot.IsPointInRange(p))
                {
                    return true;
                }
            }
            return false;
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
