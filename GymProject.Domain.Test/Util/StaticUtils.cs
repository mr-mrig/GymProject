using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace GymProject.Domain.Test.Util
{
    internal static class StaticUtils
    {

        /// <summary>
        /// Checks current timestamp is as expected
        /// </summary>
        /// <param name="toBeChecked">The datetime object storing the current timestamp</param>
        public static void CheckCurrentTimestamp(DateTime toBeChecked)
        {
            Assert.InRange(toBeChecked, DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), DateTime.Now);
        }

        /// <summary>
        /// Checks that two values are inside a specific tolerance.
        /// This should be used to check conversions where rounding can lead to precision issues
        /// </summary>
        /// <param name="srcValue">The original value</param>
        /// <param name="convertedValue">The converted value</param>
        /// <param name="srcUnitMeasId">The Meas Unit ID of the original value</param>
        /// <param name="convertedUnitMeasId">The Meas Unit ID of the converted  value</param>
        /// <param name="tolerance">The tolerance as a [0-1] float - default = 1.5% </param>
        public static void CheckConversions(float srcValue, float convertedValue, int srcUnitMeasId, int convertedUnitMeasId, float tolerance = 0.15f)
        {
            Assert.InRange(convertedValue, srcValue * (1 - tolerance), srcValue * (1 + tolerance));
            Assert.Equal(srcUnitMeasId, convertedUnitMeasId);
        }
    }
}
