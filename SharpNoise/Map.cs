using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpNoise
{
    /// <summary>
    /// Base class for 2D arrays like NoiseMap and Image
    /// </summary>
    /// <typeparam name="T">The type of the data within the map</typeparam>
    public abstract class Map<T> where T : struct
    {
        /// <summary>
        /// Provides read-only access to a single line in the map.
        /// </summary>
        public sealed class LineReader : IEnumerator<T>, IEnumerable<T>
        {
            int currentIndex;
            T currentItem;

            readonly Map<T> map;
            readonly int lowerIndex, upperIndex;

            public T Current
            {
                get { return currentItem; }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get { return currentItem; }
            }

            public bool MoveNext()
            {
                if (++currentIndex >= upperIndex)
                    return false;
                else
                    currentItem = map.values[currentIndex];
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException("The Reset operation is not supported.");
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="map">The map to read from.</param>
            /// <param name="lower">The lower (start) index of the row.</param>
            /// <param name="upper">The upper (end) index of the row.</param>
            internal LineReader(Map<T> map, int lower, int upper)
            {
                this.map = map;
                lowerIndex = lower;
                upperIndex = upper;

                currentIndex = lowerIndex - 1;
            }
        }

        protected T[] values;

        /// <summary>
        /// Gets or sets the border value for all positions outside the map
        /// </summary>
        public T BorderValue { get; set; }

        /// <summary>
        /// Gets the height of the Map
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width of the Map
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the amount of memory allocated by the Map (in bytes)
        /// </summary>
        public abstract int UsedMemory { get; }

        /// <summary>
        /// Gets a value indicating whether the Map is empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return values == null;
            }
        }

        /// <summary>
        /// Constructor for an empty Map.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="width">The width of the new noise map.</param>
        /// <param name="height">The height of the new noise map.</param>
        /// <remarks>
        /// Creates a map with uninitialized values.
        ///
        /// It is considered an error if the specified dimensions are not
        /// positive.
        /// </remarks>
        public Map(int width, int height)
        {
            SetSize(height, width);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other">The Map to copy</param>
        public Map(Map<T> other)
        {
            SetSize(other.Height, other.Width);
            other.values.CopyTo(values, 0);
            BorderValue = other.BorderValue;
        }

        /// <summary>
        /// Calculates the index to a slab at the specified position.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <returns>The index to a slab at the position ( x, y )</returns>
        /// <remarks>
        /// The coordinates must exist within the bounds of the map.
        ///
        /// This method does not perform bounds checking so be careful when
        /// calling it.
        /// 
        /// This method will throw an InvalidOperationException if the Map is empty.
        /// </remarks>
        protected int GetIndex(int x, int y)
        {
            return x + Width * y;
        }

        /// <summary>
        /// Calculates the index to a slab at the specified row.
        /// </summary>
        /// <param name="row">The row, or y coordinate.</param>
        /// <returns>The index to a slab at the position ( 0, row )</returns>
        /// <remarks>
        /// The coordinates must exist within the bounds of the  map.
        ///
        /// This method does not perform bounds checking so be careful when
        /// calling it.
        /// 
        /// This method will throw an InvalidOperationException if the Map is empty.
        /// </remarks>
        protected int GetIndex(int row)
        {
            return GetIndex(0, row);
        }

        /// <summary>
        /// Clears and resets the map
        /// </summary>
        protected void ResetMap()
        {
            values = null;
            Width = 0;
            Height = 0;
            BorderValue = default(T);
        }

        /// <summary>
        /// Create a LineReader for the specified row in the Map.
        /// </summary>
        /// <param name="row">The index of the row that will be read.</param>
        /// <returns>Returns a new LineReader.</returns>
        public LineReader GetLineReader(int row)
        {
            if (row < 0 || row >= Height)
                throw new ArgumentException("row must be greater than 0 and less than Height.");

            var lower = GetIndex(row);
            return new LineReader(this, lower, lower + Width);
        }

        /// <summary>
        /// Creates LineReaders for all rows in the Map.
        /// </summary>
        /// <returns>Returns LineReaders for all rows in the Map from 0 to Height.</returns>
        public IEnumerable<LineReader> GetLineReaders()
        {
            for (var row = 0; row < Height; row++)
                yield return GetLineReader(row);
        }

        /// <summary>
        /// Clears the map to a specified value.
        /// </summary>
        /// <param name="value">
        /// The value that all positions within the noise map are
        /// cleared to.
        /// </param>
        public void Clear(T value)
        {
            if (values != null)
            {
                for (var i = 0; i < values.Length; i++)
                    values[i] = value;
            }
        }

        /// <summary>
        /// Sets the size of the Map
        /// </summary>
        /// <param name="height">The new height of the Map</param>
        /// <param name="width">The new width of the Map</param>
        /// <remarks>
        /// After changing the size of the Map,
        /// the contents of the map are undefined.
        /// </remarks>
        public void SetSize(int height, int width)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("width and height cannot be less than 0.");

            if (width == 0 || height == 0)
                ResetMap();
            else
            {
                values = new T[width * height];
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// Gets or sets a value at the specified position
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <returns>The value at that position</returns>
        /// <remarks>
        /// This calls <see cref="SetValue()"/> or <see cref="GetValue()"/>
        /// </remarks>
        public T this[int x, int y]
        {
            get
            {
                return GetValue(x, y);
            }
            set
            {
                SetValue(x, y, value);
            }
        }

        /// <summary>
        /// Returns a value from the specified position in the map.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <returns>The value at that position.</returns>
        /// <remarks>
        /// This method returns the border value if the coordinates exist
        /// outside of the map.
        /// </remarks>
        public T GetValue(int x, int y)
        {
            if (values != null)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    return values[GetIndex(x, y)];
                }
            }

            return BorderValue;
        }

        /// <summary>
        /// Sets a value at a specified position in the map.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <param name="value">The value to set at the given position.</param>
        /// <remarks>
        /// This method does nothing if the noise map object is empty or the
        /// position is outside the bounds of the map.
        /// </remarks>
        public void SetValue(int x, int y, T value)
        {
            if (values != null)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    values[GetIndex(x, y)] = value;
                }
            }
        }
    }
}
