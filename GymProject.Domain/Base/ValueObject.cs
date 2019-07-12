using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.Base
{



    /// <summary>
    ///     Provides a standard base class for facilitating comparison of value objects using all the object's properties.
    /// </summary>
    /// <remarks>
    ///     For a discussion of the implementation of Equals/GetHashCode, see 
    ///     http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    ///     and http://groups.google.com/group/sharp-architecture/browse_thread/thread/f76d1678e68e3ece?hl=en for 
    ///     an in depth and conclusive resolution.
    /// </remarks>
    public abstract class ValueObject
    {




        #region Abstract Methods

        /// <summary>
        /// Get the atomic values of the ValueObject.
        /// The internal values are private and should be accessed via this method.
        /// </summary>
        /// <returns>An object representation of the atomic value</returns>
        protected abstract IEnumerable<object> GetAtomicValues();
        #endregion

        /// <summary>
        ///     Implements the <c>==</c> operator.
        /// </summary>
        /// <param name="valueObject1">The first value object.</param>
        /// <param name="valueObject2">The second value object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left?.Equals(right) != false;
        }


        /// <summary>
        ///     Implements the <c>!=</c> operator.
        /// </summary>
        /// <param name="valueObject1">The first value object.</param>
        /// <param name="valueObject2">The second value object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with the current <see cref="Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            var thisValues = GetAtomicValues().GetEnumerator();
            var otherValues = other.GetAtomicValues().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current is null ^ otherValues.Current is null)
                {
                    return false;
                }

                if (thisValues.Current != null &&
                    !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <remarks>
        ///     This is used to provide the hash code identifier of an object using the signature
        ///     properties of the object; although it's necessary for NHibernate's use, this can
        ///     also be useful for business logic purposes and has been included in this base
        ///     class, accordingly. Since it is recommended that GetHashCode change infrequently,
        ///     if at all, in an object's lifetime, it's important that properties are carefully
        ///     selected which truly represent the signature of an object.
        /// </remarks>
        public override int GetHashCode()
        {
            return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
    }
}