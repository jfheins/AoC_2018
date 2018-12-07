using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using MoreLinq;

namespace Day_07
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../demo.txt");



            var sw = new Stopwatch();
            sw.Start();

            var (time, path) = GetFinishTimeForSledge(input, 5, 60);

            Console.WriteLine($"Path: {path}");
            Console.WriteLine();

            Console.WriteLine($"Simulation Time: {time}"); //1276 too high

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");

            Console.ReadLine();
        }

        public static (int time, string path) GetFinishTimeForSledge(IEnumerable<string> input, int workerCount = 2, int stepBaseTime = 0)
        {

            var edges = input.Select(ParseLine).ToArray();
            var nodes = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).Distinct().ToArray();

            var finished = new HashSet<string>();
            var path = new List<string>();
            var remainingNodes = new HashSet<string>(nodes);
            var mapTargetToSources = edges.ToLookup(e => e.Target, e => e.Source);

            var workers = Enumerable.Repeat(0, workerCount).Select(_ => new Worker()).ToArray();
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
                    worker.FinishTime = time + stepBaseTime + (nextNode[0] - 'A' + 1);
                    availableWorkers.RemoveAt(0);
                }

                time = workers.Where(w => w.FinishTime > 0).MinBy(w => w.FinishTime).FirstOrDefault()?.FinishTime ?? time + 1;
            }

            return (time-1, string.Concat(path));
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
