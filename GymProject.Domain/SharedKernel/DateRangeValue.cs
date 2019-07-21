using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class DateRangeValue : ValueObject
    {


        /// <summary>
        /// The date range left boundary
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// The date range right boundary
        /// </summary>
        public DateTime End { get; private set; }




        #region Ctors

        private DateRangeValue(DateTime? startDate = null, DateTime? endDate = null)
        {
            Start = startDate.Value;
            End = endDate.Value;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new date range 
        /// </summary>
        /// <param name="startDate">The range starting date</param>
        /// <param name="endDate">The range end date</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeBetween(DateTime startDate, DateTime endDate)
            => new DateRangeValue(startDate, endDate);



        /// <summary>
        /// Factory for creating a new date range with no end date
        /// </summary>
        /// <param name="startDate">The range starting date</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeStartingFrom(DateTime startDate)
            => new DateRangeValue(startDate, null);


        /// <summary>
        /// Factory for creating a new date range with no starting date
        /// </summary>
        /// <param name="endDate">The range end date</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeUpTo(DateTime endDate)
            => new DateRangeValue(null, endDate);

        #endregion



        #region Business Methods

        /// <summary>
        /// Reschedules the date range to the start date specified.
        /// </summary>
        /// <param name="newStartDate">The new start date - null if not to be changed</param>
        /// <returns>The DateRangeValue object which is the extension of the current one</returns>
        public DateRangeValue RescheduleStart(DateTime newStartDate)
        {
            return DateRangeValue.RangeBetween(newStartDate, End);
        }


        /// <summary>
        /// Reschedules the date range to the end date specified.
        /// </summary>
        /// <param name="newEndDate">The new end date</param>
        /// <returns>The DateRangeValue object which is the extension of the current one</returns>
        public DateRangeValue RescheduleEnd(DateTime newEndDate)
        {
            return DateRangeValue.RangeBetween(Start, newEndDate);
        }


        /// <summary>
        /// Checks wether the specified date is included in the range or not
        /// </summary>
        /// <param name="toCheck">The date to be checked</param>
        /// <returns>True if date inside DateRange boundaries</returns>
        public bool Includes(DateTime toCheck)
        {
            return toCheck > Start && toCheck < End;
        }


        /// <summary>
        /// Checks wether the specified daterange is included in the range or not
        /// </summary>
        /// <param name="toCheck">The date range to be checked</param>
        /// <returns>True if the DateRange encloses the one specified</returns>
        public bool Includes(DateRangeValue toCheck)
        {
            return Includes(toCheck.Start) && Includes(toCheck.End);
        }


        /// <summary>
        /// Checks wether two dateranges are overlapping
        /// </summary>
        /// <param name="toCheck">The date range to be checked</param>
        /// <returns>True if the DetaRange objects are overlapping</returns>
        public bool Overlaps(DateRangeValue toCheck)
        {
            return toCheck.Includes(Start) || toCheck.Includes(End) || Includes(toCheck);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public int CompareTo(DateRangeValue another)
        {
            if (!Start.Equals(another.Start))
                return Start.CompareTo(another.Start);

            return End.CompareTo(another.End);
        }


        /// <summary>
        /// Get the gap between two date ranges
        /// </summary>
        /// <param name="another">The DateRange to be processed</param>
        /// <returns>A new DateRange instance between the two ranges - NULL if overlapping ranges</returns>
        public DateRangeValue GepBetween(DateRangeValue another)
        {
            if (Overlaps(another))
                return null;

            DateRangeValue lower, higher;

            if (CompareTo(another) < 0)
            {
                lower = this;
                higher = another;
            }
            else
            {
                lower = another;
                higher = this;
            }

            return RangeBetween(lower.End.AddDays(1), higher.Start.AddDays(-1));
        }


        /// <summary>
        /// Checks if all the properties are null
        /// </summary>
        /// <returns>True if invalid state</returns>
        public bool CheckNullState()
            => GetAtomicValues().All(x => x == null);
        #endregion



        #region Private Methods
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }

    }
}
