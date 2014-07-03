using System;
using System.Threading;
using System.Threading.Tasks;

using SharpNoise.Models;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Builds a planar noise map.
    /// </summary>
    /// <remarks>
    /// This class builds a noise map by filling it with coherent-noise values
    /// generated from the surface of a plane.
    ///
    /// This class describes these input values using (x, z) coordinates.
    /// Their y coordinates are always 0.0.
    ///
    /// The application must provide the lower and upper x coordinate bounds
    /// of the noise map, in units, and the lower and upper z coordinate
    /// bounds of the noise map, in units.
    ///
    /// To make a tileable noise map with no seams at the edges, modify the
    /// <see cref="EnableSeamless"/> property.
    /// </remarks>
    public class PlaneNoiseMapBuilder : NoiseMapBuilder
    {
        /// <summary>
        /// Enables or disables seamless tiling.
        /// </summary>
        /// <remarks>
        /// Enabling seamless tiling builds a noise map with no seams at the
        /// edges.  This allows the noise map to be tileable.
        /// </remarks>
        public bool EnableSeamless { get; set; }

        /// <summary>
        /// Gets the lower x boundary of the planar noise map.
        /// </summary>
        public double LowerXBound { get; private set; }

        /// <summary>
        /// Gets the upper x boundary of the planar noise map.
        /// </summary>
        public double UpperXBound { get; private set; }

        /// <summary>
        /// Gets the lower z boundary of the planar noise map.
        /// </summary>
        public double LowerZBound { get; private set; }

        /// <summary>
        /// Gets the upper z boundary of the planar noise map.
        /// </summary>
        public double UpperZBound { get; private set; }

        /// <summary>
        /// Sets the boundaries of the planar noise map.
        /// </summary>
        /// <param name="lowerXBound">The lower x boundary of the noise map, in
        /// units.</param>
        /// <param name="upperXBound">The upper x boundary of the noise map, in
        /// units.</param>
        /// <param name="lowerZBound">The lower z boundary of the noise map, in
        /// units.</param>
        /// <param name="upperZBound">The upper z boundary of the noise map, in
        /// units.</param>
        public void SetBounds(double lowerXBound, double upperXBound, double lowerZBound, double upperZBound)
        {
            if (lowerXBound >= upperXBound ||
                lowerZBound >= upperZBound) 
                throw new ArgumentException("Lower bounds must be less than upper bounds.");

            LowerXBound = lowerXBound;
            UpperXBound = upperXBound;
            LowerZBound = lowerZBound;
            UpperZBound = upperZBound;
        }

        protected override void PrepareBuild()
        {
            if (LowerXBound >= UpperXBound ||
                LowerZBound >= UpperZBound ||
                destWidth <= 0 ||
                destHeight <= 0 ||
                SourceModule == null ||
                DestNoiseMap == null)
                throw new InvalidOperationException("Builder isn't properly set up.");

            DestNoiseMap.SetSize(destHeight, destWidth);
        }

        public override void Build()
        {
            PrepareBuild();

            Plane planeModel = new Plane(SourceModule);

            var xExtent = UpperXBound - LowerXBound;
            var zExtent = UpperZBound - LowerZBound;
            var xDelta = xExtent / destWidth;
            var zDelta = zExtent / destHeight;
            var xCur = LowerXBound;
            var zCur = LowerZBound;

            for (var z = 0; z < destHeight; z++)
            {
                xCur = LowerXBound;
                for (var x = 0; x < destWidth; x++)
                {
                    float finalValue;
                    if (!EnableSeamless)
                        finalValue = (float)planeModel.GetValue(xCur, zCur);
                    else
                    {
                        var swValue = planeModel.GetValue(xCur, zCur);
                        var seValue = planeModel.GetValue(xCur + xExtent, zCur);
                        var nwValue = planeModel.GetValue(xCur, zCur + zExtent);
                        var neValue = planeModel.GetValue(xCur + xExtent, zCur + zExtent);
                        var xBlend = 1.0 - ((xCur - LowerXBound) / xExtent);
                        var zBlend = 1.0 - ((zCur - LowerZBound) / zExtent);
                        var z0 = NoiseMath.Linear(swValue, seValue, xBlend);
                        var z1 = NoiseMath.Linear(nwValue, neValue, xBlend);
                        finalValue = (float)NoiseMath.Linear(z0, z1, zBlend);
                    }
                    xCur += xDelta;
                    DestNoiseMap[x, z] = finalValue;
                }
                if (callback != null)
                    callback(DestNoiseMap.IterateLine(z));
                zCur += zDelta;
            }
        }

        protected override void BuildParallelImpl(CancellationToken cancellationToken)
        {
            Plane planeModel = new Plane(SourceModule);

            var xExtent = UpperXBound - LowerXBound;
            var zExtent = UpperZBound - LowerZBound;
            var xDelta = xExtent / destWidth;
            var zDelta = zExtent / destHeight;

            Parallel.For(0, destHeight, z =>
            {
                double zCur = LowerZBound + z * zDelta;

                int x;
                double xCur;

                for (x = 0, xCur = LowerXBound; x < destWidth; x++, xCur += xDelta)
                {
                    float finalValue;
                    if (!EnableSeamless)
                        finalValue = (float)planeModel.GetValue(xCur, zCur);
                    else
                    {
                        var swValue = planeModel.GetValue(xCur, zCur);
                        var seValue = planeModel.GetValue(xCur + xExtent, zCur);
                        var nwValue = planeModel.GetValue(xCur, zCur + zExtent);
                        var neValue = planeModel.GetValue(xCur + xExtent, zCur + zExtent);
                        var xBlend = 1.0 - ((xCur - LowerXBound) / xExtent);
                        var zBlend = 1.0 - ((zCur - LowerZBound) / zExtent);
                        var z0 = NoiseMath.Linear(swValue, seValue, xBlend);
                        var z1 = NoiseMath.Linear(nwValue, neValue, xBlend);
                        finalValue = (float)NoiseMath.Linear(z0, z1, zBlend);
                    }
                    DestNoiseMap[x, z] = finalValue;
                }
            });
        }
    }
}
