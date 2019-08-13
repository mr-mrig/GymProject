using System;

namespace GymProject.Domain.Base
{

    /// <summary>
    /// To be used to globally define the type of the Ids
    /// </summary>
    public class IdType
    {




        public long Id { get; private set; }


        public IdType(long id)
        {
            Id = id;

            if (Id <= 0)
                throw new ArgumentException($"The Id must be a positive number. Value provided: {Id.ToString()}");
        }




        /// <summary>
        /// Overridden method, prints the Id as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj) => obj is IdType type && Id == type.Id;

        public override int GetHashCode() => base.GetHashCode();

        #region IdType Vs IdType Operators


        public static bool operator ==(IdType left, IdType right) => left?.Id == right?.Id;

        public static bool operator !=(IdType left, IdType right) => !(left?.Id == right?.Id);

        public static bool operator >(IdType left, IdType right) => left?.Id > right?.Id;

        public static bool operator <(IdType left, IdType right) => left?.Id < right?.Id;

        public static bool operator >=(IdType left, IdType right) => left?.Id >= right?.Id;

        public static bool operator <=(IdType left, IdType right) => left?.Id <= right?.Id;
        #endregion


        #region IdType Vs int Operators

        public static IdType operator +(IdType left, long right)
        {
            if (left == null)
                throw new ArgumentException($"Cannot perform arithmetic operations on null IdType objects");

            return new IdType(left.Id + right);
        }

        public static IdType operator -(IdType left, long right)
        {
            if (left == null)
                throw new ArgumentException($"Cannot perform arithmetic operations on null IdType objects");

            return new IdType(left.Id - right);
        }

        public static bool operator ==(IdType left, long right) => left?.Id == right;

        public static bool operator !=(IdType left, long right) => !(left?.Id == right);

        public static bool operator >(IdType left, long right) => left?.Id > right;

        public static bool operator <(IdType left, long right) => left?.Id < right;

        public static bool operator >=(IdType left, long right) => left?.Id >= right;

        public static bool operator <=(IdType left, long right) => left?.Id <= right;
        #endregion

    }

}
