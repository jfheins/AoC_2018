using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;

namespace Day_16
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllLines(@"../../../input_1.txt");
			var sw = new Stopwatch();
			sw.Start();

			var numbers = input.Select(l => l.ParseInts()).Chunks(4);
			var beforeAfter = numbers.Select(chunk =>
			{
				var block = chunk.ToArray();
				return new
				{
					before = block[0],
					opcode = block[1],
					after = block[2]
				};
			}).ToList();

			var threeormore = 0;
			foreach (var sample in beforeAfter)
			{
				var possibleOpcodes = new List<OpCode>();
				for (var opcode = 0; opcode < 16; opcode++)
				{
					var instruction = InstructionFromOpcodeAndContent(opcode, sample.opcode);
					var result = Calculate(instruction, sample.before);
					if (result.SequenceEqual(sample.after))
					{
						possibleOpcodes.Add(instruction.OpCode);
					}
				}

				if (possibleOpcodes.Count >= 3)
				{
					threeormore++;
				}
			}

			var program = File.ReadAllLines(@"../../../input_2.txt").Select(l => l.ParseInts(4));

			var registers = new[] {0, 0, 0, 0};
			foreach (var instr in program)
			{
				var instruction = InstructionFromInput(instr);
				registers = Calculate(instruction, registers);
			}

			Console.WriteLine($"Part 1: {threeormore}");
			Console.WriteLine($"Part 2: {registers[0]}");

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
			Console.ReadLine();
		}

		public static Instruction InstructionFromOpcodeAndContent(int opcode, int[] content)
		{
			return new Instruction((OpCode) opcode, content[1], content[2], content[3]);
		}

		public static Instruction InstructionFromInput(int[] content)
		{
			return new Instruction((OpCode) content[0], content[1], content[2], content[3]);
		}

		public static int[] Calculate(Instruction op, int[] registers)
		{
			var result = registers.ToArray();
			switch (op.OpCode)
			{
				case OpCode.Addr:
					result[op.C] = result[op.A] + result[op.B];
					break;
				case OpCode.Addi:
					result[op.C] = result[op.A] + op.B;
					break;
				case OpCode.Mulr:
					result[op.C] = result[op.A] * result[op.B];
					break;
				case OpCode.Muli:
					result[op.C] = result[op.A] * op.B;
					break;
				case OpCode.Banr:
					result[op.C] = result[op.A] & result[op.B];
					break;
				case OpCode.Bani:
					result[op.C] = result[op.A] & op.B;
					break;
				case OpCode.Borr:
					result[op.C] = result[op.A] | result[op.B];
					break;
				case OpCode.Bori:
					result[op.C] = result[op.A] | op.B;
					break;
				case OpCode.Setr:
					result[op.C] = result[op.A];
					break;
				case OpCode.Seti:
					result[op.C] = op.A;
					break;
				case OpCode.Gtir:
					result[op.C] = op.A > result[op.B] ? 1 : 0;
					break;
				case OpCode.Gtri:
					result[op.C] = result[op.A] > op.B ? 1 : 0;
					break;
				case OpCode.Gtrr:
					result[op.C] = result[op.A] > result[op.B] ? 1 : 0;
					break;
				case OpCode.Eqir:
					result[op.C] = op.A == result[op.B] ? 1 : 0;
					break;
				case OpCode.Eqri:
					result[op.C] = result[op.A] == op.B ? 1 : 0;
					break;
				case OpCode.Eqrr:
					result[op.C] = result[op.A] == result[op.B] ? 1 : 0;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(op), op, null);
			}

			return result;
		}
	}

	public struct Instruction
	{
		public OpCode OpCode { get; }
		public int A { get; }
		public int B { get; }
		public int C { get; }

		public Instruction(OpCode opCode,
						   int a,
						   int b,
						   int c)
		{
			OpCode = opCode;
			A = a;
			B = b;
			C = c;
		}
	}

	public enum OpCode
	{
		Addi = 0,
		Bani = 1,
		Gtir = 2,
		Borr = 3,
		Eqrr = 4,
		Bori = 5,
		Gtrr = 6,
		Setr = 7,
		Muli = 8,
		Seti = 9,
		Banr = 10,
		Gtri = 11,
		Eqir = 12,
		Eqri = 13,
		Addr = 14,
		Mulr = 15
	}
}