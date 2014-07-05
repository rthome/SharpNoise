using System;

namespace SharpNoise
{
    /// <summary>
    /// Contains various interpolation methods, and other math helpers
    /// </summary>
    public static class NoiseMath
    {
        /// <summary>
        /// Square root of 2
        /// </summary>
        public const double Sqrt2 = 1.4142135623730950488;

        /// <summary>
        /// Square root of 3
        /// </summary>
        public const double Sqrt3 = 1.7320508075688772935;

        /// <summary>
        /// Converts an angle from degrees to radians.
        /// </summary>
        public const double DegToRad = Math.PI / 180D;

        /// <summary>
        /// Converts an angle from radians to degrees.
        /// </summary>
        public const double RadToDeg = 1D / DegToRad;

        /// <summary>
        /// Performs cubic interpolation between two values bound between two other
        /// values.
        /// </summary>
        /// <param name="n0">The value before the first value.</param>
        /// <param name="n1">The first value.</param>
        /// <param name="n2">The second value.</param>
        /// <param name="n3">The value after the second value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The interpolated value.</returns>
        /// <remarks>
        /// The alpha value should range from 0.0 to 1.0.  If the alpha value is
        /// 0.0, this function returns n1.  If the alpha value is 1.0, this
        /// function returns n2.
        /// </remarks>
        public static double Cubic(double n0, double n1, double n2, double n3, double a)
        {
            var p = (n3 - n2) - (n0 - n1);
            var q = (n0 - n1) - p;
            var r = n2 - n0;
            var s = n1;
            return (p * a * a * a) + (q * a * a * a) + (r * a) + s;
        }

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="n0">The first value.</param>
        /// <param name="n1">The second value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The interpolated value.</returns>
        /// <remarks>
        /// The alpha value should range from 0.0 to 1.0.  If the alpha value is
        /// 0.0, this function returns <paramref name="n0"/>.  If the alpha value is 1.0, this
        /// function returns <paramref name="n1"/>.
        /// </remarks>
        public static double Linear(double n0, double n1, double a)
        {
            return ((1D - a) * n0) + (a * n1);
        }

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="n0">The first value.</param>
        /// <param name="n1">The second value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>The interpolated value.</returns>
        /// <remarks>
        /// The alpha value should range from 0.0 to 1.0.  If the alpha value is
        /// 0.0, this function returns <paramref name="n0"/>.  If the alpha value is 1.0, this
        /// function returns <paramref name="n1"/>.
        /// </remarks>
        public static float Linear(float n0, float n1, float a)
        {
            return (1 - a) * n0 + a * n1;
        }

        /// <summary>
        /// Performs bilinear interpolation between four values.
        /// </summary>
        /// <returns>The interpolated value.</returns>
        /// <remarks>
        /// <paramref name="x"/> and <paramref name="y"/>
        /// should range from 0.0 to 1.0.
        /// </remarks>
        public static float Bilinear(float x, float y,
            float x0y0, float x0y1, float x1y0, float x1y1)
        {
            float c0 = Linear(x0y0, x1y0, x);
            float c1 = Linear(x0y1, x1y1, x);

            return Linear(c0, c1, y);
        }

        /// <summary>
        /// Performs trilinear interpolation between eight values.
        /// </summary>
        /// <returns>The interpolated value.</returns>
        /// <remarks>
        /// <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/>
        /// should range from 0.0 to 1.0.
        /// </remarks>
        public static float Trilinear(float x, float y, float z,
            float x0y0z0, float x0y0z1, float x0y1z0, float x0y1z1,
            float x1y0z0, float x1y0z1, float x1y1z0, float x1y1z1)
        {
            float c00 = Linear(x0y0z0, x1y0z0, x);
            float c10 = Linear(x0y1z0, x1y1z0, x);
            float c01 = Linear(x0y0z1, x1y0z1, x);
            float c11 = Linear(x0y1z1, x1y1z1, x);

            float c0 = Linear(c00, c10, y);
            float c1 = Linear(c01, c11, y);

            return Linear(c0, c1, z);
        }

