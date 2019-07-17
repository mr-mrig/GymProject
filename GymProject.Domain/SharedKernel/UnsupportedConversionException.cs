using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UnsupportedConversionException : Exception
    {


        public UnsupportedConversionException() : base() { }


        public UnsupportedConversionException(string msg) : base(msg) { }

        public UnsupportedConversionException(string fromUnitDescr, string toUnitDescr) 
            : this($"Invalid conversion: from {fromUnitDescr} to {toUnitDescr}") { }
    }
}
