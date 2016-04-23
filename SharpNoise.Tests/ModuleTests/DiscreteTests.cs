using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class DiscreteTests
    {
        Random random;

        const Byte VALUES = 4;

        public void DiscreteMainTestCode(Module source, Byte Count = VALUES)
        {
            Assert.IsNotNull(source);

            Discrete module = new Discrete() { Source0 = source, Values = Count };

            random = new Random((int)DateTime.Now.Ticks & (0x0000FFFF));

            double sourceValue, checkvalue;
            double x, y, z;

            for (Byte count = 0; count < 50; count++)
            {
                x = random.NextDouble();
                y = random.NextDouble();
                z = random.NextDouble();

                sourceValue = source.GetValue(x, y, z);

                checkvalue = 0;

                for (Byte value = 0; value < Count; value++)
                {
                    if (sourceValue >= value * 1.0 / Count && sourceValue < (value + 1) * 1.0 / Count)
                    {
                        checkvalue = value * 1.0 / Count;
                    }
                }

                if (checkvalue != 0)
                {
                    Assert.AreEqual(checkvalue, module.GetValue(x, y, z));
                }
                else
                {
                    Assert.AreEqual(0, module.GetValue(x, y, z));
                }

            }
        }


        [TestMethod]
        public void DiscretePerlinTest()
        {
            Perlin mperlin = new Perlin();
            
            DiscreteMainTestCode(mperlin, 3);
        }

        [TestMethod]
        public void DiscreteBillowTest()
        {
            Billow mbillow = new Billow();
            mbillow.Frequency = 0.5;
            mbillow.Persistence = 0.8;

            DiscreteMainTestCode(mbillow, 5);
        }

        [TestMethod]
        public void DiscreteVoronoiTest()
        {
            Voronoi mvoronoi = new Voronoi();

            ScaleBias mscabi = new ScaleBias();
            mscabi.Source0 = mvoronoi;
            mscabi.Bias = 0.6;
            mscabi.Scale = 0.5;

            DiscreteMainTestCode(mscabi, 4);
        }
    }
}
