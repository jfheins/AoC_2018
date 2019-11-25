using Microsoft.VisualStudio.TestTools.UnitTesting;

using Core;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Core.Test
{
    [TestClass]
    public class BfsTests
    {
        public BreadthFirstSearch<Point, Size> Setup()
        {
            static IEnumerable<Point> Expander(Point p)
            {
                yield return p + new Size(1, 0);
                yield return p + new Size(0, 1);
            }

            return new BreadthFirstSearch<Point, Size>(EqualityComparer<Point>.Default, Expander);
        }

        [TestMethod]
        public void FindsInitialNode()
        {
            var bfs = new BreadthFirstSearch<int, int>(EqualityComparer<int>.Default, _ => Enumerable.Empty<int>());
            var result = bfs.FindAll(0, x => true);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].Length);
            Assert.AreEqual(1, result[0].Steps.Length);
            Assert.AreEqual(0, result[0].Steps[0]);
        }

        [TestMethod]
        public void ReturnsEmptySetIfNothingFound()
        {
            var bfs = new BreadthFirstSearch<int, int>(EqualityComparer<int>.Default, _ => Enumerable.Empty<int>());
            var result = bfs.FindAll(0, x => false);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindsFirstTarget()
        {
            var bfs = new BreadthFirstSearch<int, int>(EqualityComparer<int>.Default, x => new[] { x * 2, x * 3 });
            var result = bfs.FindFirst(1, x => x == 192);

            Assert.AreEqual(1, result.Steps.First());
            Assert.AreEqual(192, result.Steps.Last());
            Assert.AreEqual(7, result.Length);
        }

        [TestMethod]
        public void FindTargetSetInInfiniteGraph()
        {
            var bfs = new BreadthFirstSearch<int, int>(EqualityComparer<int>.Default, x => new[] { x * 2, x * 3 });

            var result = bfs.FindAll(1, x => x < 30, null, 12);

            var expected = new[] { 1, 2, 4, 8, 16, 3, 6, 12, 24, 9, 18, 27 };
            Assert.AreEqual(12, result.Count);
            foreach (var target in expected)
            {
                Assert.IsTrue(result.Any(p => p.Target == target));
            }
        }

        [TestMethod]
        public void FindAllTargetsInFiniteGraph()
        {
            var bfs = new BreadthFirstSearch<int, int>(
                EqualityComparer<int>.Default,
                (int x) => (x > 100) ? Enumerable.Empty<int>() : new[] { x * 2, x * 3 });

            var result = bfs.FindAll(1, x => x < 30);

            var expected = new[] { 1, 2, 4, 8, 16, 3, 6, 12, 24, 9, 18, 27 };
            Assert.AreEqual(12, result.Count);
            foreach (var target in expected)
            {
                Assert.IsTrue(result.Any(p => p.Target == target));
            }
        }

        [TestMethod]
        public void FindAllTargetsWithDistance()
        {
            var bfs = new BreadthFirstSearch<int, int>(
                EqualityComparer<int>.Default,
                (int x) => (x > 100) ? Enumerable.Empty<int>() : new[] { x * 2, x * 3 });

            var result = bfs.FindAll2(1, node => node.Distance > 4);

            Assert.AreEqual(13, result.Count);
        }

        [TestMethod]
        public void FindAllTargetsWithPredecessor()
        {
            var bfs = new BreadthFirstSearch<int, int>(EqualityComparer<int>.Default, x => new[] { x * 2, x * 3 });

            var result = bfs.FindAll2(1, node => node.Predecessor?.Current == 64, null, 2);

            Assert.IsTrue(result.All(x => x.Target == 128 || x.Target == 192));
        }
    }
}
