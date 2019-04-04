using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs 3-dimensional Simplex noise.
    /// </summary>
    [Serializable]
    public class Simplex : Module
    {
        private struct Grad
        {
            public readonly double X, Y, Z;

            public Grad(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        private const double F3 = 1.0 / 3.0;
        private const double G3 = 1.0 / 6.0;

        private static readonly Grad[] Grad3 = 
        {
            new Grad(1,1,0), new Grad(-1,1,0), new Grad(1,-1,0), new Grad(-1,-1,0),
            new Grad(1,0,1), new Grad(-1,0,1), new Grad(1,0,-1), new Grad(-1,0,-1),
            new Grad(0,1,1), new Grad(0,-1,1), new Grad(0,1,-1), new Grad(0,-1,-1),
        };

        private static readonly short[] P = 
        {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private static readonly short[] Perm = new short[512];
        private static readonly short[] PermMod12 = new short[512];

        static Simplex()
        {
            for (int i = 0; i < 512; i++)
            {
                Perm[i] = P[i & 255];
                PermMod12[i] = (short)(Perm[i] % 12);
            }
        }

        private static double Dot(ref Grad g, double x, double y, double z)
        {
            return g.X * x + g.Y * y + g.Z * z;
        }

        // 3D simplex noise
        public static double SimplexNoise3D(double xin, double yin, double zin)
        {
            // Skew the input space to determine which simplex cell we're in
            // Very nice and simple skew factor for 3D
            double s = (xin + yin + zin) * F3;
            int i = NoiseMath.FastFloor(xin + s);
            int j = NoiseMath.FastFloor(yin + s);
            int k = NoiseMath.FastFloor(zin + s);

            double t = (i + j + k) * G3;
            // Unskew the cell origin back to (x,y,z) space
            double X0 = i - t;
            double Y0 = j - t;
            double Z0 = k - t;
            // The x,y,z distances from the cell origin
            double x0 = xin - X0;
            double y0 = yin - Y0;
            double z0 = zin - Z0;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            if (x0 >= y0)
            {
                // X Y Z order
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                // X Z Y order
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                // Z X Y order
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            }
            else // x0<y0
            {
                // Z Y X order
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                // Y Z X order
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                // Y X Z order
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            double x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;
            double x2 = x0 - i2 + 2.0 * G3; // Offsets for third corner in (x,y,z) coords
            double y2 = y0 - j2 + 2.0 * G3;
            double z2 = z0 - k2 + 2.0 * G3;
            double x3 = x0 - 1.0 + 3.0 * G3; // Offsets for last corner in (x,y,z) coords
            double y3 = y0 - 1.0 + 3.0 * G3;
            double z3 = z0 - 1.0 + 3.0 * G3;

            // Work out the hashed gradient indices of the four simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int gi0 = PermMod12[ii + Perm[jj + Perm[kk]]];
            int gi1 = PermMod12[ii + i1 + Perm[jj + j1 + Perm[kk + k1]]];
            int gi2 = PermMod12[ii + i2 + Perm[jj + j2 + Perm[kk + k2]]];
            int gi3 = PermMod12[ii + 1 + Perm[jj + 1 + Perm[kk + 1]]];

            // Noise contributions from the four corners
            double n0 = 0.0, n1 = 0.0, n2 = 0.0, n3 = 0.0;

            // Calculate the contribution from the four corners
            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 >= 0)
            {
                t0 *= t0;
                n0 = t0 * t0 * Dot(ref Grad3[gi0], x0, y0, z0);
            }

            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 >= 0)
            {
                t1 *= t1;
                n1 = t1 * t1 * Dot(ref Grad3[gi1], x1, y1, z1);
            }

            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 >= 0)
            {
                t2 *= t2;
                n2 = t2 * t2 * Dot(ref Grad3[gi2], x2, y2, z2);
            }

            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 >= 0)
            {
                t3 *= t3;
                n3 = t3 * t3 * Dot(ref Grad3[gi3], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0 * (n0 + n1 + n2 + n3);
        }

        /// <summary>
        /// Default frequency for the Simplex noise module.
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Default lacunarity for the Simplex noise module.
        /// </summary>
        public const double DefaultLacunarity = 2D;

        /// <summary>
        /// Default number of octaves for the Simplex noise module.
        /// </summary>
        public const int DefaultOctaveCount = 6;

        /// <summary>
        /// Default persistence value for the Simplex noise module.
        /// </summary>
        public const double DefaultPersistence = 0.5D;

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        public double Frequency { get; set; } = DefaultFrequency;

        /// <summary>
        /// Gets or sets the lacunarity of the Simplex noise.
        /// </summary>
        /// <remarks>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        ///
        /// For best results, set the lacunarity to a number between 1.5 and
        /// 3.5.
        /// </remarks>
        public double Lacunarity { get; set; } = DefaultLacunarity;

        /// <summary>
        /// Gets or sets the number of octaves that generate the Simplex noise.
        /// </summary>
        /// <remarks>
        /// The number of octaves controls the amount of detail in the Simplex
        /// noise.
        ///
        /// The larger the number of octaves, the more time required to
        /// calculate the Simplex noise value.
        /// </remarks>
        public int OctaveCount { get; set; } = DefaultOctaveCount;

        /// <summary>
        /// Gets or sets the persistence value of the Simplex noise.
        /// </summary>
        /// <remarks>
        /// The persistence value controls the roughness of the Simplex noise.
        ///
        /// For best results, set the persistence to a number between 0.0 and
        /// 1.0.
        /// </remarks>
        public double Persistence { get; set; } = DefaultPersistence;

        /// <summary>
        /// See documentation on the base class
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>The computed value</returns>
        public override double GetValue(double x, double y, double z)
        {
            double value = 0D;
            double currentPersistence = 1D;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                var signal = SimplexNoise3D(x, y, z);
                value += signal * currentPersistence;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                currentPersistence *= Persistence;
            }

            return value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Simplex()
            : base(0)
        {
        }
    }
}
