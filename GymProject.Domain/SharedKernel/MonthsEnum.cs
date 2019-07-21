using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.DietDomain
{
    public class MonthsEnum : Enumeration
    {

        #region enum

        public static MonthsEnum Generic = new MonthsEnum(0, "Generic", "NA");
        public static MonthsEnum January = new MonthsEnum(1, "January", "Jan");
        public static MonthsEnum February = new MonthsEnum(2, "February", "Feb");
        public static MonthsEnum March = new MonthsEnum(3, "March", "Mar");
        public static MonthsEnum April = new MonthsEnum(4, "April", "Apr");
        public static MonthsEnum May = new MonthsEnum(5, "May", "May");
        public static MonthsEnum June = new MonthsEnum(6, "June", "Jun");
        public static MonthsEnum July = new MonthsEnum(7, "July", "Jul");
        public static MonthsEnum August = new MonthsEnum(8, "August", "Aug");
        public static MonthsEnum September = new MonthsEnum(9, "September", "Sep");
        public static MonthsEnum October = new MonthsEnum(10, "October", "Oct");
        public static MonthsEnum November = new MonthsEnum(11, "November", "Nov");
        public static MonthsEnum December = new MonthsEnum(12, "December", "Dec");
        #endregion



        /// <summary>
        /// Day abbreviation - Jan, Feb, Mar etc.
        /// </summary>
        public string Abbreviation { get; private set; }



        #region Ctors

        public MonthsEnum(int id, string name, string abbreviation) : base(id, name)
        {
            Abbreviation = abbreviation;
        }
        #endregion



        public static bool operator ==(MonthsEnum left, MonthsEnum right)
        {
            if (left == null || right == null)
                return false;

            return left.Id == right.Id;
        }


        public static bool operator !=(MonthsEnum left, MonthsEnum right)
        {
            if (left == null || right == null)
                return false;

            return left.Id != right.Id;
        }



        #region Enum methods

        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<MonthsEnum> List() =>
            new[] { Generic, January, February, March, April, May, June, July, August, September, October, November, December};


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static MonthsEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static MonthsEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }
        #endregion

    }
}