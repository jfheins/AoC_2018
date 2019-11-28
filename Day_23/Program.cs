using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Core;
using MoreLinq;

namespace Day_23
{

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
            var nanoBotsPerCube = new Dictionary<Cube, int>();

            nanoBotsPerCube[initialQuad] = nanobots.Count(b => IntersectCubeWithOctogon(initialQuad, b));
            _ = prioQueue.Add(initialQuad);

            var seenmax = 0;
            var maxPosition = Point3.Empty;
            while (true)
            {
                var next = prioQueue.DeleteMax();

                if (next.SideLength == 1)
                {
                    if (nanoBotsPerCube[next] > seenmax)
                    {
                        seenmax = nanoBotsPerCube[next];
                        maxPosition = next.BottomLeft;
                    }
                    else
                        break;
                }

                var smallerCubes = Cube.Split(next);

                foreach (var cube in smallerCubes)
                {
                    nanoBotsPerCube[cube] = nanobots.Count(b => IntersectCubeWithOctogon(cube, b));
                    _ = prioQueue.Add(cube);
                }
            }

            var bestReception = maxPosition;
            var bestscore = CalcNormalizedDistance(bestReception);

            Console.WriteLine($"Part 2: Point with best reception is {bestReception} :-)");
            Console.WriteLine($"It is in Range of {CountNanobotsInRange(bestReception)} bots.");
            Console.WriteLine($"Distance from origin: {ManhattanDist3D(origin, maxPosition)}");

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
                        return TranslateTowardsOrigin(currentPoint, 2 * coarsening);
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
                return true;
            }
            if (c.Contains(bot.Position))
            {
                return true;
            }
            foreach (var edge in c.GetEdges())
            {
                if (bot.IsPointInRange(edge.ClosestPointTo(bot.Position)))
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
