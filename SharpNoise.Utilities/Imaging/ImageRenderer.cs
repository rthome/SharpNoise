using System;

namespace SharpNoise.Utilities.Imaging
{
    /// <summary>
    /// Renders an image from a noise map.
    /// </summary>
    /// <remarks>
    /// This class renders an image given the contents of a noise-map object.
    ///
    /// An application can configure the output of the image in three ways:
    /// - Specify the color gradient.
    /// - Specify the light source parameters.
    /// - Specify the background image.
    ///
    /// This class uses a color gradient to calculate the color for each pixel
    /// in the destination image according to the value from the corresponding
    /// position in the noise map.
    ///
    /// A color gradient is a list of gradually-changing colors.  A color
    /// gradient is defined by a list of gradient points.  Each
    /// gradient point has a position and a color.  In a color gradient, the
    /// colors between two adjacent gradient points are linearly interpolated.
    ///
    /// For example, suppose this class contains the following color gradient:
    ///
    /// - -1.0 maps to dark blue.
    /// - -0.2 maps to light blue.
    /// - -0.1 maps to tan.
    /// - 0.0 maps to green.
    /// - 1.0 maps to white.
    ///
    /// The value 0.5 maps to a greenish-white color because 0.5 is halfway
    /// between 0.0 (mapped to green) and 1.0 (mapped to white).
    ///
    /// The value -0.6 maps to a medium blue color because -0.6 is halfway
    /// between -1.0 (mapped to dark blue) and -0.2 (mapped to light blue).
    ///
    /// The color gradient requires a minimum of two gradient points.
    ///
    /// This class contains two pre-made gradients: a grayscale gradient and a
    /// color gradient suitable for terrain.  To use these pre-made gradients,
    /// call the <see cref="BuildGrayscaleGradient" /> or <see cref="BuildTerrainGradient" /> methods,
    /// respectively.
    ///
    /// The color value passed to <see cref="AddGradientPoint" /> has an alpha
    /// channel.  This alpha channel specifies how a pixel in the background
    /// image (if specified) is blended with the calculated color.  If the
    /// alpha value is high, this class weighs the blend towards the
    /// calculated color, and if the alpha value is low, this class weighs the
    /// blend towards the color from the corresponding pixel in the background
    /// image.
    ///
    /// This class contains a parallel light source that lights the image.  It
    /// interprets the noise map as a bump map.
    ///
    /// To enable or disable lighting, pass a Boolean value to the
    /// <see cref="EnableLight" /> property.
    ///
    /// To set the position of the light source in the "sky", call the
    /// <see cref="LightAzimuth" /> and <see cref="LightElev" /> properties.
    ///
    /// To set the color of the light source, call the <see cref="LightColor" /> property.
    ///
    /// To set the intensity of the light source, call the <see cref="LightIntensity" />
    /// property.  A good intensity value is 2.0, although that value tends to
    /// "wash out" very light colors from the image.
    /// 
    /// To set the contrast amount between areas in light and areas in shadow,
    /// call the <see cref="LightContrast" /> property.  Determining the correct contrast
    /// amount requires some trial and error, but if your application
    /// interprets the noise map as a height map that has its elevation values
    /// measured in meters and has a horizontal resolution of h meters, a
    /// good contrast amount to use is ( 1.0 / h ).
    /// 
    /// To specify a background image, pass an Image object to the
    /// <see cref="BackgroundImage" /> property.
    ///
    /// This class determines the color of a pixel in the destination image by
    /// blending the calculated color with the color of the corresponding
    /// pixel from the background image.
    ///
    /// The blend amount is determined by the alpha of the calculated color.
    /// If the alpha value is high, this class weighs the blend towards the
    /// calculated color, and if the alpha value is low, this class weighs the
    /// blend towards the color from the corresponding pixel in the background
    /// image.
    ///
    /// To render the image, perform the following steps:
    /// - Pass a NoiseMap object to the <see cref="SourceNoiseMap" /> property.
    /// - Pass an Image object to the <see cref="DestinationImage" /> property.
    /// - Pass an Image object to the <see cref="BackgroundImage" /> property (optional)
    /// - Call the <see cref="Render" /> method.
    /// </remarks>
    public class ImageRenderer
    {
        readonly GradientColor gradient;

