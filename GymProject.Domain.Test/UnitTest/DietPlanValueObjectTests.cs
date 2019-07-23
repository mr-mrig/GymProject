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

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeUpTo(left));
        }

        [Fact]
        public void DateRangeFailChronolicalOrderViolated2()
        {
            DateTime left = DateTime.Now.AddDays(1);
            DateTime right = DateTime.Now;

            Assert.Throws<ValueObjectInvariantViolationException>(() => DateRangeValue.RangeUpTo(left));
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
            Assert.Equal(days, range.GetLength());
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
            Assert.Equal(days, range.GetLength());
        }



    }
}
