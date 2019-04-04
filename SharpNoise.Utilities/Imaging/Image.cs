using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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
        public override int UsedMemory => values.Length * Marshal.SizeOf<Color>();

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

        /// <summary>
        /// Convert the image to a System.Drawing.Bitmap and save it to a stream
        /// </summary>
        /// <param name="destStream">The stream to save the image to</param>
        /// <param name="imageFormat">The image format to use</param>
        /// <param name="leaveOpen">
        /// If true, <paramref name="destStream"/> will be left open after saving the image;
        /// otherwise, the stream will be closed.
        /// </param>
        public void SaveGdiBitmap(Stream destStream, ImageFormat imageFormat, bool leaveOpen = true)
        {
            if (destStream == null)
                throw new ArgumentNullException("destStream");
            if (!destStream.CanWrite)
                throw new ArgumentException("Given stream cannot be written to.", "destStream");
            if (imageFormat == null)
                throw new ArgumentNullException("imageFormat");

            var bitmap = ToGdiBitmap();
            bitmap.Save(destStream, imageFormat);

            if (!leaveOpen)
                destStream.Close();
        }

        /// <summary>
        /// Convert the image to a System.Drawing.Bitmap and save it to a file
        /// </summary>
        /// <param name="filename">The file to save the image to</param>
        /// <param name="imageFormat">The ImageFormat to use</param>
        public void SaveGdiBitmap(string filename, ImageFormat imageFormat)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (imageFormat == null)
                throw new ArgumentNullException("imageFormat");

            try
            {
                using (var stream = File.OpenWrite(filename))
                    SaveGdiBitmap(stream, imageFormat);
            }
            catch (IOException exc)
            {
                throw new IOException(String.Format("Cannot write to given file '{0}'", filename), exc);
            }
        }
    }
}
