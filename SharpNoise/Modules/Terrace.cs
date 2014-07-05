using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that maps the output value from a source module onto a
    /// terrace-forming curve.
    /// </summary>
    /// <remarks>
    /// This noise module maps the output value from the source module onto a
    /// terrace-forming curve.  The start of this curve has a slope of zero;
    /// its slope then smoothly increases.  This curve also contains
    /// control points which resets the slope to zero at that point,
    /// producing a "terracing" effect.
    /// 
    /// To add a control point to this noise module, call the
    /// <see cref="AddControlPoint"/> method.
    ///
    /// An application must add a minimum of two control points to the curve.
    /// If this is not done, the <see cref="GetValue"/> method fails.  The control points
    /// can have any value, although no two control points can have the same
    /// value.  There is no limit to the number of control points that can be
    /// added to the curve.
    ///
    /// This noise module clamps the output value from the source module if
    /// that value is less than the value of the lowest control point or
    /// greater than the value of the highest control point.
    ///
    /// This noise module is often used to generate terrain features such as
    /// your stereotypical desert canyon.
    ///
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class Terrace : Module
    {
        readonly List<double> controlPoints;

        /// <summary>
        /// Gets the number of control points on the terrace-forming curve.
        /// </summary>
        public int ControlPointCount { get { return controlPoints.Count; } }

        /// <summary>
        /// Enables or disables the inversion of the terrace-forming curve
        /// between the control points.
        /// </summary>
        public bool InvertTerraces { get; set; }

        /// <summary>
        /// Gets or sets all ControlPoints in the Module
        /// </summary>
        public IEnumerable<double> ControlPoints
        {
            get
            {
                return controlPoints.AsReadOnly();
            }
            set
            {
                controlPoints.Clear();
                controlPoints.AddRange(value);
            }
        }

        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return GetSourceModule(0); }
            set { SetSourceModule(0, value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Terrace()
            : base(1)
        {
            controlPoints = new List<double>();
            InvertTerraces = false;
        }

        /// <summary>
        /// Adds a control point to the terrace-forming curve.
        /// </summary>
        /// <param name="value">The value of the control point to add.</param>
        /// <remarks>
        /// No two control points can have the same value.
        /// 
        /// Two or more control points define the terrace-forming curve.  The
        /// start of this curve has a slope of zero; its slope then smoothly
        /// increases.  At the control points, its slope resets to zero.
        ///
        /// It does not matter which order these points are added.
        /// </remarks>
        public void AddControlPoint(double value)
        {
            if (controlPoints.Contains(value))
                throw new ArgumentException("Duplicate ControlPoint found. ControlPoints must be unique!");

            controlPoints.Add(value);
        }

        /// <summary>
        /// Deletes all the control points on the terrace-forming curve.
        /// </summary>
        public void ClearControlPoints()
        {
            controlPoints.Clear();
        }

        /// <summary>
        /// Creates a number of equally-spaced control points that range from
        /// -1 to +1.
        /// </summary>
        /// <param name="count">The number of control points to generate.</param>
        /// <remarks>
        /// The number of control points must be greater than or equal to
        /// 2.
        /// 
        /// The previous control points on the terrace-forming curve are
        /// deleted.
        /// 
        /// Two or more control points define the terrace-forming curve.  The
        /// start of this curve has a slope of zero; its slope then smoothly
        /// increases.  At the control points, its slope resets to zero.
        /// </remarks>
        public void MakeControlPoints(int count)
        {
            if (count < 2)
                throw new ArgumentException("There must be at least 2 ControlPoints");

            ClearControlPoints();

            var terraceStep = 2.0 / ((double)count - 1.0);
            var curValue = -1.0;
            for (var i = 0; i < (int)count; i++)
            {
                AddControlPoint(curValue);
                curValue += terraceStep;
            }
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
            // Get the output value from the source module.
            double sourceModuleValue = sourceModules[0].GetValue(x, y, z);

            // Find the first element in the control point array that has a value
            // larger than the output value from the source module.
            int indexPos;
            for (indexPos = 0; indexPos < ControlPointCount; indexPos++)
            {
                if (sourceModuleValue < controlPoints[indexPos])
                    break;
            }

            // Find the two nearest control points so that we can map their values
            // onto a quadratic curve.
            var index0 = NoiseMath.Clamp(indexPos - 1, 0, ControlPointCount - 1);
            var index1 = NoiseMath.Clamp(indexPos, 0, ControlPointCount - 1);

            // If some control points are missing (which occurs if the output value from
            // the source module is greater than the largest value or less than the
            // smallest value of the control point array), get the value of the nearest
            // control point and exit now.
            if (index0 == index1)
                return controlPoints[index1];

            // Compute the alpha value used for linear interpolation.
            var value0 = controlPoints[index0];
            var value1 = controlPoints[index1];
            var alpha = (sourceModuleValue - value0) / (value1 - value0);
            if (InvertTerraces)
            {
                alpha = 1.0 - alpha;
                NoiseMath.Swap(ref value0, ref value1);
            }

            // Squaring the alpha produces the terrace effect.
            alpha *= alpha;

            // Now perform the linear interpolation given the alpha value.
            return NoiseMath.Linear(value0, value1, alpha);
        }
    }
}