        bool recalcLightValues;
        double sinAzimuth, cosAzimuth;
        double sinElevation, cosElevation;

        // backing fields for properties
        double lightIntensity, lightElevation, lightContrast, lightBrightness, lightAzimuth;

        /// <summary>
        /// Enables or disables the light source.
        /// </summary>
        /// <remarks>
        /// If the light source is enabled, this object will interpret the
        /// noise map as a bump map.
        /// </remarks>
        public bool EnableLight { get; set; }

        /// <summary>
        /// Enables or disables noise-map wrapping.
        /// </summary>
        /// <remarks>
        /// This object requires five points (the initial point and its four
        /// neighbors) to calculate light shading.  If wrapping is enabled,
        /// and the initial point is on the edge of the noise map, the
        /// appropriate neighbors that lie outside of the noise map will
        /// "wrap" to the opposite side(s) of the noise map.  Otherwise, the
        /// appropriate neighbors are cropped to the edge of the noise map.
        ///
        /// Enabling wrapping is useful when creating spherical renderings and
        /// tileable textures.
        /// </remarks>
        public bool EnableWrap { get; set; }

        /// <summary>
        /// Gets or sets the azimuth of the light source, in degrees.
        /// </summary>
        /// <remarks>
        /// The azimuth is the location of the light source around the
        /// horizon:
        /// - 0.0 degrees is east.
        /// - 90.0 degrees is north.
        /// - 180.0 degrees is west.
        /// - 270.0 degrees is south.
        ///
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public double LightAzimuth
        {
            get { return lightAzimuth; }
            set
            {
                lightAzimuth = value;
                recalcLightValues = true;
            }
        }

        /// <summary>
        /// Gets or sets the brightness of the light source.
        /// </summary>
        /// <remarks>
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public double LightBrightness
        {
            get { return lightBrightness; }
            set
            {
                lightBrightness = value;
                recalcLightValues = true;
            }
        }

        /// <summary>
        /// Gets or sets the color of the light source.
        /// </summary>
        /// <remarks>
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public Color LightColor { get; set; }

        /// <summary>
        /// Gets or sets the elevation of the light source, in degrees.
        /// </summary>
        /// <remarks>
        /// The elevation is the angle above the horizon:
        /// - 0 degrees is on the horizon.
        /// - 90 degrees is straight up.
        /// 
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public double LightElevation
        {
            get { return lightElevation; }
            set
            {
                lightElevation = value;
                recalcLightValues = true;
            }
        }

        /// <summary>
        /// Gets or sets the intensity of the light source.
        /// </summary>
        /// <remarks>
        /// A good value for intensity is 2.0.
        /// 
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public double LightIntensity
        {
            get { return lightIntensity; }
            set
            {
                if (value < 0D)
                    throw new ArgumentException("Intensity can't be less than 0.");

                lightIntensity = value;
                recalcLightValues = true;
            }
        }

        /// <summary>
        /// Gets or sets the contrast of the light source.
        /// </summary>
        /// <remarks>
        /// The contrast specifies how sharp the boundary is between the
        /// light-facing areas and the shadowed areas.
        ///
        /// The contrast determines the difference between areas in light and
        /// areas in shadow.  Determining the correct contrast amount requires
        /// some trial and error, but if your application interprets the noise
        /// map as a height map that has a spatial resolution of h meters
        /// and an elevation resolution of 1 meter, a good contrast amount to
        /// use is ( 1.0 / h ).
        /// 
        /// Make sure the light source is enabled via a call to the
        /// <see cref="EnableLight"/> property before calling the <see cref="Render"/> method.
        /// </remarks>
        public double LightContrast
        {
            get { return lightContrast; }
            set
            {
                if (value < 0D)
                    throw new ArgumentException("Contrast can't be less than 0.");

                lightContrast = value;
                recalcLightValues = true;
            }
        }

