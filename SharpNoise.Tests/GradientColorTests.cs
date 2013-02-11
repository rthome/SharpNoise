using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Utilities;

namespace SharpNoise.Tests
{
    [TestClass]
      public class MyTestClass
      {
          
      }

    [TestClass]
    class GradientColorTests
    {
        GradientColor gradient;

        [TestInitialize]
        void SetUp()
        {
            gradient = new GradientColor();
        }

        [TestMethod]
        public void GradientInsertTest()
        {
            gradient.AddGradientPoint(0, new Color());
            gradient.AddGradientPoint(1, new Color());

            var expected  = 2;
            var actual = gradient.PointCount;

            Assert.Equals(actual, expected);
        }
    }
}