        /// <summary>
        /// Maps a value onto a cubic S-curve.
        /// </summary>
        /// <param name="a">The value to map onto a cubic S-curve.</param>
        /// <returns>The mapped value.</returns>
        /// <remarks>
        /// <paramref name="a"/> should range from 0.0 to 1.0.
        ///
        /// The derivitive of a cubic S-curve is zero at <paramref name="a"/> = 0.0
        /// and <paramref name="a"/> = 1.0
        /// </remarks>
        public static double SCurve3(double a)
        {
            return a * a * (3D - 2D * a);
        }

        /// <summary>
        /// Maps a value onto a quintic S-curve.
        /// </summary>
        /// <param name="a">The value to map onto a quintic S-curve.</param>
        /// <returns>The mapped value.</returns>
        /// <remarks>
        /// The first derivitive of a quintic S-curve
        /// is zero at <paramref name="a"/> = 0.0 and
        /// <paramref name="a"/> = 1.0
        ///
        /// The second derivitive of a quintic S-curve
        /// is zero at <paramref name="a"/> = 0.0 and
        /// <paramref name="a"/> = 1.0
        /// </remarks>
        public static double SCurve5(double a)
        {
            var a3 = a * a * a;
            var a4 = a3 * a;
            var a5 = a4 * a;
            return (6.0 * a5) - (15.0 * a4) + (10.0 * a3);
        }

        /// <summary>
        /// Converts latitude/longitude coordinates on a unit sphere into 3D
        /// Cartesian coordinates.
        /// </summary>
        /// <param name="lat">The latitude, in degrees.</param>
        /// <param name="lon">The longitude, in degrees.</param>
        /// <param name="x">On exit, this parameter contains the x coordinate.</param>
        /// <param name="y">On exit, this parameter contains the y coordinate.</param>
        /// <param name="z">On exit, this parameter contains the z coordinate.</param>
        /// <remarks>
        /// <paramref name="lat"/> must range from -90 to +90.
        /// <paramref name="lon"/> must range from -180 to +180.
        /// </remarks>
        public static void LatLonToXYZ(double lat, double lon, out double x, out double y, out double z)
        {
            var r = Math.Cos(DegToRad * lat);
            x = r * Math.Cos(DegToRad * lon);
            y = Math.Sin(DegToRad * lat);
            z = r * Math.Sin(DegToRad * lon);
        }

        /// <summary>
        /// Clamps a value onto a clamping range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lowerBound">The lower bound of the clamping range.</param>
        /// <param name="upperBound">The upper bound of the clamping range.</param>
        /// <returns>
        /// - <paramref name="value" /> if <paramref name="value" /> lies between <paramref name="lowerBound" /> and <paramref name="upperBound" />.
        /// - <paramref name="lowerBound" /> if <paramref name="value" /> is less than <paramref name="lowerBound" />.
        /// - <paramref name="upperBound" /> if <paramref name="value" /> is greater than <paramref name="upperBound" />.
        /// </returns>
        public static int Clamp(int value, int lowerBound, int upperBound)
        {
            return value > upperBound ? upperBound : (value < lowerBound ? lowerBound : value);
        }

        /// <summary>
        /// Clamps a value onto a clamping range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="lowerBound">The lower bound of the clamping range.</param>
        /// <param name="upperBound">The upper bound of the clamping range.</param>
        /// <returns>
        /// - <paramref name="value" /> if <paramref name="value" /> lies between <paramref name="lowerBound" /> and <paramref name="upperBound" />.
        /// - <paramref name="lowerBound" /> if <paramref name="value" /> is less than <paramref name="lowerBound" />.
        /// - <paramref name="upperBound" /> if <paramref name="value" /> is greater than <paramref name="upperBound" />.
        /// </returns>
        public static double Clamp(double value, double lowerBound, double upperBound)
        {
            return value > upperBound ? upperBound : (value < lowerBound ? lowerBound : value);
        }

        /// <summary>
        /// Swaps two values.
        /// </summary>
        /// <param name="a">A variable containing the first value.</param>
        /// <param name="b">A variable containing the second value.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            var c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Performs fast flooring on a positive double value
        /// </summary>
        /// <param name="x">The value to floor</param>
        /// <returns>Returns the floored value as an integer</returns>
        public static int FastFloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }
    }
}