        /// <summary>
        /// Gets ot sets the background image.
        /// </summary>
        /// <remarks>
        /// If a background image has been specified, the <see cref="Render"/> method
        /// blends the pixels from the background image onto the corresponding
        /// pixels in the destination image.  The blending weights are
        /// determined by the alpha channel in the pixels in the destination
        /// image.
        ///
        /// The destination image must exist throughout the lifetime of this
        /// object unless another image replaces that image.
        /// </remarks>
        public Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the destination image.
        /// </summary>
        /// <remarks>
        /// The destination image will contain the rendered image after a
        /// successful call to the <see cref="Render"/> method.
        ///
        /// The destination image must exist throughout the lifetime of this
        /// object unless another image replaces that image.
        /// </remarks>
        public Image DestinationImage { get; set; }

        /// <summary>
        /// Gets or sets the source noise map.
        /// </summary>
        public NoiseMap SourceNoiseMap { get; set; }

        /// <summary>
        /// Adds a gradient point to this gradient object.
        /// </summary>
        /// <param name="gradientPos">The position of this gradient point.</param>
        /// <param name="gradientColor">The color of this gradient point.</param>
        /// <remarks>
        /// No two gradient points can have the same position.
        /// 
        /// This object uses a color gradient to calculate the color for each
        /// pixel in the destination image according to the value from the
        /// corresponding position in the noise map.
        ///
        /// The gradient requires a minimum of two gradient points.
        ///
        /// The specified color value passed to this method has an alpha
        /// channel.  This alpha channel specifies how a pixel in the
        /// background image (if specified) is blended with the calculated
        /// color.  If the alpha value is high, this object weighs the blend
        /// towards the calculated color, and if the alpha value is low, this
        /// object weighs the blend towards the color from the corresponding
        /// pixel in the background image.
        /// </remarks>
        public void AddGradientPoint(double gradientPos, Color gradientColor)
        {
            gradient.AddGradientPoint(gradientPos, gradientColor);
        }

        /// <summary>
        /// Builds a grayscale gradient.
        /// </summary>
        /// <remarks>
        /// The original gradient is cleared and a grayscale gradient is
        /// created.
        /// 
        /// This color gradient contains the following gradient points:
        /// - -1.0 maps to black
        /// - 1.0 maps to white
        /// </remarks>
        public void BuildGrayscaleGradient()
        {
            ClearGradient();
            gradient.AddGradientPoint(-1D, new Color(0, 0, 0, 255));
            gradient.AddGradientPoint(1D, new Color(255, 255, 255, 255));
        }

        /// <summary>
        /// Builds a color gradient suitable for terrain.
        /// </summary>
        /// <remarks>
        /// The original gradient is cleared and a terrain gradient is
        /// created.
        /// 
        /// This gradient color at position 0.0 is the "sea level".  Above
        /// that value, the gradient contains greens, browns, and whites.
        /// Below that value, the gradient contains various shades of blue.
        /// </remarks>
        public void BuildTerrainGradient()
        {
            ClearGradient();
            gradient.AddGradientPoint(-1.00, new Color(0, 0, 128, 255));
            gradient.AddGradientPoint(-0.20, new Color(32, 64, 128, 255));
            gradient.AddGradientPoint(-0.04, new Color(64, 96, 192, 255));
            gradient.AddGradientPoint(-0.02, new Color(192, 192, 128, 255));
            gradient.AddGradientPoint(0.00, new Color(0, 192, 0, 255));
            gradient.AddGradientPoint(0.25, new Color(192, 192, 0, 255));
            gradient.AddGradientPoint(0.50, new Color(160, 96, 64, 255));
            gradient.AddGradientPoint(0.75, new Color(128, 255, 255, 255));
            gradient.AddGradientPoint(1.00, new Color(255, 255, 255, 255));
        }

        /// <summary>
        /// Clears the color gradient.
        /// </summary>
        /// <remarks>
        /// Before calling the <see cref="Render"/> method, the application must specify a
        /// new color gradient with at least two gradient points.
        /// </remarks>
        public void ClearGradient()
        {
            gradient.ClearGradientPoints();
        }

