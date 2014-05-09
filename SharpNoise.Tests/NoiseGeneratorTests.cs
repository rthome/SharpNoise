using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace SharpNoise.Tests
{
    [TestClass]
    public class NoiseGeneratorTests
    {
        [TestInitialize]
        public void SetUp()
        {
            NoiseGenerator.SetDefaultVectorTable();
        }

        [TestMethod]
        public void NoiseGenerator_SetVectorTable_Test()
        {
            var table = NoiseGenerator.GenerateRandomVectorTable(Environment.TickCount);
            NoiseGenerator.SetVectorTable(table);

            CollectionAssert.AreEqual(table, NoiseGenerator.VectorTable);
        }
    }
}
