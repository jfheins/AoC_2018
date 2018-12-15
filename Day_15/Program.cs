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

			for (var elfPower = 3; elfPower < 20; elfPower++)
			{
				var sim = new BattleSimulator(input, elfPower);
				var elfCount = sim.Players.Count(p => p.IsElf);

				while (sim.Step())
				{
					if (sim.Players.Count(player => player.IsElf) < elfCount)
					{
						Console.WriteLine("At least one elf died :-(");
						break;
					}
				}

				Console.WriteLine($"Elf power: {elfPower}");
				Console.WriteLine($"Remaining: {sim.Players.First().Symbol} ");
				Console.WriteLine($"Outcome: {sim.Rounds * sim.HitPointSum}");
				Console.WriteLine($"Rounds: {sim.Rounds}");
				Console.WriteLine();
			}


			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}
	}
}