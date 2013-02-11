using System;

namespace SharpNoise.Utilities
{
    /// <summary>
    /// Defines a color
    /// </summary>
    /// <remarks>
    /// A color object contains four 8-bit channels: red, green, blue, and an
    /// alpha (transparency) channel.  Channel values range from 0 to 255.
    ///
    /// The alpha channel defines the transparency of the color.  If the alpha
    /// channel has a value of 0, the color is completely transparent.  If the
    /// alpha channel has a value of 255, the color is completely opaque.
    /// </remarks>
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// Gets the red color channel
        /// </summary>
        public readonly byte Red;

        /// <summary>
        /// Gets the green color channel
        /// </summary>
        public readonly byte Green;

        /// <summary>
        /// Gets the blue color channel
        /// </summary>
        public readonly byte Blue;

        /// <summary>
        /// Gets the alpha channel
        /// </summary>
        public readonly byte Alpha;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="red">The red color channel</param>
        /// <param name="green">The green color channel</param>
        /// <param name="blue">The blue color channel</param>
        /// <param name="alpha">The alpha channel</param>
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other">The color to be cloned</param>
        public Color(Color other)
        {
            Red = other.Red;
            Green = other.Green;
            Blue = other.Blue;
            Alpha = other.Alpha;
        }

        /// <summary>
        /// Converts this Color to a System.Drawing.Color
        /// </summary>
        /// <returns>Returns the created color</returns>
        public System.Drawing.Color ToGdiColor()
        {
            return System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
        }

        static byte BlendChannel(byte channel0, byte channel1, float alpha)
        {
            var c0 = (float)channel0 / 255F;
            var c1 = (float)channel1 / 255F;
            return (byte)(((c1 * alpha) + (c0 * (1f - alpha))) * 255f);
        }

        public static Color LinearInterpColor(Color color0, Color color1, float alpha)
        {
            return new Color
            (
                BlendChannel(color0.Red, color1.Red, alpha),
                BlendChannel(color0.Green, color1.Green, alpha),
                BlendChannel(color0.Blue, color1.Blue, alpha),
                BlendChannel(color0.Alpha, color1.Alpha, alpha)
            );
        }

        public bool Equals(Color other)
        {
            return Red == other.Red 
                && Green == other.Green 
                && Blue == other.Blue 
                && Alpha == other.Alpha;
        }

        public override string ToString()
        {
            return String.Format("<Color {0} {1} {2} * {3}>", Red, Green, Blue, Alpha);
        }
    }
}
