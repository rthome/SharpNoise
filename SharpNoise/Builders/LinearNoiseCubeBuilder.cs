using System;
using System.Threading;
using System.Threading.Tasks;

using SharpNoise.Modules;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Builds a linear noise cube.
    /// </summary>
    /// <remarks>
    /// This class builds a noise cube by filling it with coherent-noise values
    /// generated linearly from the given bounds.
    ///
    /// This class describes these input values using (x, y, z) coordinates.
    ///
    /// The application must provide the lower and upper x, y and z coordinate bounds
    /// of the noise cube, in units.
    /// </remarks>
    public class LinearNoiseCubeBuilder : NoiseCubeBuilder
    {
        /// <summary>Gets the lower x boundary of the planar noise cube.</summary>
        public double LowerXBound { get; private set; }
        /// <summary>Gets the upper x boundary of the planar noise cube.</summary>
        public double UpperXBound { get; private set; }

        /// <summary>Gets the lower y boundary of the planar noise cube.</summary>
        public double LowerYBound { get; private set; }
        /// <summary>Gets the upper y boundary of the planar noise cube.</summary>
        public double UpperYBound { get; private set; }

        /// <summary>Gets the lower z boundary of the planar noise cube.</summary>
        public double LowerZBound { get; private set; }
        /// <summary>Gets the upper z boundary of the planar noise cube.</summary>
        public double UpperZBound { get; private set; }

        /// <summary>
        /// Sets the boundaries of the planar noise cube.
        /// </summary>
        /// <param name="lowerXBound">The lower x boundary of the noise cube, in units.</param>
        /// <param name="upperXBound">The upper x boundary of the noise cube, in units.</param>
        /// <param name="lowerYBound">The lower y boundary of the noise cube, in units.</param>
        /// <param name="upperYBound">The upper y boundary of the noise cube, in units.</param>
        /// <param name="lowerZBound">The lower z boundary of the noise cube, in units.</param>
        /// <param name="upperZBound">The upper z boundary of the noise cube, in units.</param>
        public void SetBounds(double lowerXBound, double upperXBound,
            double lowerYBound, double upperYBound,
            double lowerZBound, double upperZBound)
        {
            if (lowerXBound >= upperXBound || lowerYBound >= upperYBound || lowerZBound >= upperZBound)
                throw new ArgumentException("Lower bounds must be less than upper bounds.");

            LowerXBound = lowerXBound;
            UpperXBound = upperXBound;
            LowerYBound = lowerYBound;
            UpperYBound = upperYBound;
            LowerZBound = lowerZBound;
            UpperZBound = upperZBound;
        }

        protected override void PrepareBuild()
        {
            if (LowerXBound >= UpperXBound || LowerYBound >= UpperYBound || LowerZBound >= UpperZBound ||
                destWidth <= 0 || destHeight <= 0 || destDepth <= 0 ||
                SourceModule == null || DestNoiseCube == null)
                throw new InvalidOperationException("Builder isn't properly set up.");

            DestNoiseCube.SetSize(destWidth, destHeight, destDepth);
        }

        protected override void BuildImpl(CancellationToken cancellationToken)
        {
            var xExtent = UpperXBound - LowerXBound;
            var yExtent = UpperYBound - LowerYBound;
            var zExtent = UpperZBound - LowerZBound;
            var xDelta = xExtent / destWidth;
            var yDelta = yExtent / destHeight;
            var zDelta = zExtent / destDepth;

            var po = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
            };

            Parallel.For(0, destDepth, po, z =>
            {
                double zCur = LowerZBound + z * zDelta;

                double yCur = LowerYBound;
                for (var y = 0; y < destHeight; y++)
                {
                    double xCur = LowerXBound;
                    for (var x = 0; x < destWidth; x++)
                    {
                        float finalValue = (float)SourceModule.GetValue(xCur, yCur, zCur);
                        xCur += xDelta;
                        DestNoiseCube[x, y, z] = finalValue;
                    }
                    yCur += yDelta;
                }
                zCur += zDelta;
            });
        }
    }
}
