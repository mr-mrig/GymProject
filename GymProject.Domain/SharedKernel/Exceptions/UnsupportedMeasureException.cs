using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UnsupportedMeasureException : Exception
    {


        public UnsupportedMeasureException() : base() { }


        public UnsupportedMeasureException(string msg) : base(msg) { }

        public UnsupportedMeasureException(string fromUnitDescr, string toUnitDescr) 
            : this($"Invalid conversion: from {fromUnitDescr} to {toUnitDescr}") { }

        public UnsupportedMeasureException(string msg, string fromUnitDescr, string toUnitDescr)
            : this($"Invalid conversion: from {fromUnitDescr} to {toUnitDescr}. {msg}") { }
    }
}
