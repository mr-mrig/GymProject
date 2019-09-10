using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class WeekdayEnum : Enumeration
    {


        public const int AllTheWeek = 7;

        #region Enum

        public static WeekdayEnum Generic = new WeekdayEnum(0, "Generic", "NA");
        public static WeekdayEnum Monday = new WeekdayEnum(1, "Monday", "Mon");
        public static WeekdayEnum Tuesday = new WeekdayEnum(2, "Tuesday", "Tue");
        public static WeekdayEnum Wednesday = new WeekdayEnum(3, "Wednesday", "Wed");
        public static WeekdayEnum Thursday = new WeekdayEnum(4, "Thursday", "Thu");
        public static WeekdayEnum Friday = new WeekdayEnum(5, "Friday", "Fri");
        public static WeekdayEnum Saturday = new WeekdayEnum(6, "Saturday", "Sat");
        public static WeekdayEnum Sunday = new WeekdayEnum(7, "Sunday", "Sun");
        #endregion



        /// <summary>
        /// Day abbreviation - Mon, Tue, Wed, Thu, Fri, Sat, Sun
        /// </summary>
        public string Abbreviation { get; private set; }



        #region Ctors


        private WeekdayEnum() : base(0, null)
        {

        }


        public WeekdayEnum(int id, string name, string abbreviation) : base(id, name)
        {
            Abbreviation = abbreviation;
        }
        #endregion



        /// <summary>
        /// Checks wether this weekday is generic - IE: not assigned to a specific weekday
        /// </summary>
        /// <returns>True if generic weekday</returns>
        public bool IsGeneric() => this == Generic;


        #region Object Override

        public static bool operator ==(WeekdayEnum left, WeekdayEnum right)
        {
            if (left is null || right is null)
                return false;

            return left.Id == right.Id;
        }


        public static bool operator !=(WeekdayEnum left, WeekdayEnum right)
        {
            if (left is null || right is null)
                return false;

            return left.Id != right.Id;
        }

        public override bool Equals(object obj) => obj is MonthsEnum right && right.Id == Id;

        public override int GetHashCode() => base.GetHashCode();
        #endregion



        #region Enum methods

        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<WeekdayEnum> List() =>
            new[] { Generic, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static WeekdayEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static WeekdayEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id== id);
        }
        #endregion

    }
}