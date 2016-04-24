using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs Voronoi cells.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In mathematics, a Voronoi cell is a region containing all the
    /// points that are closer to a specific seed point than to any
    /// other seed point.  These cells mesh with one another, producing
    /// polygon-like formations.
    /// </para>
    /// 
    /// <para>
    /// By default, this noise module randomly places a seed point within
    /// each unit cube.  By modifying the frequency of the seed points,
    /// an application can change the distance between seed points.  The
    /// higher the frequency, the closer together this noise module places
    /// the seed points, which reduces the size of the cells.  To specify the
    /// frequency of the cells, modify the Frequency property.
    ///
    /// This noise module assigns each Voronoi cell with a random constant
    /// value from a coherent-noise function.  The displacement value
    /// controls the range of random values to assign to each cell.  The
    /// range of random values is +/- the displacement value.  
    /// Modify the Displacement property to specify the displacement value.
    /// </para>
    /// 
    /// <para>
    /// To modify the random positions of the seed points, 
    /// modify the Seed property.
    /// </para>
    /// 
    /// <para>
    /// This noise module can optionally add the distance from the nearest
    /// seed to the output value. To enable this feature, modify the
    /// EnableDistance property. This causes the points in the Voronoi cells
    /// to increase in value the further away that point is from the nearest
    /// seed point.
    /// </para>
    /// 
    /// <para>
    /// Voronoi cells are often used to generate cracked-mud terrain
    /// formations or crystal-like textures
    /// </para>
    /// 
    /// This noise module requires no source modules.
    /// </remarks>
    [Serializable]
    public class Voronoi : Module
    {
        /// <summary>
        /// Default displacement to apply to each cell
        /// </summary>
        public const double DefaultDisplacement = 1D;

        /// <summary>
        /// Default frequency of the seed points
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Default value for EnableDistance
        /// </summary>
        public const bool DefaultEnableDistance = false;

        /// <summary>
        /// Default seed of the noise function
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Enables or disables applying the distance from the nearest seed
        /// point to the output value.
        /// </summary>
        /// <remarks>
        /// Applying the distance from the nearest seed point to the output
        /// value causes the points in the Voronoi cells to increase in value
        /// the further away that point is from the nearest seed point.
        /// Setting this value to true (and setting the displacement to a
        /// near-zero value) causes this noise module to generate cracked mud
        /// formations.
        /// </remarks>
        public bool EnableDistance { get; set; } = DefaultEnableDistance;

        /// <summary>
        /// Gets or sets the displacement value of the Voronoi cells.
        /// </summary>
        /// <remarks>
        /// This noise module assigns each Voronoi cell with a random constant
        /// value from a coherent-noise function.  The displacement
        /// value controls the range of random values to assign to each
        /// cell.  The range of random values is +/- the displacement value.
        /// </remarks>
        public double Displacement { get; set; } = DefaultDisplacement;

        /// <summary>
        /// Gets or sets the frequency of the seed points.
        /// </summary>
        /// <remarks>
        /// The frequency determines the size of the Voronoi cells and the
        /// distance between these cells.
        /// </remarks>
        public double Frequency { get; set; } = DefaultFrequency;

        /// <summary>
        /// Gets or sets the seed value used by the Voronoi cells
        /// </summary>
        /// <remarks>
        /// The positions of the seed values are calculated by a
        /// coherent-noise function.  By modifying the seed value, the output
        /// of that function changes.
        /// </remarks>
        public int Seed { get; set; } = DefaultSeed;

        /// <summary>
        /// See the documentation on the base class.
        /// <seealso cref="Module"/>
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>Returns the computed value</returns>
        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            var xint = (x > 0D) ? (int)x : (int)x - 1;
            var yint = (y > 0D) ? (int)y : (int)y - 1;
            var zint = (z > 0D) ? (int)z : (int)z - 1;

            double minDistance = double.MaxValue;
            double xCandidate = 0D, yCandidate = 0D, zCandidate = 0D;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (var zCur = zint - 2; zCur <= zint + 2; zCur++)
            {
                for (var yCur = yint - 2; yCur <= yint + 2; yCur++)
                {
                    for (var xCur = xint - 2; xCur <= xint + 2; xCur++)
                    {
                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        var xPos = xCur + NoiseGenerator.ValueNoise3D(xCur, yCur, zCur, Seed);
                        var yPos = yCur + NoiseGenerator.ValueNoise3D(xCur, yCur, zCur, Seed + 1);
                        var zPos = zCur + NoiseGenerator.ValueNoise3D(xCur, yCur, zCur, Seed + 2);
                        var xDist = xPos - x;
                        var yDist = yPos - y;
                        var zDist = zPos - z;
                        var dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDistance)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDistance = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            var value = 0D;
            if (EnableDistance)
            {
                // Determine the distance to the nearest seed point.
                var xDist = xCandidate - x;
                var yDist = yCandidate - y;
                var zDist = zCandidate - z;
                value = (Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)) * NoiseMath.Sqrt3 - 1.0;
            }

            // Return the calculated distance with the displacement value applied.
            return value + (Displacement * NoiseGenerator.ValueNoise3D(
                (int)Math.Floor(xCandidate), 
                (int)Math.Floor(yCandidate), 
                (int)Math.Floor(zCandidate)));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Voronoi()
            : base(0)
        {
        }
    }
}
