using System;
using System.Collections.Generic;
using Xunit;

namespace GymApp.Test
{
    public class UnitTest1
    {



        [Fact]
        public static void Test()
        {



            Object x1 = new Object();
            Object x2 = new Object();
            Object x3 = new Object();
            Object xnull = null;

            List<Object> xx = new List<Object>()
            {
                x1, x2, x3,
            };

            xx.Remove(xnull);


            System.Diagnostics.Debugger.Break();
        }


    }
}
