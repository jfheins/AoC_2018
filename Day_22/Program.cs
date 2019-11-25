using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Core;

namespace Day_22
{
    public class Program
    {
        private static Point origin;
        private static Point target;
        private static int depth;

        private static void Main(string[] args)
        {
            depth = 5913;
            origin = new Point(0, 0);
            target = new Point(8, 701);

            var sw = new Stopwatch();
            sw.Start();

            var risk = 0;
            for (var x = 0; x <= target.X; x++)
            {
                for (var y = 0; y <= target.Y; y++)
                    risk += (GetErosionLevelAt(new Point(x, y)) % 3);
            }

            Console.WriteLine($"Part 1: {risk}"); // 6256

            var search = new DijkstraSearch<ValueTuple<Point, Tool>, Direction>(
                EqualityComparer<(Point, Tool)>.Default,
                Expander);

            var path = search.FindFirst(
                (origin, Tool.Torch),
                tuple => tuple.Item1 == target && tuple.Item2 == Tool.Torch);


            Console.WriteLine($"Part 2: {path.Cost}"); // 973

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static readonly Dictionary<Point, int> erosionLevelCache = new Dictionary<Point, int>();
        private static readonly Size left = new Size(-1, 0);
        private static readonly Size up = new Size(0, -1);

        private static int GetErosionLevelAt(Point p)
        {
            return erosionLevelCache.GetOrAdd(p, () =>
            {
                int geoIndex = 0;
                if (p == origin || p == target)
                {
                    geoIndex = 0;
                }
                else if (p.Y == 0)
                {
                    geoIndex = p.X * 16807;
                }
                else if (p.X == 0)
                {
                    geoIndex = p.Y * 48271;
                }
                else
                {
                    geoIndex = GetErosionLevelAt(p + left) * GetErosionLevelAt(p + up);
                }
                return (geoIndex + depth) % 20183;
            });
        }

        private static IEnumerable<((Point, Tool) node, float cost)> Expander((Point pos, Tool tool) state)
        {
            var newPosition = new List<Point>();

            if (state.pos.X > 0)
            {
                newPosition.Add(state.pos + _mapDirectionToSize[Direction.Left]);
            }
            if (state.pos.Y > 0)
            {
                newPosition.Add(state.pos + _mapDirectionToSize[Direction.Up]);
            }
            newPosition.Add(state.pos + _mapDirectionToSize[Direction.Right]);
            newPosition.Add(state.pos + _mapDirectionToSize[Direction.Down]);

            foreach (var t in ValidToolsAt(state.pos).ExceptFor(state.tool))
            {
                yield return ((state.pos, t), 7);
            }

            foreach (var point in newPosition)
            {
                if (IsToolValidAt(state.tool, point))
                {
                    yield return ((point, state.tool), 1);
                }
            }
        }

        private static IEnumerable<Tool> ValidToolsAt(Point p)
        {
            switch (GetErosionLevelAt(p) % 3)
            {
                case 0:
                    return new[] { Tool.Torch, Tool.Gear };
                case 1:
                    return new[] { Tool.Gear, Tool.None };
                case 2:
                    return new[] { Tool.Torch, Tool.None };
            }

            return null;
        }

        private static bool IsToolValidAt(Tool t, Point p)
        {
            return GetErosionLevelAt(p) % 3 != (int)t;
        }

        private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
        {
            {Direction.Left, new Size(-1, 0)},
            {Direction.Up, new Size(0, -1)},
            {Direction.Right, new Size(1, 0)},
            {Direction.Down, new Size(0, 1)}
        };
    }

    public enum Tool
    {
        None = 0,
        Torch = 1,
        Gear = 2
    }

    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }
}