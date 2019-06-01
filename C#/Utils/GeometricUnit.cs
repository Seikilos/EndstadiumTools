using System;

namespace Endstadium.Utils
{
    /// <summary>
    /// Unit class wrapping around geometry results which can be converted to all types.
    /// This explicit interface prevents variables from being named like "double lengthInMeters"
    /// Note: This should move into Core after review.
    /// </summary>
    public class GeometricUnit : IComparable<GeometricUnit>
    {
        /// <summary>
        /// Returns the geometric unit in meters.
        /// </summary>
        public double Meters { get; }

        /// <summary>
        /// Returns the geometric unit in kilometers
        /// </summary>
        public double Kilometers
        {
            get
            {
                return this.Meters / 1000.0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometricUnit" /> class.
        /// </summary>
        private GeometricUnit(double meters)
        {
            this.Meters = meters;
        }

        /// <summary>
        /// Creates a unit from kilometers.
        /// </summary>
        /// <param name="km">Unit in kilometers</param>
        /// <returns></returns>
        public static GeometricUnit FromKilometers(double km)
        {
            return new GeometricUnit(km * 1000.0);
        }

        /// <summary>
        /// Creates a unit from meters.
        /// </summary>
        /// <param name="m">Unit in meters</param>
        /// <returns></returns>
        public static GeometricUnit FromMeters(double m)
        {
            return new GeometricUnit(m);
        }

        #region Equality Members

        /// <summary>
        /// Compares to units.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(GeometricUnit other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is GeometricUnit other && this.Equals(other);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.Meters.GetHashCode();
        }

        #endregion

        #region Relational Operators

        /// <summary>
        /// Compares unit to another unit.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(GeometricUnit other)
        {
            return this.Meters.CompareTo(other.Meters);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value is less than another <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        public static bool operator <(GeometricUnit left, GeometricUnit right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value is greater than another <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        public static bool operator >(GeometricUnit left, GeometricUnit right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value is less than or equal to another <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        public static bool operator <=(GeometricUnit left, GeometricUnit right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value is greater than or equal to another <see cref="T:C4I.Logic.Domain.Sensors.Dummy.GeometricUnit" /> value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        public static bool operator >=(GeometricUnit left, GeometricUnit right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion
    }
}
