using Microsoft.VisualStudio.TestTools.UnitTesting;

using Core;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Test
{
    [TestClass]
    public class PriorityDictionaryTests
    {
        [TestMethod]
        public void CanBeConstructed()
        {
            var dict = new PriorityDictionary<int, string>();
            Assert.IsTrue(dict is IDictionary<int, string>);
        }

        [TestMethod]
        public void CanAddItems()
        {
            var dict = new PriorityDictionary<int, string>();
            dict.Add(1, "Test");
            Assert.AreEqual(dict[1], "Test");
        }
    }
}
