using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace GymProject.Domain.Test.Util
{
    internal static class StaticUtils
    {


        public static void CheckCurrentTimestamp(DateTime toBeChecked)
        {
            Assert.InRange(toBeChecked, DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), DateTime.Now);
        }
    }
}
