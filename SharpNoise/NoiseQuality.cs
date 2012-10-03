namespace SharpNoise
{
    /// <summary>
    /// Noise quality
    /// </summary>
    public enum NoiseQuality
    {
        /// <summary>
        /// Generates coherent noise quickly.  When a coherent-noise function with
        /// this quality setting is used to generate a bump-map image, there are
        /// noticeable "creasing" artifacts in the resulting image.  This is
        /// because the derivative of that function is discontinuous at integer
        /// boundaries.
        /// </summary>
        Fast,

        /// <summary>
        /// Generates standard-quality coherent noise.  When a coherent-noise
        /// function with this quality setting is used to generate a bump-map
        /// image, there are some minor "creasing" artifacts in the resulting
        /// image.  This is because the second derivative of that function is
        /// discontinuous at integer boundaries.
        /// </summary>
        Standard,

        /// <summary>
        /// Generates the best-quality coherent noise.  When a coherent-noise
        /// function with this quality setting is used to generate a bump-map
        /// image, there are no "creasing" artifacts in the resulting image.  This
        /// is because the first and second derivatives of that function are
        /// continuous at integer boundaries.
        /// </summary>
        Best,
    }
}
