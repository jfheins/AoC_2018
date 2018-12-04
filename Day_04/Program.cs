using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
            var activityPerDay = records.GroupBy(r => r.DateTime.DayOfYear).Select(GuardActionsToStatus).ToList();

            List<(int guard, int sum)> part1 = activityPerDay.GroupBy(f => f.Number)
                .Select(group =>
                (guard: @group.Key, sum: @group
                    .Sum(status => status.AsleepMinutes.Count))).ToList();

            var (mostTiredGuard, sleepingMinutes) = part1.MaxBy(x => x.sum).Single();
            Console.WriteLine($"{mostTiredGuard} sleeps {sleepingMinutes} min.");

            var mostSleptMinute = activityPerDay.Where(g => g.Number == mostTiredGuard)
                .SelectMany(g => g.AsleepMinutes)
                .GroupBy(x => x)
                .OrderByDescending(gr => gr.Count())
                .First().Key;

            Console.WriteLine($"Most slept minute: {mostSleptMinute}");
            Console.WriteLine($"Part 1 answer: {mostSleptMinute * mostTiredGuard}");

            var actionPerGuard = activityPerDay.ToLookup(s => s.Number);
            var maxOccurence = 0;
            var sleepyGuard = 0;
            mostSleptMinute = 0;
            foreach (var guardActions in actionPerGuard)
            {
                var mostFrequentGroup = guardActions
                    .SelectMany(s => s.AsleepMinutes)
                    .GroupBy(x => x)
                    .OrderByDescending(gr => gr.Count())
                    .First();
                if (mostFrequentGroup.Count() > maxOccurence)
                {
                    maxOccurence = mostFrequentGroup.Count();
                    sleepyGuard = guardActions.Key;
                    mostSleptMinute = mostFrequentGroup.Key;
                }
            }

            Console.WriteLine($"Guard {sleepyGuard} sleeps often in minute {mostSleptMinute}. ({maxOccurence} times)");
            Console.WriteLine($"Part 2 answer: {mostSleptMinute * sleepyGuard}");

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

        private static GuardStatus GuardActionsToStatus(IEnumerable<GuardAction> actionsInDay)
        {
            var sleepingMinutes = new List<int>();
            var actions = actionsInDay.ToLookup(a => a.Minute);
            var isAsleep = false;

            for (var i = 0; i < 60; i++)
            {
                if (actions[i].Any())
                    isAsleep = actions[i].First().Consciousness == GuardConsciousness.FallsAsleep;

                if (isAsleep)
                    sleepingMinutes.Add(i);
            }

            var firstAction = actionsInDay.First();
            return new GuardStatus(firstAction.Number, firstAction.DateTime.Date, sleepingMinutes);
        }
    }

    public enum GuardConsciousness
    {
        FallsAsleep,
        WakesUp
    }

    public struct GuardStatus
    {
        public DateTime Date { get; }
        public int Number { get; }
        public HashSet<int> AsleepMinutes { get; }

        public GuardStatus(int number, DateTime date, IEnumerable<int> asleepMinutes)
        {
            Number = number;
            Date = date;
            AsleepMinutes = new HashSet<int>(asleepMinutes);
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
