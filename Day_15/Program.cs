using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_15
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var result = Enumerable.Range(4, 16).AsParallel().Select(elfPower =>
			{
				var sim = new BattleSimulator(input, elfPower);

				while (sim.Step(stopOnElfDeath: true))
				{
					//Console.WriteLine(sim.ToString());
				}

				return new
				{
					elfPower,
					sim.TerminationReason,
					sim.Winners,
					outcome = sim.Rounds * sim.HitPointSum,
					sim.Rounds
				};
			}).First(res => res.Winners == "E");

			Console.WriteLine($"Elf power: {result.elfPower}");
			Console.WriteLine($"Ended because: {result.TerminationReason}");
			Console.WriteLine($"Winning side: {result.Winners}");
			Console.WriteLine($"Outcome: {result.outcome}");
			Console.WriteLine($"Rounds: {result.Rounds}");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}