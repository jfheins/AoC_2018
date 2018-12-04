using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_04
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines(@"../../../example.txt");

            var records = input.Select(ParseLine).ToList();

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
            int guardNumber = -1;
            // Guard #99 begins shift
            var guardregex = new Regex(@"Guard #(\d+) begins shift");

            foreach (var tuple in ordered)
            {
                if (tuple.action == "falls asleep")
                {
                    yield return new GuardAction(guardNumber, tuple.time, GuardConsciousness.FallsAsleep);
                }
                else if (tuple.action == "wakes up")
                {
                    yield return new GuardAction(guardNumber, tuple.time, GuardConsciousness.FallsAsleep);

                } else if(guardregex.IsMatch(tuple.action))
                {
                    var number = guardregex.Match(tuple.action).Groups[1].Value;
                    guardNumber = int.Parse(number);
                }
            }
        }
    }

    public enum GuardConsciousness
    { FallsAsleep, WakesUp }

    public struct GuardAction
    {
        public GuardConsciousness Consciousness { get; set; }
        public DateTime DateTime { get; set; }
        public int Number { get; set; }

        public GuardAction(int number, DateTime dateTime, GuardConsciousness consciousness)
        {
            Number = number;
            DateTime = dateTime;
            Consciousness = consciousness;
        }
    }
}
