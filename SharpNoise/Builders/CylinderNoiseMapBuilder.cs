using System;

using SharpNoise.Models;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Builds a cylindrical noise map.
    /// </summary>
    /// <remarks>
    /// This class builds a noise map by filling it with coherent-noise values
    /// generated from the surface of a cylinder.
    ///
    /// This class describes these input values using an (angle, height)
    /// coordinate system.  After generating the coherent-noise value from the
    /// input value, it then "flattens" these coordinates onto a plane so that
    /// it can write the values into a two-dimensional noise map.
    ///
    /// The cylinder model has a radius of 1.0 unit and has infinite height.
    /// The cylinder is oriented along the y axis.  Its center is at the
    /// origin.
    ///
    /// The x coordinate in the noise map represents the angle around the
    /// cylinder's y axis.  The y coordinate in the noise map represents the
    /// height above the x-z plane.
    ///
    /// The application must provide the lower and upper angle bounds of the
    /// noise map, in degrees, and the lower and upper height bounds of the
    /// noise map, in units.
    /// </remarks>
    public class CylinderNoiseMapBuilder : NoiseMapBuilder
    {
        /// <summary>
        /// Gets the lower angle boundary of the cylindrical noise map, in degrees.
        /// </summary>
        public double LowerAngleBound { get; private set; }

        /// <summary>
        /// Gets the lower height boundary of the cylindrical noise map, in units.
        /// </summary>
        public double LowerHeightBound { get; private set; }

        /// <summary>
        /// Gets the upper angle boundary of the cylindrical noise map, in degrees.
        /// </summary>
        public double UpperAngleBound { get; private set; }

        /// <summary>
        /// Gets the upper height boundary of the cylindrical noise map, in units.
        /// </summary>
        public double UpperHeightBound { get; private set; }

        /// <summary>
        /// Sets the coordinate boundaries of the noise map.
        /// </summary>
        /// <param name="lowerAngleBound">The lower angle boundary of the noise map, in degrees.</param>
        /// <param name="upperAngleBound">The upper angle boundary of the noise map, in degrees.</param>
        /// <param name="lowerHeightBound">The lower height boundary of the noise map, in units.</param>
        /// <param name="upperHeightBound">The upper height boundary of the noise map, in units.</param>
        /// <remarks>
        /// One unit is equal to the radius of the cylinder.
        /// </remarks>
        public void SetBounds(double lowerAngleBound, double upperAngleBound, double lowerHeightBound, double upperHeightBound)
        {
            if (lowerAngleBound >= upperAngleBound ||
                lowerHeightBound >= upperHeightBound)
                throw new ArgumentException("Lower bounds must be less than upper bounds.");

            LowerAngleBound = lowerAngleBound;
            UpperAngleBound = upperAngleBound;
            LowerHeightBound = lowerHeightBound;
            UpperHeightBound = upperHeightBound;
        }

        public override void Build()
        {
            if (LowerAngleBound >= UpperAngleBound ||
                LowerHeightBound >= UpperHeightBound ||
                destWidth <= 0 ||
                destHeight <= 0 ||
                SourceModule == null ||
                DestNoiseMap == null)
                throw new InvalidOperationException("Builder isn't properly set up.");

            DestNoiseMap.SetSize(destHeight, destWidth);
            Cylinder cylinderModel = new Cylinder(SourceModule);

            var angleExtent = UpperAngleBound - LowerAngleBound;
            var heightExtent = UpperHeightBound - LowerHeightBound;
            var xDelta = angleExtent / destWidth;
            var yDelta = heightExtent / destHeight;
            var curAngle = LowerAngleBound;
            var curHeight = LowerHeightBound;

            for (var y = 0; y < destHeight; y++)
            {
                for (var x = 0; x < destWidth; x++)
                {
                    var curValue = (float)cylinderModel.GetValue(curAngle, curHeight);
                    DestNoiseMap[x, y] = curValue;
                    curAngle += xDelta;
                }
                if (callback != null)
                    callback(DestNoiseMap.IterateLine(y));
                curHeight += yDelta;
            }
        }
    }
}