        /// <summary>
        /// Renders the destination image using the contents of the source
        /// noise map and an optional background image.
        /// </summary>
        /// <remarks>
        /// The background image and the destination image can safely refer to
        /// the same image, although in this case, the destination image is
        /// irretrievably blended into the background image.
        /// </remarks>
        public void Render()
        {
            if (SourceNoiseMap == null ||
                DestinationImage == null ||
                SourceNoiseMap.Width <= 0 ||
                SourceNoiseMap.Height <= 0 ||
                gradient.PointCount < 2)
            {
                throw new InvalidOperationException("The renderer isn't propertly set up.");
            }

            var width = SourceNoiseMap.Width;
            var height = SourceNoiseMap.Height;

            // If a background image was provided, make sure it is the same size the
            // source noise map.
            if (BackgroundImage != null)
            {
                if (BackgroundImage.Width != width ||
                    BackgroundImage.Height != height)
                {
                    throw new ArgumentException("The provided background image isn't the same size as the source NoiseMap.");
                }
            }

            // Create the destination image.  It is safe to reuse it if this is also the
            // background image.
            if (DestinationImage != BackgroundImage)
                DestinationImage.SetSize(height, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float source = SourceNoiseMap[x, y];

                    // Get the color based on the value at the current point in the noise
                    // map.
                    Color destColor = gradient.GetColor(source);

                    // If lighting is enabled, calculate the light intensity based on the
                    // rate of change at the current point in the noise map.
                    double lightIntensity;
                    if (EnableLight)
                    {
                        // Calculate the positions of the current point's four-neighbors.
                        int xLeftOffset, xRightOffset;
                        int yUpOffset, yDownOffset;
                        if (EnableWrap)
                        {
                            if (x == 0)
                            {
                                xLeftOffset = width - 1;
                                xRightOffset = 1;
                            }
                            else if (x == width - 1)
                            {
                                xLeftOffset = -1;
                                xRightOffset = -(width - 1);
                            }
                            else
                            {
                                xLeftOffset = -1;
                                xRightOffset = 1;
                            }
                            if (y == 0)
                            {
                                yDownOffset = height - 1;
                                yUpOffset = 1;
                            }
                            else if (y == height - 1)
                            {
                                yDownOffset = -1;
                                yUpOffset = -(height - 1);
                            }
                            else
                            {
                                yDownOffset = -1;
                                yUpOffset = 1;
                            }
                        }
                        else
                        {
                            if (x == 0)
                            {
                                xLeftOffset = 0;
                                xRightOffset = 1;
                            }
                            else if (x == width - 1)
                            {
                                xLeftOffset = -1;
                                xRightOffset = 0;
                            }
                            else
                            {
                                xLeftOffset = -1;
                                xRightOffset = 1;
                            }
                            if (y == 0)
                            {
                                yDownOffset = 0;
                                yUpOffset = 1;
                            }
                            else if (y == height - 1)
                            {
                                yDownOffset = -1;
                                yUpOffset = 0;
                            }
                            else
                            {
                                yDownOffset = -1;
                                yUpOffset = 1;
                            }
                        }

                        yDownOffset *= SourceNoiseMap.Width;
                        yUpOffset *= SourceNoiseMap.Width;

                        // Get the noise value of the current point in the source noise map
                        // and the noise values of its four-neighbors.
                        var nc = (double)(SourceNoiseMap[x, y]);
                        var nl = (double)(SourceNoiseMap[x + xLeftOffset, y]);
                        var nr = (double)(SourceNoiseMap[x + xRightOffset, y]);
                        var nd = (double)(SourceNoiseMap[x, y + yDownOffset]);
                        var nu = (double)(SourceNoiseMap[x, y + yUpOffset]);

                        // Now we can calculate the lighting intensity.
                        lightIntensity = CalcLightIntensity(nc, nl, nr, nd, nu);
                        lightIntensity *= LightBrightness;
                    }
                    else
                    {
                        // These values will apply no lighting to the destination image.
                        lightIntensity = 1.0;
                    }

                    // Get the current background color from the background image.
                    Color backgroundColor;
                    if (BackgroundImage != null)
                        backgroundColor = BackgroundImage[x, y];
                    else
                        backgroundColor = new Color(255, 255, 255, 255);

                    // Blend the destination color, background color, and the light
                    // intensity together, then update the destination image with that
                    // color.
                    DestinationImage[x, y] = CalcDestinationColor(destColor, backgroundColor, lightIntensity);
                }
            }
        }

