﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;

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
            
            var finished = new HashSet<string>();
            var path = new List<string>();
            var remainingNodes = new HashSet<string>(nodes);
            var mapTargetToSources = edges.ToLookup(e => e.Target, e => e.Source);

            var workers = new [] {new Worker(), new Worker()};
            var time = 0;

            while (remainingNodes.Count > 0 || workers.Any(w => w.WorksOn != null))
            {
                var finishedWork = workers.Select(w => w.DoStep(time)).WhereNotNull().ToList();
                path.AddRange(finishedWork);
                finished.UnionWith(finishedWork);

                var availableWorkers = workers.Where(w => w.WorksOn == null).ToList();
                var nextNodes = FirstByAlphabet(availableWorkers.Count,
                    remainingNodes
                        .Where(n => mapTargetToSources[n].All(finished.Contains)));

                foreach (var nextNode in nextNodes)
                {
                    remainingNodes.Remove(nextNode);
                    var worker = availableWorkers[0];
                    worker.WorksOn = nextNode;
                    worker.FinishTime = time + 60 + (nextNode[0] - 'A' + 1);
                    availableWorkers.RemoveAt(0);
                }

                time++;
            }

            Console.WriteLine("Path:");
            foreach (var node in path)
                Console.Write(node);
            Console.WriteLine();

            Console.WriteLine($"Simulation Time: {time-1}");

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        private static T FirstByAlphabet<T>(IEnumerable<T> collection)
        {
            return collection.OrderBy(x => x).First();
        }

        private static IEnumerable<T> FirstByAlphabet<T>(int count, IEnumerable<T> collection)
        {
            return collection.OrderBy(x => x).Take(count);
        }

        private static Edge ParseLine(string line)
        {
            // Step C must be finished before step A can begin.
            var regex = new Regex(@"Step (\w+) must be finished before step (\w+) can begin");
            var groups = regex.Match(line).Groups;
            return new Edge {Source = groups[1].Value, Target = groups[2].Value};
        }

        private class Worker
        {
            public Worker()
            {
                FinishTime = -1;
            }

            public string WorksOn { get; set; }
            public int FinishTime { get; set; }

            public string DoStep(int currentTime)
            {
                if ( WorksOn == null || currentTime < FinishTime)
                    return null;

                var result = WorksOn;
                WorksOn = null;
                FinishTime = -1;
                return result;
            }
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