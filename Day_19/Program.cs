using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Day_16;

namespace Day_19
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input.txt");
			var sw = new Stopwatch();
			sw.Start();

			var instructions = input.Skip(1).Select(ParseLine).ToArray();

			var ipRegister = int.Parse(input[0][4].ToString());
			var ip = 0;

			var registers = new[] {0, 0, 0, 0, 0, 0};
			while (ip >= 0 && ip < instructions.Length)
			{
				registers[ipRegister] = ip;
				Calculate(instructions[ip], ref registers);
				ip = registers[ipRegister];
				ip++;
			}

			var part1 = registers[0];
			Console.WriteLine($"Part 1: {part1}");


			//instructions[2] = new Instruction(OpCode.Seti, 10551250, 0, 3);
			ip = 0;
			ipRegister = int.Parse(input[0][4].ToString());
			registers = new[] {1, 0, 0, 0, 0, 0};
			while (ip >= 0 && ip < instructions.Length)
			{
				if (ip >= 3 && ip <= 11)
				{
					ip = InnerLoop(ref registers);
					//Console.WriteLine(registers[1]);
				}
				else
				{
					registers[ipRegister] = ip;
					Calculate(instructions[ip], ref registers);
					ip = registers[ipRegister];
					ip++;
				}
			}

			var part2 = registers[0];

			Console.WriteLine($"Part 2: {part2}");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		private static int InnerLoop(ref int[] registers)
		{
			const int register5 = 10551264;

			while (true)
			{
				if (registers[1] * registers[3] == register5)
				{
					registers[0]++;
				}

				registers[3]++;
				if (registers[3] > register5)
				{
					registers[2] = 1;
					registers[4] = 11;
					return 12;
				}
			}
		}

		private static Instruction ParseLine(string line)
		{
			var str = line.Substring(0, 4);
			var opcode = Enum.Parse<OpCode>(str, true);
			var args = line.ParseInts();
			return new Instruction(opcode, args[0], args[1], args[2]);
		}

		public static void Calculate(Instruction op, ref int[] registers)
		{
			switch (op.OpCode)
			{
				case OpCode.Addr:
					registers[op.C] = registers[op.A] + registers[op.B];
					break;
				case OpCode.Addi:
					registers[op.C] = registers[op.A] + op.B;
					break;
				case OpCode.Mulr:
					registers[op.C] = registers[op.A] * registers[op.B];
					break;
				case OpCode.Muli:
					registers[op.C] = registers[op.A] * op.B;
					break;
				case OpCode.Banr:
					registers[op.C] = registers[op.A] & registers[op.B];
					break;
				case OpCode.Bani:
					registers[op.C] = registers[op.A] & op.B;
					break;
				case OpCode.Borr:
					registers[op.C] = registers[op.A] | registers[op.B];
					break;
				case OpCode.Bori:
					registers[op.C] = registers[op.A] | op.B;
					break;
				case OpCode.Setr:
					registers[op.C] = registers[op.A];
					break;
				case OpCode.Seti:
					registers[op.C] = op.A;
					break;
				case OpCode.Gtir:
					registers[op.C] = op.A > registers[op.B] ? 1 : 0;
					break;
				case OpCode.Gtri:
					registers[op.C] = registers[op.A] > op.B ? 1 : 0;
					break;
				case OpCode.Gtrr:
					registers[op.C] = registers[op.A] > registers[op.B] ? 1 : 0;
					break;
				case OpCode.Eqir:
					registers[op.C] = op.A == registers[op.B] ? 1 : 0;
					break;
				case OpCode.Eqri:
					registers[op.C] = registers[op.A] == op.B ? 1 : 0;
					break;
				case OpCode.Eqrr:
					registers[op.C] = registers[op.A] == registers[op.B] ? 1 : 0;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(op), op, null);
			}
		}
	}
}