using System;
using System.Collections.Generic;

namespace GymProject.Domain.Base
{

    /// <summary>
    /// To be used to globally define the type of the Ids
    /// </summary>
    public class IdTypeValue : ValueObject
    {




        public long Id { get; private set; }


        #region Ctors

        private IdTypeValue(long id)
        {
            Id = id;

            if (Id <= 0)
                throw new ArgumentException($"The Id must be a positive number. Value provided: {Id.ToString()}");
        }
        #endregion


        #region Factories

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="id">The id as a long integer</param>
        /// <returns>The new IdType instance</returns>
        public static IdTypeValue Create(long id) => new IdTypeValue(id);
        #endregion




        /// <summary>
        /// Overridden method, prints the Id as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj) => obj is IdTypeValue type && Id == type.Id;

        public override int GetHashCode() => base.GetHashCode();


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
        }



        #region IdType Vs IdType Operators


        public static bool operator ==(IdTypeValue left, IdTypeValue right) => left?.Id == right?.Id;

        public static bool operator !=(IdTypeValue left, IdTypeValue right) => !(left?.Id == right?.Id);

        public static bool operator >(IdTypeValue left, IdTypeValue right) => left?.Id > right?.Id;

        public static bool operator <(IdTypeValue left, IdTypeValue right) => left?.Id < right?.Id;

        public static bool operator >=(IdTypeValue left, IdTypeValue right) => left?.Id >= right?.Id;

        public static bool operator <=(IdTypeValue left, IdTypeValue right) => left?.Id <= right?.Id;
        #endregion


        #region IdType Vs int Operators

        public static IdTypeValue operator +(IdTypeValue left, long right)
        {
            if (left == null)
                throw new ArgumentException($"Cannot perform arithmetic operations on null IdType objects");

            return Create(left.Id + right);
        }

        public static IdTypeValue operator -(IdTypeValue left, long right)
        {
            if (left == null)
                throw new ArgumentException($"Cannot perform arithmetic operations on null IdType objects");

            return Create(left.Id - right);
        }

        public static bool operator ==(IdTypeValue left, long right) => left?.Id == right;

        public static bool operator !=(IdTypeValue left, long right) => !(left?.Id == right);

        public static bool operator >(IdTypeValue left, long right) => left?.Id > right;

        public static bool operator <(IdTypeValue left, long right) => left?.Id < right;

        public static bool operator >=(IdTypeValue left, long right) => left?.Id >= right;

        public static bool operator <=(IdTypeValue left, long right) => left?.Id <= right;
        #endregion


    }

}
