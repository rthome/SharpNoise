using System;
using System.Threading;
using System.Threading.Tasks;

using SharpNoise.Models;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Builds a spherical noise map.
    /// </summary>
    /// <remarks>
    /// This class builds a noise map by filling it with coherent-noise values
    /// generated from the surface of a sphere.
    ///
    /// This class describes these input values using a (latitude, longitude)
    /// coordinate system.  After generating the coherent-noise value from the
    /// input value, it then "flattens" these coordinates onto a plane so that
    /// it can write the values into a two-dimensional noise map.
    ///
    /// The sphere model has a radius of 1.0 unit.  Its center is at the
    /// origin.
    ///
    /// The x coordinate in the noise map represents the longitude.  The y
    /// coordinate in the noise map represents the latitude.  
    ///
    /// The application must provide the southern, northern, western, and
    /// eastern bounds of the noise map, in degrees.
    /// </remarks>
    public class SphereNoiseMapBuilder : NoiseMapBuilder
    {
        /// <summary>
        /// Gets the eastern boundary of the spherical noise map.
        /// </summary>
        public double EastLonBound { get; private set; }

        /// <summary>
        /// Gets the northern boundary of the spherical noise map.
        /// </summary>
        public double NorthLatBound { get; private set; }

        /// <summary>
        /// Gets the southern boundary of the spherical noise map.
        /// </summary>
        public double WestLonBound { get; private set; }

        /// <summary>
        /// Gets the western boundary of the spherical noise map.
        /// </summary>
        public double SouthLatBound { get; private set; }

        /// <summary>
        /// Sets the coordinate boundaries of the noise map.
        /// </summary>
        /// <param name="southLatBound">The southern boundary of the noise map, in
        /// degrees.</param>
        /// <param name="northLatBound">The northern boundary of the noise map, in
        /// degrees.</param>
        /// <param name="westLonBound">The western boundary of the noise map, in
        /// degrees.</param>
        /// <param name="eastLonBound">The eastern boundary of the noise map, in
        /// degrees.</param>
        public void SetBounds(double southLatBound, double northLatBound, double westLonBound, double eastLonBound)
        {
            if (southLatBound >= northLatBound ||
                westLonBound >= eastLonBound) 
                throw new ArgumentException("Lower bounds must be less than upper bounds.");

          SouthLatBound = southLatBound;
          NorthLatBound = northLatBound;
          WestLonBound = westLonBound;
          EastLonBound = eastLonBound;
        }

        protected override void PrepareBuild()
        {
            if (EastLonBound <= WestLonBound ||
                NorthLatBound <= SouthLatBound ||
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

            Sphere sphereModel = new Sphere(SourceModule);

            var lonExtent = EastLonBound - WestLonBound;
            var latExtent = NorthLatBound - SouthLatBound;
            var xDelta = lonExtent / destWidth;
            var yDelta = latExtent / destHeight;
            var curLon = WestLonBound;
            var curLat = SouthLatBound;

            for (var y = 0; y < destHeight; y++)
            {
                curLon = WestLonBound;
                for (var x = 0; x < destWidth; x++)
                {
                    var curValue = (float)sphereModel.GetValue(curLat, curLon);
                    DestNoiseMap[x, y] = curValue;
                    curLon += xDelta;
                }
                if (callback != null)
                    callback(DestNoiseMap.IterateLine(y));
                curLat += yDelta;
            }
        }

        protected override void BuildParallelImpl(CancellationToken cancellationToken)
        {
            Sphere sphereModel = new Sphere(SourceModule);

            var lonExtent = EastLonBound - WestLonBound;
            var latExtent = NorthLatBound - SouthLatBound;
            var xDelta = lonExtent / destWidth;
            var yDelta = latExtent / destHeight;

            var po = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
            };

            Parallel.For(0, destHeight, po, y =>
            {
                double curLat = SouthLatBound + y * yDelta;

                int x;
                double curLon = WestLonBound;

                for (x = 0, curLon = WestLonBound; x < destWidth; x++, curLon += xDelta)
                {
                    var curValue = (float)sphereModel.GetValue(curLat, curLon);
                    DestNoiseMap[x, y] = curValue;
                }
            });
        }
    }
}
