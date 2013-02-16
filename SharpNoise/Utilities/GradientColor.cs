using System;
using System.Collections.Generic;

namespace SharpNoise.Utilities
{
    /// <summary>
    /// Defines a color gradient.
    /// </summary>
    /// <remarks>
    /// A color gradient is a list of gradually-changing colors.  A color
    /// gradient is defined by a list of gradient points.  Each
    /// gradient point has a position and a color.  In a color gradient, the
    /// colors between two adjacent gradient points are linearly interpolated.
    ///
    /// To add a gradient point to the color gradient, pass its position and
    /// color to the <see cref="AddGradientPoint"/> method.
    ///
    /// To retrieve a color from a specific position in the color gradient,
    /// pass that position to the GetColor() method.
    ///
    /// This class is a useful tool for coloring height maps based on
    /// elevation.
    ///
    /// Gradient example
    ///
    /// Suppose a gradient object contains the following gradient points:
    /// - -1.0 maps to black.
    /// - 0.0 maps to white.
    /// - 1.0 maps to red.
    ///
    /// If an application passes -0.5 to the <see cref="GetColor"/> method, this method
    /// will return a gray color that is halfway between black and white.
    ///
    /// If an application passes 0.25 to the <see cref="GetColor"/> method, this method
    /// will return a very light pink color that is one quarter of the way
    /// between white and red.
    /// </remarks>
    public class GradientColor
    {
        /// <summary>
        /// Defines a point used to build a color gradient.
        /// </summary>
        /// <remarks>
        /// A color gradient is a list of gradually-changing colors.  A color
        /// gradient is defined by a list of gradient points.  Each
        /// gradient point has a position and a color.  In a color gradient, the
        /// colors between two adjacent gradient points are linearly interpolated.
        ///
        /// The ColorGradient class defines a color gradient by a list of these
        /// objects.
        /// </remarks>
        public struct GradientPoint : IComparable<GradientPoint>
        {
            /// <summary>
            /// The position of this gradient point.
            /// </summary>
            public readonly double Position;

            /// <summary>
            /// The color of this gradient point.
            /// </summary>
            public readonly Color Color;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="position">Position of the GradientPoint</param>
            /// <param name="color">Color of the GradientPoint</param>
            public GradientPoint(double position, Color color)
            {
                Position = position;
                Color = color;
            }

            public int CompareTo(GradientPoint other)
            {
                return Position.CompareTo(other.Position);
            }
        }

        readonly List<GradientPoint> gradientPoints;

        /// <summary>
        /// Gets the amount of gradient points in the gradient
        /// </summary>
        public int PointCount
        {
            get
            {
                return gradientPoints.Count;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GradientColor()
        {
            gradientPoints = new List<GradientPoint>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gradientPoints">A predefined list of gradient points</param>
        public GradientColor(IEnumerable<GradientPoint> gradientPoints)
        {
            this.gradientPoints = new List<GradientPoint>(gradientPoints);
        }

        /// <summary>
        /// Adds a gradient point to this gradient object.
        /// </summary>
        /// <param name="gradientPos">The position of this gradient point.</param>
        /// <param name="color">The color of this gradient point.</param>
        /// <remarks>
        /// No two gradient points have the same position.
        /// It does not matter which order these gradient points are added.
        /// </remarks>
        public void AddGradientPoint(double gradientPos, Color color)
        {
            var gradientPoint = new GradientPoint(gradientPos, color);

            var s = gradientPoints.BinarySearch(gradientPoint);
            if (s > 0)
                throw new ArgumentException("All GradientPoints must have unique positions");

            gradientPoints.Add(new GradientPoint(gradientPos, color));
            gradientPoints.Sort();
        }

        /// <summary>
        /// Deletes all the gradient points from this gradient object.
        /// </summary>
        public void ClearGradientPoints()
        {
            gradientPoints.Clear();
        }

        /// <summary>
        /// Returns the color at the specified position in the color gradient.
        /// </summary>
        /// <param name="gradientPosition">The specified position.</param>
        /// <returns>The color at that position.</returns>
        public Color GetColor(double gradientPosition)
        {
            // Find the first element in the gradient point array that has a gradient
            // position larger than the gradient position passed to this method.
            int indexPos;
            for (indexPos = 0; indexPos < gradientPoints.Count; indexPos++)
            {
                if (gradientPosition < gradientPoints[indexPos].Position)
                    break;
            }

            // Find the two nearest gradient points so that we can perform linear
            // interpolation on the color.
            var index0 = NoiseMath.Clamp(indexPos - 1, 0, gradientPoints.Count - 1);
            var index1 = NoiseMath.Clamp(indexPos, 0, gradientPoints.Count - 1);

            // If some gradient points are missing (which occurs if the gradient
            // position passed to this method is greater than the largest gradient
            // position or less than the smallest gradient position in the array), get
            // the corresponding gradient color of the nearest gradient point and exit
            // now.
            if (index0 == index1)
                return gradientPoints[index1].Color;

            // Compute the alpha value used for linear interpolation.
            var input0 = gradientPoints[index0].Position;
            var input1 = gradientPoints[index1].Position;
            var alpha = (gradientPosition - input0) / (input1 - input0);

            // Now perform the linear interpolation given the alpha value.
            var color0 = gradientPoints[index0].Color;
            var color1 = gradientPoints[index1].Color;
            return Color.LinearInterpColor(color0, color1, (float)alpha);
        }
    }
}
