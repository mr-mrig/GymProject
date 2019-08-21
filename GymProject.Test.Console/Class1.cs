using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Test.Console
{
    public class Class1
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
