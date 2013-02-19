﻿using System;

using SharpNoise.Modules;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Abstract base class for a noise-map builder
    /// </summary>
    /// <remarks>
    /// A builder class builds a noise map by filling it with coherent-noise
    /// values generated from the surface of a three-dimensional mathematical
    /// object.  Each builder class defines a specific three-dimensional
    /// surface, such as a cylinder, sphere, or plane.
    /// 
    /// A builder class describes these input values using a coordinate system
    /// applicable for the mathematical object (e.g., a latitude/longitude
    /// coordinate system for the spherical noise-map builder.)  It then
    /// "flattens" these coordinates onto a plane so that it can write the
    /// coherent-noise values into a two-dimensional noise map.
    /// 
    /// To build the noise map, perform the following steps:
    /// - Pass the bounding coordinates to the SetBounds() method.
    /// - Pass the noise map size, in points, to the <see cref="SetDestSize"/> method.
    /// - Pass a NoiseMap object to the <see cref="SetDestNoiseMap"/> method.
    /// - Pass a noise module (derived from <see cref="Module"/>) to the
    ///   <see cref="SetSourceModule"/> method.
    /// - Call the <see cref="Build"/> method.
    /// 
    /// Note that SetBounds() is not defined in the abstract base class; it is
    /// only defined in the derived classes.  This is because each model uses
    /// a different coordinate system.
    /// </remarks>
    public abstract class NoiseMapBuilder
    {
        /// <summary>
        /// Destination noise map size
        /// </summary>
        protected int destWidth, destHeight;

        // the callback action, will be called by build every time a new line is completed
        protected Action<NoiseMap.LineReader> callback;

        /// <summary>
        /// Builds the noise map.
        /// </summary>
        /// <remarks>
        /// The width and height values specified by <see cref="SetDestSize"/> must not
        /// exceed the maximum possible width and height for the noise map.
        /// 
        /// If a callback function was set, it will be called after each row is completed.
        /// 
        /// The original contents of the destination noise map is
        /// destroyed.
        /// </remarks>
        public abstract void Build();

        /// <summary>
        /// Gets or sets the source module
        /// </summary>
        /// <remarks>
        /// This object fills in a noise map with the coherent-noise values
        /// from this source module.
        /// </remarks>
        public Module SourceModule { get; set; }

        /// <summary>
        /// Gets or sets the destination noise map
        /// </summary>
        /// <remarks>
        /// The destination noise map will contain the coherent-noise values
        /// from this noise map after a successful call to the <see cref="Build"/> method.
        /// </remarks>
        public NoiseMap DestNoiseMap { get; set; }

        /// <summary>
        /// Sets the size of the destination noise map.
        /// </summary>
        /// <param name="width">
        /// The width of the destination noise map, in
        /// points.
        /// </param>
        /// <param name="height">
        /// The height of the destination noise map, in
        /// points.
        /// </param>
        /// <remarks>
        /// This method does not change the size of the destination noise map
        /// until the <see cref="Build"/> method is called.
        /// </remarks>
        public void SetDestSize(int width, int height)
        {
            destWidth = width;
            destHeight = height;
        }

        /// <summary>
        /// Sets a callback function that will be called each time
        /// a new row is filled with noise values.
        /// </summary>
        /// <param name="callback">The callback function.</param>
        /// <remarks>
        /// Set the callback to null to clear the callback.
        /// </remarks>
        public void SetCallback(Action<NoiseMap.LineReader> callback)
        {
            this.callback = callback;
        }
    }
}
