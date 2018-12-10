using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Day_09
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var players = 438;
			var rounds = 71626;

			var game = new MarbleGame(players);

			for (int i = 0; i < rounds; i++)
			{
				game.PlaceNextMarble(i % players);
			}


			//Console.WriteLine(game.ToString());
			var winner = game.Score.MaxBy(kvp => kvp.Value).First();
			Console.WriteLine($"Elf {winner.Key + 1} scored {winner.Value}") ;
			Console.ReadLine();
		}
	}

	public class MarbleGame : LinkedList<Marble>
	{
		public int NextMarbleNumber { get; set; }
		public LinkedListNode<Marble> CurrentMarble { get; set; } = null;
		public string LastPlayer { get; set; } = "-";

		public Dictionary<int, int> Score { get; } = new Dictionary<int, int>();

		public MarbleGame(int playercount)
		{
			CurrentMarble = AddFirst(new Marble(0, -1));
			NextMarbleNumber = 1;

			for (int i = 0; i < playercount; i++)
			{
				Score[i] = 0;
			}
		}

		public void PlaceNextMarble(int player)
		{
			LastPlayer = player.ToString();
			if (NextMarbleNumber % 23 == 0)
			{
				Score[player] += NextMarbleNumber;
				var victim = GetNthLeftNeighborOf(CurrentMarble, 7);
				CurrentMarble = GetRightNeighborOf(victim);
				Score[player] += victim.Value.Number;
				Remove(victim);
			}
			else
			{
				var neighbor = GetRightNeighborOf(CurrentMarble);
				CurrentMarble = AddAfter(neighbor, new Marble(NextMarbleNumber, player));
			}
			NextMarbleNumber++;
		}

		// Clockwise
		private LinkedListNode<Marble> GetRightNeighborOf(LinkedListNode<Marble> x)
		{
			return x.Next ?? this.First;
		}

		// Counter clockwise
		private LinkedListNode<Marble> GetLeftNeighborOf(LinkedListNode<Marble> x)
		{
			return x.Previous ?? this.Last;
		}

		// Counter clockwise
		private LinkedListNode<Marble> GetNthLeftNeighborOf(LinkedListNode<Marble> x, int n)
		{
			for (var i = 0; i < n; i++)
				x = x.Previous ?? Last;

			return x;
		}


		public override string ToString()
		{
			var items = string.Join(' ', this.Select(m => m.ToString(CurrentMarble.Value == m)));
			return $"[{LastPlayer}] {items}";
		}
	}

	public class Marble
	{
		public int Number { get; }
		public int Player { get; }

		public Marble(int number, int player)
		{
			Number = number;
			Player = player;
		}

		public string ToString(bool parens = false)
		{
			return parens ? "(" + Number + ")" : string.Format("{0,2} ", Number);
		}
	}
}