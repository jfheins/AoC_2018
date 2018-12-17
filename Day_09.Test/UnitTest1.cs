using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day_09.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void Test0()
		{
			var game = new MarbleGame(1);
			Assert.AreEqual("[-] (0)", game.ToString());
		}


		[TestMethod]
		public void Test1()
		{
			var game = new MarbleGame(1);
			game.PlaceNextMarble(1);
			Assert.AreEqual("[1]  0  (1)", game.ToString());
		}
	}
}
