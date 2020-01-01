using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.Base
{

    /// <summary>
    /// It's just a label to identify Enities or Value Objects storing many-to-many relations
    /// This is needed to ensure EF Core consitence
    /// </summary>
    public abstract class RelationEntity
    {




        #region Abstract Methods

        /// <summary>
        /// Get the fields which univocally identify the relation - these should be the identifying FKs.
        /// </summary>
        /// <returns>An object representation of the atomic value</returns>
        protected abstract IEnumerable<object> GetIdentifyingFields();
        #endregion

        /// <summary>
        ///  Implements the <c>==</c> operator.
        /// </summary>
        /// <param name="left">The first relation object.</param>
        /// <param name="right">The second relation object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RelationEntity left, RelationEntity right)
        {
            if (left is null ^ right is null)
                return false;

            return left?.Equals(right) != false;
        }


        /// <summary>
        ///  Implements the <c>!=</c> operator.
        /// </summary>
        /// <param name="left">The first relation object.</param>
        /// <param name="right">The second relation object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RelationEntity left, RelationEntity right)
        {
            return !(left == right);
        }

        /// <summary>
        ///  Determines whether the specified <see cref="Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with the current <see cref="Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (RelationEntity)obj;
            var thisValues = GetIdentifyingFields().GetEnumerator();
            var otherValues = other.GetIdentifyingFields().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current is null ^ otherValues.Current is null)
                    return false;

                if (thisValues.Current != null &&
                    !thisValues.Current.Equals(otherValues.Current))
                    return false;
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
            return GetIdentifyingFields()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

    }
}
