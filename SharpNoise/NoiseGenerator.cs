using System;

namespace SharpNoise
{
    /// <summary>
    /// Implements basic noise generation methods.
    /// </summary>
    public static class NoiseGenerator
    {
        // A table of 256 random normalized vectors.  Each row is an (x, y, z, 0)
        // coordinate.  The 0 is used as padding so we can use bit shifts to index
        // any row in the table.
        static readonly double[] vectortable;

        // Constants used by the current version of libnoise.
        const int XNoiseGen = 1619;
        const int YNoiseGen = 31337;
        const int ZNoiseGen = 6971;
        const int SeedNoiseGen = 1013;
        const int ShiftNoiseGen = 8;

        static NoiseGenerator()
        {
            vectortable = new double[1024];
            var rng = new Random();

            for (var i = 0; i < vectortable.Length; i += 4)
            {
                vectortable[i] = (rng.NextDouble() * 2D) - 1D;
                vectortable[i + 1] = (rng.NextDouble() * 2D) - 1D;
                vectortable[i + 2] = (rng.NextDouble() * 2D) - 1D;
                vectortable[i + 3] = 0D;
            }
        }

        /// <summary>
        /// Generates a gradient-coherent-noise value from the coordinates of a
        /// three-dimensional input value.
        /// </summary>
        /// <param name="x">The x coordinate of the input value.</param>
        /// <param name="y">The y coordinate of the input value.</param>
        /// <param name="z">The z coordinate of the input value.</param>
        /// <param name="seed">The random number seed.</param>
        /// <param name="noiseQuality">The quality of the coherent-noise.</param>
        /// <returns>The generated gradient-coherent-noise value.</returns>
        /// <remarks>
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// For an explanation of the difference between gradient noise and
        /// value noise, see the comments for the <see cref="GradientNoise3D"/> function.
        /// </remarks>
        public static double GradientCoherentNoise3D(double x, double y, double z, int seed = 0, NoiseQuality noiseQuality = NoiseQuality.Standard)
        {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            var x0 = (x > 0D ? (int)x : (int)x - 1);
            var x1 = x0 + 1;
            var y0 = (y > 0D ? (int)y : (int)y - 1);
            var y1 = y0 + 1;
            var z0 = (z > 0D ? (int)z : (int)z - 1);
            var z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            double xs = 0D, ys = 0D, zs = 0D;
            switch (noiseQuality)
            {
                case NoiseQuality.Fast:
                    xs = (x - (double)x0);
                    ys = (y - (double)y0);
                    zs = (z - (double)z0);
                    break;
                case NoiseQuality.Standard:
                    xs = NoiseMath.SCurve3(x - (double)x0);
                    ys = NoiseMath.SCurve3(y - (double)y0);
                    zs = NoiseMath.SCurve3(z - (double)z0);
                    break;
                case NoiseQuality.Best:
                    xs = NoiseMath.SCurve5(x - (double)x0);
                    ys = NoiseMath.SCurve5(y - (double)y0);
                    zs = NoiseMath.SCurve5(z - (double)z0);
                    break;
            }

            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            double n0, n1, ix0, ix1, iy0, iy1;
            n0 = GradientNoise3D(x, y, z, x0, y0, z0, seed);
            n1 = GradientNoise3D(x, y, z, x1, y0, z0, seed);
            ix0 = NoiseMath.Linear(n0, n1, xs);
            n0 = GradientNoise3D(x, y, z, x0, y1, z0, seed);
            n1 = GradientNoise3D(x, y, z, x1, y1, z0, seed);
            ix1 = NoiseMath.Linear(n0, n1, xs);
            iy0 = NoiseMath.Linear(ix0, ix1, ys);
            n0 = GradientNoise3D(x, y, z, x0, y0, z1, seed);
            n1 = GradientNoise3D(x, y, z, x1, y0, z1, seed);
            ix0 = NoiseMath.Linear(n0, n1, xs);
            n0 = GradientNoise3D(x, y, z, x0, y1, z1, seed);
            n1 = GradientNoise3D(x, y, z, x1, y1, z1, seed);
            ix1 = NoiseMath.Linear(n0, n1, xs);
            iy1 = NoiseMath.Linear(ix0, ix1, ys);

            return NoiseMath.Linear(iy0, iy1, zs);
        }

