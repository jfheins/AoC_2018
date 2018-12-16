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

			for (var elfPower = 4; elfPower < 20; elfPower++)
			{
				var sim = new BattleSimulator(input, elfPower);

				while (sim.Step(stopOnElfDeath: true))
				{
				}

				Console.WriteLine($"Elf power: {elfPower}");
				Console.WriteLine($"Ended because: {sim.TerminationReason}");
				Console.WriteLine($"Winning side: {sim.Winners}");
				Console.WriteLine($"Outcome: {sim.Rounds * sim.HitPointSum}");
				Console.WriteLine($"Rounds: {sim.Rounds}");
				Console.WriteLine();

				if (sim.Winners == "E")
				{
					break;
				}
			}


			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}