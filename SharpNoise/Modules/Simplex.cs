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

        private readonly byte[] P = new byte[256];

        private readonly short[] Perm = new short[512];
        private readonly short[] PermMod12 = new short[512];

        private void RegeneratePermutations()
        {
            var rng = new Random(Seed);
            rng.NextBytes(P);

            for (int i = 0; i < 512; i++)
            {
                Perm[i] = P[i & 255];
                PermMod12[i] = (short)(Perm[i] % 12);
            }
        }

        private static double Dot(ref Grad g, double[] xyz)
        {
            return g.X * xyz[0] + g.Y * xyz[1] + g.Z * xyz[2];
        }

        private static double SquareSub(double[] values)
        {
            double r = 0.6;

            for (int i = 1; i < values.Length; i++)
            {
                r -= values[i] * values[i];
            }

            return r;
        }

        // 3D simplex noise
        private double SimplexNoise3D(double xin, double yin, double zin)
        {
            // Skew the input space to determine which simplex cell we're in
            // Very nice and simple skew factor for 3D
            double s = (xin + yin + zin) * F3;

            int[] ijk = 
            {
                NoiseMath.FastFloor(xin + s),
                NoiseMath.FastFloor(yin + s),
                NoiseMath.FastFloor(zin + s),
            };

            double t = (ijk[0] + ijk[1] + ijk[2]) * G3;

            // Unskew the cell origin back to (x,y,z) space
            double[] XYZ0 = new double[3];
            for (var i = 0; i < 3; ++i)
                XYZ0[i] = ijk[i] - t;

            // The x,y,z distances from the cell origin
            double[] xyz0 = 
            {
                xin - XYZ0[0],
                yin - XYZ0[1],
                zin - XYZ0[2],
            };

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int[] ijk1; // Offsets for second corner of simplex in (i,j,k) coords
            int[] ijk2; // Offsets for third corner of simplex in (i,j,k) coords
            if (xyz0[0] >= xyz0[1])
            {
                // X Y Z order
                if (xyz0[1] >= xyz0[2])
                {
                    ijk1 = new[] { 1, 0, 0 };
                    ijk2 = new[] { 1, 1, 0 };
                }
                // X Z Y order
                else if (xyz0[0] >= xyz0[2])
                {
                    ijk1 = new[] { 1, 0, 0 };
                    ijk2 = new[] { 1, 0, 1 };
                }
                // Z X Y order
                else
                {
                    ijk1 = new[] { 0, 0, 1 };
                    ijk2 = new[] { 1, 0, 1 };
                }
            }
            else // x0<y0
            {
                // Z Y X order
                if (xyz0[1] < xyz0[2])
                {
                    ijk1 = new[] { 0, 0, 1 };
                    ijk2 = new[] { 0, 1, 1 };
                }
                // Y Z X order
                else if (xyz0[0] < xyz0[2])
                {
                    ijk1 = new[] { 0, 1, 0 };
                    ijk2 = new[] { 0, 1, 1 };
                }
                // Y X Z order
                else
                {
                    ijk1 = new[] { 0, 1, 0 };
                    ijk2 = new[] { 1, 1, 0 };
                }
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            var xyz1 = new double[3]; // Offsets for second corner in (x,y,z) coords
            var xyz2 = new double[3]; // Offsets for third corner in (x,y,z) coords
            var xyz3 = new double[3]; // Offsets for last corner in (x,y,z) coords
            for (var i = 0; i < 3; ++i)
            {
                xyz1[i] = xyz0[i] - ijk1[i] + G3;
                xyz2[i] = xyz0[i] - ijk2[i] + 2 * G3;
                xyz3[i] = xyz0[i] - 1 + 3 * G3;
            }

            var xyz = new double[][]
            {
                xyz0,
                xyz1,
                xyz2,
                xyz3,
            };

            // Work out the hashed gradient indices of the four simplex corners
            int ii = ijk[0] & 255;
            int jj = ijk[1] & 255;
            int kk = ijk[2] & 255;
            int[] gi = 
            {
                PermMod12[ii + Perm[jj + Perm[kk]]],
                PermMod12[ii + ijk1[0] + Perm[jj + ijk[1] + Perm[kk + ijk1[2]]]],
                PermMod12[ii + ijk2[0] + Perm[jj + ijk2[1] + Perm[kk + ijk2[2]]]],
                PermMod12[ii + 1 + Perm[jj + 1 + Perm[kk + 1]]],
            };

            // Noise contributions from the four corners
            double[] ni = new double[4];
            double[] ti = new double[4];

            for (var i = 0; i < 4; ++i)
            {
                var value = SquareSub(xyz[i]);
                ti[i] = value;
                if (value >= 0)
                {
                    ti[i] = value * value;
                    ni[i] = ti[i] + ti[i] * Dot(ref Grad3[gi[i]], xyz[i]);
                }
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0 * (ni[0] + ni[1] + ni[2] + ni[3]);
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
        /// Default seed value for the Simplex noise module.
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        public double Frequency { get; set; }

        private int seed;

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
        public double Lacunarity { get; set; }

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
        public int OctaveCount { get; set; }

        /// <summary>
        /// Gets or sets the persistence value of the Simplex noise.
        /// </summary>
        /// <remarks>
        /// The persistence value controls the roughness of the Simplex noise.
        ///
        /// For best results, set the persistence to a number between 0.0 and
        /// 1.0.
        /// </remarks>
        public double Persistence { get; set; }

        /// <summary>
        /// Gets or sets the seed value used by the Simplex noise function.
        /// </summary>
        public int Seed
        {
            get { return seed; }
            set
            {
                seed = value;
                RegeneratePermutations();
            }
        }

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
            double signal = 0D;
            double currentPersistence = 1D;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                signal = SimplexNoise3D(x, y, z);
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
            Frequency = DefaultFrequency;
            Lacunarity = DefaultLacunarity;
            OctaveCount = DefaultOctaveCount;
            Persistence = DefaultPersistence;
            Seed = DefaultSeed;
        }
    }
}
