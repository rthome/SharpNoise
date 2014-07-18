using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that rotates the input value around the origin before
    /// returning the output value from a source module.
    /// </summary>
    /// <remarks>
    /// The <see cref="GetValue"/> method rotates the coordinates of the input value
    /// around the origin before returning the output value from the source
    /// module.  To set the rotation angles, call the <see cref="SetAngles"/> method.  To
    /// set the rotation angle around the individual x, y, or z axes,
    /// modify the <see cref="XAngle"/>, <see cref="YAngle"/> or <see cref="ZAngle"/> properties,
    /// respectively.
    ///
    /// The coordinate system of the input value is assumed to be
    /// "left-handed" (x increases to the right, y increases upward,
    /// and z increases inward.)
    ///
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class RotatePoint : Module
    {
        /// <summary>
        /// Default rotation angle for all axes
        /// </summary>
        public const double DefaultRotation = 0D;

        // 1st column: x1, x2, x3
        // 2nd column: y1, y2, y3
        // 3rd column: z1, z2, z3
        protected double[,] matrix;
        protected double xAngle, yAngle, zAngle;

        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return SourceModules[0]; }
            set { SourceModules[0] = value; }
        }

        /// <summary>
        /// Gets or sets the rotation angle around the x axis to apply to the
        /// input value.
        /// </summary>
        public double XAngle
        {
            get
            {
                return xAngle;
            }
            set
            {
                SetAngles(value, yAngle, zAngle);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle around the y axis to apply to the
        /// input value.
        /// </summary>
        public double YAngle
        {
            get
            {
                return yAngle;
            }
            set
            {
                SetAngles(xAngle, value, zAngle);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle around the z axis to apply to the
        /// input value.
        /// </summary>
        public double ZAngle
        {
            get
            {
                return zAngle;
            }
            set
            {
                SetAngles(xAngle, yAngle, value);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RotatePoint()
            : base(1)
        {
            matrix = new double[3, 3];
        }

        /// <summary>
        /// Sets the rotation angles around all three axes to apply to the
        /// input value.
        /// </summary>
        /// <param name="xAngle">The rotation angle around the x axis, in degrees.</param>
        /// <param name="yAngle">The rotation angle around the x axis, in degrees.</param>
        /// <param name="zAngle">The rotation angle around the x axis, in degrees.</param>
        public void SetAngles(double xAngle, double yAngle, double zAngle)
        {
            double xCos, yCos, zCos, xSin, ySin, zSin;
            xCos = Math.Cos(xAngle * NoiseMath.DegToRad);
            yCos = Math.Cos(yAngle * NoiseMath.DegToRad);
            zCos = Math.Cos(zAngle * NoiseMath.DegToRad);
            xSin = Math.Sin(xAngle * NoiseMath.DegToRad);
            ySin = Math.Sin(yAngle * NoiseMath.DegToRad);
            zSin = Math.Sin(zAngle * NoiseMath.DegToRad);

            matrix[0,0] = ySin * xSin * zSin + yCos * zCos;
            matrix[1,0] = xCos * zSin;
            matrix[2,0] = ySin * zCos - yCos * xSin * zSin;
            matrix[0,1] = ySin * xSin * zCos - yCos * zSin;
            matrix[1,1] = xCos * zCos;
            matrix[2,1] = -yCos * xSin * zCos - ySin * zSin;
            matrix[0,2] = -ySin * xCos;
            matrix[1,2] = xSin;
            matrix[2,2] = yCos * xCos;

            this.xAngle = xAngle;
            this.yAngle = yAngle;
            this.zAngle = zAngle;
        }

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
            var nx = (matrix[0, 0] * x) + (matrix[1, 0] * y) + (matrix[2, 0] * z);
            var ny = (matrix[0, 1] * x) + (matrix[1, 1] * y) + (matrix[2, 1] * z);
            var nz = (matrix[0, 2] * x) + (matrix[1, 2] * y) + (matrix[2, 2] * z);
            return SourceModules[0].GetValue(nx, ny, nz);
        }
    }
}
