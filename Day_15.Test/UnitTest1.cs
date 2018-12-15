using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day_15.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_1.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100){}

			Assert.AreEqual(47, sim.Rounds);
			Assert.AreEqual(590, sim.HitPointSum);
		}

		[TestMethod]
		public void TestMethod2()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_2.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100) { }

			Assert.AreEqual(37, sim.Rounds);
			Assert.AreEqual(982, sim.HitPointSum);
		}

		[TestMethod]
		public void TestMethod3()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_3.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100) { }

			Assert.AreEqual(46, sim.Rounds);
			Assert.AreEqual(859, sim.HitPointSum);
		}

		[TestMethod]
		public void TestMethod4()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_4.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100) { }

			Assert.AreEqual(35, sim.Rounds);
			Assert.AreEqual(793, sim.HitPointSum);
		}

		[TestMethod]
		public void TestMethod5()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_5.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100) { }

			Assert.AreEqual(54, sim.Rounds);
			Assert.AreEqual(536, sim.HitPointSum);
		}

		[TestMethod]
		public void TestMethod6()
		{
			var input = File.ReadAllLines(@"../../../../Day_15/bsp_6.txt");

			var sim = new BattleSimulator(input);
			while (sim.Step() && sim.Rounds < 100) { }

			Assert.AreEqual(20, sim.Rounds);
			Assert.AreEqual(937, sim.HitPointSum);
		}
	}
}
