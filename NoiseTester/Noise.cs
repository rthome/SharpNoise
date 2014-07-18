using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseTester
{
    static class Noise
    {
        public static Module CreateNoiseTree()
        {
            var mountainTerrain = new RidgedMulti()
            {

            };

            var baseFlatTerrain = new Billow()
            {
                Frequency = 2,
            };

            var flatTerrain = new ScaleBias()
            {
                Source0 = baseFlatTerrain,
                Scale = 0.125,
                Bias = -0.75,
            };

            var terrainType = new Perlin()
            {
                Frequency = 0.5,
                Persistence = 0.25,
            };

            var terrainSelector = new Select()
            {
                Source0 = flatTerrain,
                Source1 = mountainTerrain,
                Control = terrainType,
                LowerBound = 0,
                UpperBound = 1000,
                EdgeFalloff = 0.125,
            };

            var finalTerrain = new Turbulence()
            {
                Source0 = terrainSelector,
                Frequency = 4,
                Power = 0.125,
            };

            return finalTerrain;
        }
    }
}
