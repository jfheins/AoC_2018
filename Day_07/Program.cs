using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_07
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");


            var edges = input.Select(ParseLine).ToArray();
            var nodes = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).Distinct().ToArray();

            var sw = new Stopwatch();
            sw.Start();

            var startNode = FirstByAlphabet(nodes.Where(node => edges.All(e => e.Target != node)));

            var visited = new HashSet<string> {startNode};
            var path = new List<string> {startNode};
            var remainingNodes = new HashSet<string>(nodes.Except(visited));
            var mapTargetToSources = edges.ToLookup(e => e.Target, e => e.Source);

            while (remainingNodes.Count > 0)
            {
                var nextNode = FirstByAlphabet(
                    remainingNodes
                        .Where(n => mapTargetToSources[n].All(visited.Contains)));
            
                path.Add(nextNode);
                visited.Add(nextNode);
                remainingNodes.Remove(nextNode);
            }

            Console.WriteLine("Path:");
            foreach (var node in path)
                Console.Write(node);

            Console.WriteLine();
            Console.WriteLine("Not reachable:");
            foreach (var node in remainingNodes)
                Console.Write(node + " ");

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static T FirstByAlphabet<T>(IEnumerable<T> collection)
        {
            return collection.OrderBy(x => x).First();
        }

        private static Edge ParseLine(string line)
        {
            // Step C must be finished before step A can begin.
            var regex = new Regex(@"Step (\w+) must be finished before step (\w+) can begin");
            var groups = regex.Match(line).Groups;
            return new Edge {Source = groups[1].Value, Target = groups[2].Value};
        }

        private struct Edge
        {
            public string Source { get; set; }
            public string Target { get; set; }

            public override string ToString()
            {
                return $"{Source} before {Target}";
            }
        }
    }
}
