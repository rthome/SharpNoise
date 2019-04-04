using System.Collections.Generic;
using System.Threading;

using SharpNoise.Modules;

using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Cache"/> module
    /// </summary>
    public class CacheTests
    {
        class CounterModule : Module
        {
            public double Counter { get; private set; }

            public CounterModule()
                : base(0)
            {
            }

            public override double GetValue(double x, double y, double z)
            {
                return ++Counter;
            }
        }

        class ThreadIdModule : Module
        {
            public ThreadIdModule()
                : base(0)
            {
            }

            public override double GetValue(double x, double y, double z)
            {
                return Thread.CurrentThread.ManagedThreadId;
            }
        }

        public static IEnumerable<object[]> PositionData()
        {
            yield return new object[] { 0, 0, 0 };
            yield return new object[] { 1, -1, 2 };
            yield return new object[] { -10000, 100, 100 };
        }

        [Theory]
        [MemberData(nameof(PositionData))]
        public void TestGetValueCalledOnce(double x, double y, double z)
        {
            var testModule = new CounterModule();
            var cache = new Cache { Source0 = testModule };

            var cachedValue = cache.GetValue(x, y, z);

            // Make sure repeated calls do not reach the test module's counter
            Assert.Equal(cachedValue, cache.GetValue(x, y, z));
            Assert.Equal(cachedValue, cache.GetValue(x, y, z));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(20)]
        [InlineData(100)]
        public void TestMultithreaded(int threadCount)
        {
            var source = new ThreadIdModule();
            var cache = new Cache { Source0 = source };

            var threadArray = new Thread[threadCount];

            bool startFlag = false;
            for (int i = 0; i < threadArray.Length; i++)
            {
                threadArray[i] = new Thread(() =>
                {
                    for (int k = 0; k < 500; k++)
                    {
                        SpinWait.SpinUntil(() => startFlag);

                        var sourceValue = source.GetValue(0, 0, 0);
                        Assert.Equal(sourceValue, cache.GetValue(0, 0, 0));
                        Assert.Equal(sourceValue, cache.GetValue(0, 0, 0));
                    }
                });
            }
            for (int i = 0; i < threadArray.Length; i++)
                threadArray[i].Start();
            startFlag = true;
            for (int i = 0; i < threadArray.Length; i++)
                threadArray[i].Join();
        }
    }
}
