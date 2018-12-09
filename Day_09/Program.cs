using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_09
{
	public class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
		}
	}

	public class MarbleGame : LinkedList<Marble>
	{
		public int NextMarbleNumber { get; set; }
		public LinkedListNode<Marble> CurrentMarble { get; set; } = null;
		public string LastPlayer { get; set; } = "-";

		public MarbleGame()
		{
			NextMarbleNumber = 0;
			CurrentMarble = AddFirst(new Marble(0, -1));
		}

		public void PlaceNextMarble(int player)
		{
			LastPlayer = player.ToString();
			var neighbor = GetRightNeighborOf(CurrentMarble);
			this.AddAfter(neighbor, new Marble(NextMarbleNumber, player));
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
			return parens ? "(" + Number + ")" : Number.ToString("##") + " ";
		}
	}
}