        /// <summary>
        /// Calculates the destination color.
        /// </summary>
        /// <param name="sourceColor">The source color generated from the color
        /// gradient.</param>
        /// <param name="backgroundColor">The color from the background image at the
        /// corresponding position.</param>
        /// <param name="lightValue">The intensity of the light at that position.</param>
        /// <returns>The destination color.</returns>
        Color CalcDestinationColor(Color sourceColor, Color backgroundColor, double lightValue)
        {
            var sourceRed = (double)sourceColor.Red / 255.0;
            var sourceGreen = (double)sourceColor.Green / 255.0;
            var sourceBlue = (double)sourceColor.Blue / 255.0;
            var sourceAlpha = (double)sourceColor.Alpha / 255.0;
            var backgroundRed = (double)backgroundColor.Red / 255.0;
            var backgroundGreen = (double)backgroundColor.Green / 255.0;
            var backgroundBlue = (double)backgroundColor.Blue / 255.0;

            // First, blend the source color to the background color using the alpha
            // of the source color.
            var red = NoiseMath.Linear(backgroundRed, sourceRed, sourceAlpha);
            var green = NoiseMath.Linear(backgroundGreen, sourceGreen, sourceAlpha);
            var blue = NoiseMath.Linear(backgroundBlue, sourceBlue, sourceAlpha);

            if (EnableLight)
            {
                // Now calculate the light color.
                var lightRed = lightValue * (double)LightColor.Red / 255.0;
                var lightGreen = lightValue * (double)LightColor.Green / 255.0;
                var lightBlue = lightValue * (double)LightColor.Blue / 255.0;

                // Apply the light color to the new color.
                red *= lightRed;
                green *= lightGreen;
                blue *= lightBlue;
            }

            // Clamp the color channels to the (0..1) range.
            red = NoiseMath.Clamp(red, 0, 1);
            green = NoiseMath.Clamp(green, 0, 1);
            blue = NoiseMath.Clamp(blue, 0, 1);

            // Rescale the color channels to the (0..255) range and return
            // the new color.
            Color newColor = new Color(
                (byte)((int)(red * 255) & 0xff),
                (byte)((int)(green * 255) & 0xff),
                (byte)((int)(blue * 255) & 0xff),
                Math.Max(sourceColor.Alpha, backgroundColor.Alpha));
            return newColor;
        }

        /// <summary>
        /// Calculates the intensity of the light given some elevation values.
        /// </summary>
        /// <param name="center">Elevation of the center point.</param>
        /// <param name="left">Elevation of the point directly left of the center
        /// point.</param>
        /// <param name="right">Elevation of the point directly right of the center
        /// point.</param>
        /// <param name="down">Elevation of the point directly below the center
        /// point.</param>
        /// <param name="up">Elevation of the point directly above the center point.</param>
        /// <returns>The calculated light intensity.</returns>
        /// <remarks>
        /// These values come directly from the noise map.
        /// </remarks>
        double CalcLightIntensity(double center, double left, double right, double down, double up)
        {
            // Recalculate the sine and cosine of the various light values if
            // necessary so it does not have to be calculated each time this method is
            // called.
            if (recalcLightValues)
            {
                cosAzimuth = Math.Cos(LightAzimuth * NoiseMath.DegToRad);
                sinAzimuth = Math.Sin(LightAzimuth * NoiseMath.DegToRad);
                cosElevation = Math.Cos(LightElevation * NoiseMath.DegToRad);
                sinElevation = Math.Sin(LightElevation * NoiseMath.DegToRad);
                recalcLightValues = false;
            }

            // Now do the lighting calculations.
            const double I_MAX = 1.0;
            var io = I_MAX * NoiseMath.Sqrt2 * sinElevation / 2.0;
            var ix = (I_MAX - io) * lightContrast * NoiseMath.Sqrt2 * cosElevation * cosAzimuth;
            var iy = (I_MAX - io) * lightContrast * NoiseMath.Sqrt2 * cosElevation * sinAzimuth;
            var intensity = Math.Max(ix * (left - right) + iy * (down - up) + io, 0);
            return intensity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageRenderer()
        {
            gradient = new GradientColor();

            EnableLight = false;
            EnableWrap = false;
            LightAzimuth = 45D;
            LightBrightness = 1D;
            LightColor = new Color(255, 255, 255, 255);
            LightContrast = 1D;
            LightElevation = 45D;
            LightIntensity = 1D;

            recalcLightValues = true;
        }
    }
}
