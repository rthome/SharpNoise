using SharpNoise.Modules;

namespace SharpNoise.Models
{
    /// <summary>
    /// Model that defines the displacement of a line segment.
    /// </summary>
    /// <remarks>
    /// This model returns an output value from a noise module given the
    /// one-dimensional coordinate of an input value located on a line
    /// segment, which can be used as displacements.
    ///
    /// This class is useful for creating:
    ///  - roads and rivers
    ///  - disaffected college students
    ///
    /// To generate an output value, pass an input value between 0.0 and 1.0
    /// to the <see cref="GetValue"/> method.  0.0 represents the start position of the
    /// line segment and 1.0 represents the end position of the line segment.
    /// </remarks>
    public class Line : Model
    {
        /// <summary>
        /// Gets or sets a flag indicating whether the output value is to be
        /// attenuated (moved toward 0.0) as the ends of the line segment are
        /// approached by the input value.
        /// </summary>
        public bool Attenuate { get; set; }

        // start point
        double x0, y0, z0;

        // end point
        double x1, y1, z1;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Line()
        {
            Attenuate = false;
            x0 = 0D;
            y0 = 0D;
            z0 = 0D;
            x1 = 0D;
            y1 = 0D;
            z1 = 0D;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceModule">The noise module that is used to generate the output
        /// values.</param>
        public Line(Module sourceModule)
            : this()
        {
            Source = sourceModule;
        }

        /// <summary>
        /// Sets the position ( x, y, z ) of the start of the line
        /// segment to choose values along.
        /// </summary>
        /// <param name="x">x coordinate of the start position.</param>
        /// <param name="y">y coordinate of the start position.</param>
        /// <param name="z">z coordinate of the start position.</param>
        public void SetStartPoint(double x, double y, double z)
        {
            x0 = x;
            y0 = y;
            z0 = z;
        }

        /// <summary>
        /// Sets the position ( x, y, z ) of the end of the line
        /// segment to choose values along.
        /// </summary>
        /// <param name="x">x coordinate of the end position.</param>
        /// <param name="y">y coordinate of the end position.</param>
        /// <param name="z">z coordinate of the end position.</param>
        public void SetEndPoint(double x, double y, double z)
        {
            x1 = x;
            y1 = y;
            z1 = z;
        }

        public double GetValue(double p)
        {
            var x = (x1 - x0) * p + x0;
            var y = (y1 - y0) * p + y0;
            var z = (z1 - z0) * p + z0;
            var value = Source.GetValue(x, y, z);

            if (Attenuate)
                return p * (1.0 - p) * 4 * value;
            else
                return value;
        }
    }
}
