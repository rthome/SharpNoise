namespace SharpNoise.Utilities.Imaging
{
    /// <summary>
    /// Implements an image, a 2-dimensional array of color values.
    /// </summary>
    /// <remarks>
    /// An image can be used to store a color texture.
    ///
    /// These color values are of type <see cref="Color"/>.
    /// 
    /// The size (width and height) of the image can be specified during
    /// object construction or at any other time.
    ///
    /// The <see cref="GetValue"/> method returns the border value if the specified
    /// position lies outside of the image.
    /// </remarks>
    public class Image : Map<Color>
    {
        public override int UsedMemory
        {
            get 
            {
                unsafe
                {
                    return values.Length * sizeof(Color); 
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Image() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Image(int width, int height)
            : base(width, height)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Image(Image other)
            : base(other)
        {

        }

        /// <summary>
        /// Converts the Image to a System.Drawing.Bitmap
        /// </summary>
        /// <returns>Returns the created Bitmap</returns>
        /// <remarks>
        /// This isn't exactly optimised for speed. Will be slow for large Images.
        /// </remarks>
        public System.Drawing.Bitmap ToGdiBitmap()
        {
            var bitmap = new System.Drawing.Bitmap(Width, Height);
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    bitmap.SetPixel(x, y, GetValue(x, y).ToGdiColor());
                }
            }
            return bitmap;
        }
    }
}
