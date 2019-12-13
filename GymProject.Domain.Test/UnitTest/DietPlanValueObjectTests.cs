using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class DietPlanValueObjectTests
    {


        [Fact]
        public void DateRangeBetweenFailInfinite()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MaxValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeBetween(left, right));
        }


        [Fact]
        public void DateRangeBetweenFailInfinite2()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MinValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeBetween(left, right));
        }

        [Fact]
        public void DateRangeFailStartFromInfinite()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MaxValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeStartingFrom(left));
        }

        [Fact]
        public void DateRangeFailStartFromInfinite2()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MaxValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeStartingFrom(right));
        }

        [Fact]
        public void DateRangeFailUpToInfinite()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MaxValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeUpTo(right));
        }

        [Fact]
        public void DateRangeFailUpToInfinite2()
        {
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.MaxValue;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeUpTo(left));
        }

        [Fact]
        public void DateRangeFailChronolicalOrderViolated()
        {
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(-1);

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeBetween(left, right));
        }

        [Fact]
        public void DateRangeFailChronolicalOrderViolated2()
        {
            DateTime left = DateTime.Now.AddDays(1);
            DateTime right = DateTime.Now;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeBetween(left, right));
        }

        [Fact]
        public void DateRangeBetweenBounded()
        {
            int days = 10;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            Assert.NotNull(range);
            Assert.Equal(left, range.Start);
            Assert.Equal(right, range.End);
            Assert.Equal(days + 1, range.GetLength());
            Assert.True(range.IsLeftBounded());
            Assert.True(range.IsRightBounded());
        }

        [Fact]
        public void DateRangeBetweenLeftUnbounded()
        {
            int days = 10;
            DateTime left = DateTime.MinValue;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            Assert.NotNull(range);
            Assert.Equal(left, range.Start);
            Assert.Equal(right, range.End);
            Assert.False(range.IsLeftBounded());
            Assert.True(range.IsRightBounded());
        }

        [Fact]
        public void DateRangeBetweenRightUnbounded()
        {
            DateTime left = DateTime.Now;
            DateTime right = DateTime.MaxValue;

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            Assert.NotNull(range);
            Assert.Equal(left, range.Start);
            Assert.Equal(right, range.End);
            Assert.True(range.IsLeftBounded());
            Assert.False(range.IsRightBounded());
        }

        [Fact]
        public void DateRangeUpTo()
        {
            DateTime right = DateTime.Now;

            DateRangeValue range = DateRangeValue.RangeUpTo(right);

            Assert.NotNull(range);
            Assert.Equal(DateTime.MinValue, range.Start);
            Assert.Equal(right, range.End);
            Assert.False(range.IsLeftBounded());
            Assert.True(range.IsRightBounded());
        }

        [Fact]
        public void DateRangeStartingFrom()
        {
            DateTime left = DateTime.Now;

            DateRangeValue range = DateRangeValue.RangeStartingFrom(left);

            Assert.NotNull(range);
            Assert.Equal(left, range.Start);
            Assert.Equal(DateTime.MaxValue, range.End);
            Assert.True(range.IsLeftBounded());
            Assert.False(range.IsRightBounded());
        }

        [Fact]
        public void DateRangeBetweenReschedule()
        {
            int days = 10, newDays = 20;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateTime newStart = left.AddDays(5);
            DateTime newEnd = newStart.AddDays(newDays);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            DateRangeValue final = range.RescheduleEnd(newEnd).RescheduleStart(newStart);

            Assert.Equal(newStart, final.Start);
            Assert.Equal(newEnd, final.End);
            Assert.Equal(newDays + 1, final.GetLength());
        }

        [Fact]
        public void DateRangeBetweenReschedule2()
        {
            int days = 10, newDays = 20;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateTime newStart = left.AddDays(5);
            DateTime newEnd = newStart.AddDays(newDays);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            DateRangeValue final =range.RescheduleStart(newStart).RescheduleEnd(newEnd);

            Assert.Equal(newStart, final.Start);
            Assert.Equal(newEnd, final.End);
            Assert.Equal(newDays + 1, final.GetLength());
        }

        [Fact]
        public void DateRangeBetweenRescheduleFail()
        {
            int days = 10, newDays = 20;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateTime newStart = left.AddDays(5);
            DateTime newEnd = newStart.AddDays(newDays);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            Assert.Throws<ValueObjectInvariantViolationException>(() =>range.RescheduleEnd(newStart).RescheduleStart(newEnd));
        }

        [Fact]
        public void DateRangeUpToReschedule2()
        {
            int days = 10, newDays = 20;
            //DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days).AddHours(4);

            DateTime newStart = DateTime.Now.AddDays(5);
            DateTime newEnd = newStart.AddDays(newDays);

            DateRangeValue range = DateRangeValue.RangeUpTo(right);

            DateRangeValue final = range.RescheduleStart(newStart).RescheduleEnd(newEnd);

            Assert.Equal(newStart, final.Start);
            Assert.Equal(newEnd, final.End);
            Assert.Equal(newDays + 1, final.GetLength());
        }

        [Fact]
        public void DateRange_Overlaps()
        {
            int days = 10;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = left.AddDays(days - 1);
            DateTime right2 = left2.AddDays(days);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.True(range2.Overlaps(range));
        }

        [Fact]
        public void DateRange_Overlaps_SharingOneBoundary()
        {
            int days = 10;
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right;
            DateTime right2 = right.AddDays(days);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.True(range2.Overlaps(range));
        }
        
        [Fact]
        public void DateRange_Overlaps_UnboundedCase()
        {
            int days = 10;

            // No right boundary
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = left.AddDays(1);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeStartingFrom(left2);

            Assert.True(range2.Overlaps(range));

            // No left boundary
            DateTime right3 = left.AddDays(1);

            DateRangeValue range3 = DateRangeValue.RangeUpTo(right3);

            Assert.True(range3.Overlaps(range));
        }  

        [Fact]
        public void DateRange_DoesNotOverlap()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime right2 = left.AddDays(-1);
            DateTime left2 = right2.AddDays(-10);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.False(range2.Overlaps(range));
        }
        
        [Fact]
        public void DateRange_DoesNotOverlap_UnboundedCase()
        {
            int days = 10;

            // No right boundary
            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right.AddDays(10);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeStartingFrom(left2);

            Assert.False(range2.Overlaps(range));

            // No left boundary
            DateTime right3 = left.AddDays(-1);

            DateRangeValue range3 = DateRangeValue.RangeUpTo(right3);

            Assert.False(range3.Overlaps(range));
        }

        [Fact]
        public void DateRange_Includes_Date()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);
            DateRangeValue range = DateRangeValue.RangeBetween(left, right);

            DateTime dateIncluded = left.AddDays(1);
            DateTime pastDate = left.AddDays(-1);
            DateTime futureDate = right.AddDays(1);

            Assert.True(range.Includes(dateIncluded));
            Assert.True(range.IncludesStrictly(dateIncluded));            
            Assert.False(range.Includes(pastDate));
            Assert.False(range.IncludesStrictly(pastDate));            
            Assert.False(range.Includes(futureDate));
            Assert.False(range.IncludesStrictly(futureDate));        
            Assert.True(range.Includes(left));
            Assert.True(range.IncludesStrictly(left));
            Assert.True(range.Includes(right));
            Assert.True(range.IncludesStrictly(right));
        }

        [Fact]
        public void DateRange_Includes_Range()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = left.AddDays(1);
            DateTime right2 = right.AddDays(-1);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.True(range.Includes(range2));
            Assert.False(range2.Includes(range));
        }

        [Fact]
        public void DateRange_Includes_RangeSharingOneBoundary()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);
            DateTime left2 = left;
            DateTime right2 = right.AddDays(-1);    
            DateTime left3 = left.AddDays(1);
            DateTime right3 = right;

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);
            DateRangeValue range3 = DateRangeValue.RangeBetween(left3, right3);

            Assert.True(range.Includes(range2));
            Assert.False(range2.Includes(range));            
            Assert.True(range.Includes(range3));
            Assert.False(range3.Includes(range));
        }

        [Fact]
        public void DateRange_DoesNotInclude_Range()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime right2 = left.AddDays(-1);
            DateTime left2 = right2.AddDays(-11);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.False(range.Includes(range2));
            Assert.False(range2.Includes(range));
        }

        [Fact]
        public void DateRange_DoesNotInclude_Unbounded()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);
            DateTime right2 = left.AddDays(-1);
            DateTime left3 = right.AddDays(1);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeUpTo(right2);
            DateRangeValue range3 = DateRangeValue.RangeStartingFrom(left3);

            Assert.False(range.Includes(range2));
            Assert.False(range.Includes(range3));
        }

        [Fact]
        public void DateRangeBetweenGetGap()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right.AddDays(days);
            DateTime right2 = left2.AddDays(days);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.Equal(days - 1, range.GepBetween(range2).GetLength());   // days - edge days
        }

        [Fact]
        public void DateRangeJoinFail()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right.AddDays(days + 2);
            DateTime right2 = left2.AddDays(days + 2);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.Null(range.Join(range2));
        }

        [Fact]
        public void DateRangeJoin()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right.AddDays(1);
            DateTime right2 = left2.AddDays(days);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.Equal(DateRangeValue.RangeBetween(left, right2), range.Join(range2));
        }

        [Fact]
        public void DateRangeJoinOverlapping()
        {
            int days = 10;

            DateTime left = DateTime.Now;
            DateTime right = DateTime.Now.AddDays(days);

            DateTime left2 = right.AddDays(-1);
            DateTime right2 = left2.AddDays(days);

            DateRangeValue range = DateRangeValue.RangeBetween(left, right);
            DateRangeValue range2 = DateRangeValue.RangeBetween(left2, right2);

            Assert.Equal(DateRangeValue.RangeBetween(left, right2), range.Join(range2));
        }

        [Fact]
        public void PersonalNoteFail()
        {
            string body = "ciao ciao.";
            body = body.PadLeft(PersonalNoteValue.DefaultMaximumLength + 1, 'x');

            Assert.Throws<ValueObjectInvariantViolationException>(() => PersonalNoteValue.Write(body));
        }

        [Fact]
        public void PersonalNote()
        {
            string body = "ciao ciao.";

            PersonalNoteValue note = PersonalNoteValue.Write(body);

            Assert.NotNull(note);
            Assert.Equal(body, note.Body);
        }

        [Fact]
        public void WeeklyOccuranceExceedsFail()
        {
            int num = 8;
            Assert.Throws<ValueObjectInvariantViolationException>(() => WeeklyOccuranceValue.TrackOccurance(num));
        }

        [Fact]
        public void WeeklyOccuranceNegativeFail()
        {
            int num = -1;
            Assert.Throws<ValueObjectInvariantViolationException>(() => WeeklyOccuranceValue.TrackOccurance(num));
        }

        [Fact]
        public void WeeklyOccuranceNever()
        {
            int num = 0;
            WeeklyOccuranceValue wk = WeeklyOccuranceValue.TrackOccurance(num);

            Assert.NotNull(wk);
            Assert.Equal(num, wk.Value);
            Assert.True(wk.Never());
        }

        [Fact]
        public void WeeklyOccuranceSometimes()
        {
            int num = 2;
            WeeklyOccuranceValue wk = WeeklyOccuranceValue.TrackOccurance(num);

            Assert.NotNull(wk);
            Assert.Equal(num, wk.Value);
            Assert.False(wk.Never());
        }
    }
}