        /// <summary>
        /// Generates a gradient-noise value from the coordinates of a
        /// three-dimensional input value and the integer coordinates of a
        /// nearby three-dimensional value.
        /// </summary>
        /// <param name="fx">The floating-point x coordinate of the input value.</param>
        /// <param name="fy">The floating-point y coordinate of the input value.</param>
        /// <param name="fz">The floating-point z coordinate of the input value.</param>
        /// <param name="ix">The integer x coordinate of a nearby value.</param>
        /// <param name="iy">The integer y coordinate of a nearby value.</param>
        /// <param name="iz">The integer z coordinate of a nearby value.</param>
        /// <param name="seed">The random number seed.</param>
        /// <returns>The generated gradient-noise value.</returns>
        /// <remarks>
        /// <para>
        ///   The difference between <paramref name="fx"/> and <paramref name="ix"/> must be less than or equal to one.
        ///   The difference between <paramref name="fy"/> and <paramref name="iy"/> must be less than or equal to one.
        ///   The difference between <paramref name="fz"/> and <paramref name="iz"/> must be less than or equal to one.
        ///</para>
        ///<para>
        /// A gradient-noise function generates better-quality noise than a
        /// value-noise function.  Most noise modules use gradient noise for
        /// this reason, although it takes much longer to calculate.
        ///
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// This function generates a gradient-noise value by performing the
        /// following steps:
        /// - It first calculates a random normalized vector based on the
        ///   nearby integer value passed to this function.
        /// - It then calculates a new value by adding this vector to the
        ///   nearby integer value passed to this function.
        /// - It then calculates the dot product of the above-generated value
        ///   and the floating-point input value passed to this function.
        ///
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        /// </para>
        /// </remarks>
        public static double GradientNoise3D(double fx, double fy, double fz, int ix, int iy, int iz, int seed = 0)
        {
            unchecked
            {
                // Randomly generate a gradient vector given the integer coordinates of the
                // input value.  This implementation generates a random number and uses it
                // as an index into a normalized-vector lookup table.
                int vectorIndex = (
                    XNoiseGen * ix
                  + YNoiseGen * iy
                  + ZNoiseGen * iz
                  + SeedNoiseGen * seed)
                  & (int)0xffffffff;
                vectorIndex ^= (vectorIndex >> ShiftNoiseGen);
                vectorIndex &= 0xff;

                var xvGradient = vectortable[(vectorIndex << 2)];
                var yvGradient = vectortable[(vectorIndex << 2) + 1];
                var zvGradient = vectortable[(vectorIndex << 2) + 2];

                // Set up us another vector equal to the distance between the two vectors
                // passed to this function.
                var xvPoint = (fx - (double)ix);
                var yvPoint = (fy - (double)iy);
                var zvPoint = (fz - (double)iz);

                // Now compute the dot product of the gradient vector with the distance
                // vector.  The resulting value is gradient noise.  Apply a scaling value
                // so that this noise value ranges from -1.0 to 1.0.
                return ((xvGradient * xvPoint)
                  + (yvGradient * yvPoint)
                  + (zvGradient * zvPoint)) * 2.12;
            }
        }

        /// <summary>
        /// Generates an integer-noise value from the coordinates of a
        /// three-dimensional input value.
        /// </summary>
        /// <param name="x">The integer x coordinate of the input value.</param>
        /// <param name="y">The integer y coordinate of the input value.</param>
        /// <param name="z">The integer z coordinate of the input value.</param>
        /// <param name="seed">A random number seed.</param>
        /// <returns>The generated integer-noise value.</returns>
        /// <remarks>
        /// The return value ranges from 0 to 2147483647.
        /// 
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        /// </remarks>
        public static int IntValueNoise3D(int x, int y, int z, int seed = 0)
        {
            unchecked
            {
                // All constants are primes and must remain prime in order for this noise
                // function to work correctly.
                var n = (XNoiseGen * x
                         + YNoiseGen * y
                         + ZNoiseGen * z
                         + SeedNoiseGen * seed)
                         & 0x7fffffff;
                n = (n >> 13) ^ n;
                return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
            }
        }

