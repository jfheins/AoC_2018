using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day_07
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var input = File.ReadAllLines(@"../../../../Day_07/demo.txt");
            var (time, path) = Program.GetFinishTimeForSledge(input);
            
            Assert.AreEqual(15, time);
            Assert.AreEqual("CABFDE", path);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var input = File.ReadAllLines(@"../../../../Day_07/input.txt");
            var (time, path) = Program.GetFinishTimeForSledge(input, 5, 60);

            Assert.AreEqual(1031, time);
            Assert.AreEqual("AHJXDUBENGMQFTPYVCLWZKSROI", path);
        }
    }
}
