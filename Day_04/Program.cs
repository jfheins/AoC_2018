using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;

namespace Day_04
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var input = File.ReadAllLines(@"../../../input.txt");

            var records = ProcessActions(input.Select(ParseLine)).ToList();
            var activityPerGuard = records.GroupBy(r => r.Number).Select(GuardActionsToStatus).ToList();

            var mostTiredGuard = activityPerGuard.MaxBy(s => s.AsleepMinutes.Values.Sum()).First();
            var mostSleptMinute = mostTiredGuard.AsleepMinutes.MaxBy(kvp => kvp.Value).First().Key;

            Console.WriteLine($"Guard #{mostTiredGuard.Number} sleeps the most.");
            Console.WriteLine($"Most slept minute: {mostSleptMinute}");
            Console.WriteLine($"Part 1 answer: {mostSleptMinute * mostTiredGuard.Number}");

            // Different Strategy!
            mostTiredGuard = activityPerGuard.MaxBy(activity => activity.AsleepMinutes.Values.Max()).First();
            mostSleptMinute = mostTiredGuard.AsleepMinutes.MaxBy(kvp => kvp.Value).First().Key;

            Console.WriteLine($"Guard #{mostTiredGuard.Number} sleeps often in minute {mostSleptMinute}.");
            Console.WriteLine($"Part 2 answer: {mostSleptMinute * mostTiredGuard.Number}");

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
                    yield return new GuardAction(guardNumber, tuple.time, GuardConsciousness.WakesUp);
                }
                else if (guardregex.IsMatch(tuple.action))
                {
                    var number = guardregex.Match(tuple.action).Groups[1].Value;
                    guardNumber = int.Parse(number);
                }
        }

        // Receives a number of action for one guard
        private static GuardStatus GuardActionsToStatus(IGrouping<int, GuardAction> actionsOfGuard)
        {
            var sleepingMinutes = Enumerable.Range(0, 60).ToDictionary(x => x, x => 0);

            var actions = actionsOfGuard.ToLookup(a => a.DateTime.DayOfYear);
            foreach (var day in actions)
            {
                var isAsleep = false;
                var todayActions = day.ToLookup(a => a.Minute);
                for (var i = 0; i < 60; i++)
                {
                    if (todayActions[i].Any())
                        isAsleep = todayActions[i].First().Consciousness == GuardConsciousness.FallsAsleep;

                    if (isAsleep)
                        sleepingMinutes[i]++;
                }
            }

            return new GuardStatus(actionsOfGuard.Key, sleepingMinutes);
        }
    }

    public enum GuardConsciousness
    {
        FallsAsleep,
        WakesUp
    }

    public struct GuardStatus
    {
        public int Number { get; }

        /// <summary>
        ///     Maps the minute of the day to the number of days this guard sleeps
        /// </summary>
        public Dictionary<int, int> AsleepMinutes { get; }

        public GuardStatus(int number, Dictionary<int, int> asleepMinutes)
        {
            Number = number;
            AsleepMinutes = asleepMinutes;
        }
    }

    public struct GuardAction
    {
        public int Minute { get; }
        public DateTime DateTime { get; }
        public int Number { get; }
        public GuardConsciousness Consciousness { get; }

        public GuardAction(int number, DateTime time, GuardConsciousness consciousness)
        {
            Number = number;
            DateTime = time;
            Minute = time.Minute;
            Consciousness = consciousness;
        }

        public override string ToString()
        {
            return $"Guard #{Number} on {DateTime} does {Consciousness}";
        }
    }
}