        /// <summary>
        /// Generates a value-coherent-noise value from the coordinates of a
        /// three-dimensional input value.
        /// </summary>
        /// <param name="x">The x coordinate of the input value.</param>
        /// <param name="y">The y coordinate of the input value.</param>
        /// <param name="z">The z coordinate of the input value.</param>
        /// <param name="seed">The random number seed.</param>
        /// <param name="noiseQuality">The quality of the coherent-noise.</param>
        /// <returns>The generated value-coherent-noise value.</returns>
        /// <remarks>
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// For an explanation of the difference between gradient noise and
        /// value noise, see the comments for the <see cref="GradientNoise3D"/> function.
        /// </remarks>
        public static double ValueCoherentNoise3D(double x, double y, double z, int seed = 0, NoiseQuality noiseQuality = NoiseQuality.Standard)
        {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            int x0 = (x > 0.0 ? (int)x : (int)x - 1);
            int x1 = x0 + 1;
            int y0 = (y > 0.0 ? (int)y : (int)y - 1);
            int y1 = y0 + 1;
            int z0 = (z > 0.0 ? (int)z : (int)z - 1);
            int z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            double xs = 0, ys = 0, zs = 0;
            switch (noiseQuality)
            {
                case NoiseQuality.Fast:
                    xs = (x - (double)x0);
                    ys = (y - (double)y0);
                    zs = (z - (double)z0);
                    break;
                case NoiseQuality.Standard:
                    xs = NoiseMath.SCurve3(x - (double)x0);
                    ys = NoiseMath.SCurve3(y - (double)y0);
                    zs = NoiseMath.SCurve3(z - (double)z0);
                    break;
                case NoiseQuality.Best:
                    xs = NoiseMath.SCurve5(x - (double)x0);
                    ys = NoiseMath.SCurve5(y - (double)y0);
                    zs = NoiseMath.SCurve5(z - (double)z0);
                    break;
            }

            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            double n0, n1, ix0, ix1, iy0, iy1;
            n0 = ValueNoise3D(x0, y0, z0, seed);
            n1 = ValueNoise3D(x1, y0, z0, seed);
            ix0 = NoiseMath.Linear(n0, n1, xs);
            n0 = ValueNoise3D(x0, y1, z0, seed);
            n1 = ValueNoise3D(x1, y1, z0, seed);
            ix1 = NoiseMath.Linear(n0, n1, xs);
            iy0 = NoiseMath.Linear(ix0, ix1, ys);
            n0 = ValueNoise3D(x0, y0, z1, seed);
            n1 = ValueNoise3D(x1, y0, z1, seed);
            ix0 = NoiseMath.Linear(n0, n1, xs);
            n0 = ValueNoise3D(x0, y1, z1, seed);
            n1 = ValueNoise3D(x1, y1, z1, seed);
            ix1 = NoiseMath.Linear(n0, n1, xs);
            iy1 = NoiseMath.Linear(ix0, ix1, ys);

            return NoiseMath.Linear(iy0, iy1, zs);
        }

        /// <summary>
        /// Generates a value-noise value from the coordinates of a
        /// three-dimensional input value.
        /// </summary>
        /// <param name="x">The x coordinate of the input value.</param>
        /// <param name="y">The y coordinate of the input value.</param>
        /// <param name="z">The z coordinate of the input value.</param>
        /// <param name="seed">A random number seed.</param>
        /// <returns>The generated value-noise value.</returns>
        /// <remarks>
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        /// </remarks>
        public static double ValueNoise3D(int x, int y, int z, int seed = 0)
        {
            return 1.0 - ((double)IntValueNoise3D(x, y, z, seed) / 1073741824.0);
        }
    }
}
