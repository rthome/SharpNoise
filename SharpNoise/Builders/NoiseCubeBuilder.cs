using System;
using System.Threading;
using System.Threading.Tasks;

using SharpNoise.Modules;

namespace SharpNoise.Builders
{
    /// <summary>
    /// Abstract base class for a noise-cube builder
    /// </summary>
    /// <remarks>
    /// A builder class builds a noise cube by filling it with coherent-noise
    /// values.
    /// 
    /// To build the noise cube, perform the following steps:
    /// - Pass the bounding coordinates to the SetBounds() method.
    /// - Pass the noise cube size, in points, to the <see cref="SetDestSize"/> method.
    /// - Pass a NoiseCube object to the <see cref="DestNoiseCube"/> property.
    /// - Pass a noise module (derived from <see cref="Module"/>) to the
    ///   <see cref="SourceModule"/> property.
    /// - Call the <see cref="Build"/> method.
    /// 
    /// Note that SetBounds() is not defined in the abstract base class; it is
    /// only defined in the derived classes.  This is because each model uses
    /// a different coordinate system.
    /// </remarks>
    public abstract class NoiseCubeBuilder
    {
        protected int destWidth, destHeight, destDepth;

        /// <summary>
        /// Gets or sets the source module
        /// </summary>
        /// <remarks>
        /// This object fills in a noise cube with the coherent-noise values
        /// from this source module.
        /// </remarks>
        public Module SourceModule { get; set; }

        /// <summary>
        /// Gets or sets the destination noise cube
        /// </summary>
        /// <remarks>
        /// The destination noise cube will contain the coherent-noise values
        /// from this noise cube after a successful call to the <see cref="Build"/> method.
        /// </remarks>
        public NoiseCube DestNoiseCube { get; set; }

        /// <summary>
        /// Sets the size of the destination noise cube.
        /// </summary>
        /// <param name="width">The width of the destination noise cube, in points.</param>
        /// <param name="height">The height of the destination noise cube, in points.</param>
        /// <param name="depth">The depth of the destination noise cube, in points.</param>
        /// <remarks>
        /// This method does not change the size of the destination noise cube
        /// until the <see cref="Build"/> method is called.
        /// </remarks>
        public void SetDestSize(int width, int height, int depth)
        {
            destWidth = width;
            destHeight = height;
            destDepth = depth;
        }

        protected abstract void PrepareBuild();
        protected abstract void BuildImpl(CancellationToken cancellationToken);

        /// <summary>
        /// Builds the noise cube.
        /// </summary>
        /// <remarks>
        /// The width and height values specified by <see cref="SetDestSize"/> must not
        /// exceed the maximum possible width and height for the noise cube.
        /// 
        /// The original contents of the destination noise cube is destroyed.
        /// </remarks>
        public void Build()
        {
            Build(CancellationToken.None);
        }

        /// <summary>
        /// Builds the noise cube.
        /// </summary>
        /// <remarks>
        /// The width and height values specified by <see cref="SetDestSize"/> must not
        /// exceed the maximum possible width and height for the noise cube.
        /// 
        /// The original contents of the destination noise cube is destroyed.
        /// </remarks>
        public void Build(CancellationToken cancellationToken)
        {
            PrepareBuild();

            BuildImpl(cancellationToken);
        }

        /// <summary>
        /// Builds the noise cube.
        /// </summary>
        /// <remarks>
        /// The width and height values specified by <see cref="SetDestSize"/> must not
        /// exceed the maximum possible width and height for the noise cube.
        /// 
        /// The original contents of the destination noise cube is destroyed.
        /// </remarks>
        public async Task BuildAsync(CancellationToken cancellationToken)
        {
            PrepareBuild();

            await Task.Factory.StartNew(() => BuildImpl(cancellationToken), cancellationToken,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
