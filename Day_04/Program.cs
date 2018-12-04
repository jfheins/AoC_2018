using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

namespace Day_04
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines(@"../../../input.txt");

            var records = ProcessActions(input.Select(ParseLine)).ToList();
            var years = records.Select(r => r.DateTime.Year).Distinct().ToArray();
            var field = records.GroupBy(r => r.DateTime.DayOfYear).Select(GuardActionsToLine).ToList();



            Console.WriteLine(records[1]);

            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static (DateTime, string) ParseLine(string line)
        {
            // [1518-06-07 00:03] Guard #2789 begins shift
            var regex = new Regex(@"\[([\d- :]+)\] (.+)");

            var groups = regex.Match(line).Groups;
            Debug.Assert(groups.Count == 3);
            var date = DateTime.Parse(groups[1].Value);
            return (date, groups[2].Value);
        }

        private static IEnumerable<GuardAction> ProcessActions(
            IEnumerable<(DateTime time, string action)> records)
        {
            var ordered = records.OrderBy(x => x.time);
            var guardNumber = -1;
            // Guard #99 begins shift
            var guardregex = new Regex(@"Guard #(\d+) begins shift");

            foreach (var tuple in ordered)
                if (tuple.action == "falls asleep")
                {
                    yield return new GuardAction(guardNumber, tuple.time, GuardConsciousness.FallsAsleep);
                }
                else if (tuple.action == "wakes up")
                {
                    yield return new GuardAction(guardNumber, tuple.time, GuardConsciousness.FallsAsleep);
                }
                else if (guardregex.IsMatch(tuple.action))
                {
                    var number = guardregex.Match(tuple.action).Groups[1].Value;
                    guardNumber = int.Parse(number);
                }
        }

        private static string GuardActionsToLine(IEnumerable<GuardAction> actionsInDay)
        {
            var arr = new string('?', 60).ToCharArray();
            var actions = actionsInDay.ToLookup(a => a.Minute);
            var signal = '.';

            for (int i = 0; i < 60; i++)
            {
                if (actions[i].Any())
                    signal = actions[i].First().Consciousness == GuardConsciousness.FallsAsleep ? '#' : '.';
                
                arr[i] = signal;
            }

            return string.Concat(arr);
        }
    }

    public enum GuardConsciousness
    {
        FallsAsleep,
        WakesUp
    }

    public struct GuardAction
    {
        public GuardConsciousness Consciousness { get; }
        public int Minute { get; }
        public DateTime DateTime { get; }
        public int Number { get; }

        public GuardAction(int number, DateTime time, GuardConsciousness consciousness)
        {
            Number = number;
            DateTime = time;
            Minute = time.Minute;
            Consciousness = consciousness;
        }
    }
}
