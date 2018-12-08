using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day_08
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var input = File.ReadAllText(@"../../../input.txt");

			var sw = new Stopwatch();
			sw.Start();

			var numbers =
				new Queue<int>(input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse));

			var root = Node.FromSeq(numbers);

			Console.WriteLine(root.SumMetadataRecusivly());

			sw.Stop();
			Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

			Console.ReadLine();
		}


		private class Node
		{
			public Node()
			{
				Children = new List<Node>();
				Metadata = new List<int>();
			}

			public List<Node> Children { get; }
			public List<int> Metadata { get; }

			public static Node FromSeq(Queue<int> numbers)
			{
				var childCount = numbers.Dequeue();
				var metaCount = numbers.Dequeue();
				var node = new Node();

				for (var i = 0; i < childCount; i++)
					node.Children.Add(FromSeq(numbers));

				for (var i = 0; i < metaCount; i++)
					node.Metadata.Add(numbers.Dequeue());

				return node;
			}

			public int SumMetadataRecusivly() => Metadata.Sum() + Children.Select(c => c.SumMetadataRecusivly()).Sum();

		}
	}
}