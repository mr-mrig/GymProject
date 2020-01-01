using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class DateRangeValue : ValueObject
    {



        private DateTime _start;

        /// <summary>
        /// The date range left boundary
        /// If not set, DateTime.MinValue is used, hence no need to check for non-null properties
        /// </summary>
        public DateTime? Start
        {
            get => _start;
            private set => _start = value ?? DateTime.MinValue;     // This way the internal state will never be null
        }

        private DateTime _end;

        /// <summary>
        /// The date range right boundary
        /// If not set, DateTime.MaxValue is used, hence no need to check for non-null properties
        /// </summary>
        public DateTime? End
        {
            get => _end;
            private set => _end = value ?? DateTime.MaxValue;     // This way the internal state will never be null
        }




        #region Ctors


        private DateRangeValue()
        {

        }

        private DateRangeValue(DateTime? startDate) : this(startDate, null)
        {
        }


        private DateRangeValue(DateTime? startDate, DateTime? endDate)
        {
            Start = startDate ?? DateTime.MinValue;
            End = endDate ?? DateTime.MaxValue;

            if (!DateRangeValidChronologicalOrder())
                throw new ValueObjectInvariantViolationException($"StartDate must happen prior to EndDate when creating a {GetType().Name} object");

            if(!DateRangeNonInfinitePeriod())
                throw new ValueObjectInvariantViolationException($"The Date Range must have at least one finite boundary in a {GetType().Name} object");
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new date range 
        /// </summary>
        /// <param name="startDate">The range starting date - must precide endDate</param>
        /// <param name="endDate">The range end date - must be after startDate</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeBetween(DateTime? startDate, DateTime? endDate)
            => new DateRangeValue(startDate, endDate);



        /// <summary>
        /// Factory for creating a new date range with no end date
        /// </summary>
        /// <param name="startDate">The range starting date - must precide endDate</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeStartingFrom(DateTime? startDate)
            => new DateRangeValue(startDate);


        /// <summary>
        /// Factory for creating a new date range with no starting date
        /// </summary>
        /// <param name="endDate">The range end date - must be after startDate</param>
        /// /// <returns>The DateRangeValue instance</returns>
        public static DateRangeValue RangeUpTo(DateTime? endDate)
            => new DateRangeValue(null, endDate);

        #endregion



        #region Business Methods

        /// <summary>
        /// Checks wheter the start date has been set
        /// </summary>
        /// <returns>True if valid Start Date</returns>
        public bool IsLeftBounded() => Start != DateTime.MinValue;


        /// <summary>
        /// Checks wheter the end date has been set
        /// </summary>
        /// <returns>True if valid End Date</returns>
        public bool IsRightBounded() => End != DateTime.MaxValue;


        /// <summary>
        /// Reschedules the date range to the start date specified.
        /// </summary>
        /// <param name="newStartDate">The new start date - null if not to be changed</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if invalid state</exception>
        /// <returns>The DateRangeValue object which is the extension of the current one</returns>
        public DateRangeValue RescheduleStart(DateTime? newStartDate)
        {
            return RangeBetween(newStartDate, End);     // Throws
        }


        /// <summary>
        /// Reschedules the date range to the end date specified.
        /// </summary>
        /// <param name="newEndDate">The new end date</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if invalid state</exception>
        /// <returns>The DateRangeValue object which is the extension of the current one</returns>
        public DateRangeValue RescheduleEnd(DateTime? newEndDate)
        {
            return RangeBetween(Start, newEndDate);
        }


        /// <summary>
        /// Checks wether the specified date is included in the range or not
        /// IE: if one of the range boundares are null then it just check the other one
        /// </summary>
        /// <param name="toCheck">The date to be checked</param>
        /// <returns>True if date inside DateRange boundaries</returns>
        public bool Includes(DateTime toCheck)

            => IsDateBetween(toCheck, Start, End);


        /// <summary>
        /// Checks wether the specified date is strinctly included in the range or not.
        /// This is different from <see cref="Includes(DateTime)"/> as it does not work with null boundaries.
        /// </summary>
        /// <param name="toCheck">The date to be checked</param>
        /// <returns>True if date inside DateRange boundaries</returns>
        /// <exception cref="InvalidOperationException">If one of the boundaries is null</exception>
        public bool IncludesStrictly(DateTime toCheck)

            => IsDateStrictlyBetween(toCheck, Start.Value, End.Value);


        /// <summary>
        /// Checks wether the specified daterange is included in the range or not
        /// </summary>
        /// <param name="toCheck">The date range to be checked</param>
        /// <returns>True if the DateRange encloses the one specified</returns>
        public bool Includes(DateRangeValue toCheck)
        {
            return IncludesStrictly(toCheck.Start.Value) && IncludesStrictly(toCheck.End.Value);
        }


        /// <summary>
        /// Checks wether two dateranges are overlapping.
        /// If ranges share the same boundarie then they are considered as non-overlapping.
        /// </summary>
        /// <param name="toCheck">The date range to be checked</param>
        /// <returns>True if the DetaRange objects are overlapping</returns>
        public bool Overlaps(DateRangeValue toCheck)

            => AreOverlapping(toCheck, this);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public int CompareTo(DateRangeValue another)
        {
            if (!Start.Equals(another.Start))
                return Start.Value.CompareTo(another.Start.Value);

            return End.Value.CompareTo(another.End.Value);
        }


        /// <summary>
        /// Get the gap between two date ranges.
        /// The new DateRange spans between the day before and the day after of the proper edges.
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

            return RangeBetween(lower.End.Value.AddDays(1), higher.Start.Value.AddDays(-1));
        }


        /// <summary>
        /// Join two DateRanges, provided that they are contiguous - IE: there is no gap between the two
        /// </summary>
        /// <param name="another">The second DateRange to be joined</param>
        /// <returns>The DateRange union - null if not contiguous</returns>
        public DateRangeValue Join(DateRangeValue another)
        {
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

            if (higher.Start > lower.End.Value.AddDays(1))
                return null;

            return RangeBetween(lower.Start, higher.End);
        }


        /// <summary>
        /// Get the DateRange length [days, truncated]
        /// </summary>
        /// <returns>The days</returns>
        public int GetLength() => (End.Value.AddDays(1) - Start.Value).Days;    // Boundaries included -> need for the +1day

        #endregion


        #region Business Rules Specificatrions

        /// <summary>
        /// Chect the the date range boundaries respect the chronological order, IE: right boundary happens after left boundary
        /// </summary>
        /// <returns>True if the chronological is respected</returns>
        private bool DateRangeValidChronologicalOrder() => End >= Start;

        /// <summary>
        /// Check wether the date range is non-infinite, IE at least one of the two boundaries is finite
        /// </summary>
        /// <returns>True if the period is finite</returns>
        private bool DateRangeNonInfinitePeriod()
        {
            if(End == Start)
                return End != DateTime.MinValue && End != DateTime.MaxValue;

            return !(End == DateTime.MaxValue && Start == DateTime.MinValue);
        }
        #endregion


        #region Private Methods
        #endregion


        #region Static Utils

        /// <summary>
        /// Check whether the specified date is between the two edge ones.
        /// Returns true even when the date is strictly equal to one of the boundaries.
        /// Does not check for invalid dates.
        /// </summary>
        /// <param name="toCheck">The date to be checked</param>
        /// <param name="startDate">The left boundary of the period to check</param>
        /// <param name="endDate">The right boundary of the period to check</param>
        /// <returns>True if the date occurs between - or at - the edge dates</returns>
        public static bool IsDateStrictlyBetween(DateTime toCheck, DateTime startDate, DateTime endDate) => toCheck >= startDate && toCheck <= endDate;

        /// <summary>
        /// Check whether the specified date is between the two edge ones, including the case when .
        /// Returns true even when the date is strictly equal to one of the boundaries.
        /// </summary>
        /// <param name="toCheck">The date to be checked</param>
        /// <param name="startDate">The left boundary of the period to check</param>
        /// <param name="endDate">The right boundary of the period to check</param>
        /// <returns>True if the date occurs between - or at - the edge dates</returns>
        public static bool IsDateBetween(DateTime toCheck, DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
                return toCheck <= (endDate ?? toCheck);

            if (endDate == null)
                return toCheck >= (startDate ?? toCheck);

            return IsDateStrictlyBetween(toCheck, startDate.Value, endDate.Value);
        }

        /// <summary>
        /// Check whether the specified period are overlapping.
        /// </summary>
        /// <param name="first">The first period</param>
        /// <param name="second">The second period</param>
        /// <returns>True if the periods overlap.
        public static bool AreOverlapping(DateRangeValue first, DateRangeValue second) 
            
            => first.IncludesStrictly(second.Start.Value) || first.IncludesStrictly(second.End.Value) || second.Includes(first);

        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }

    }
}
