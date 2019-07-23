using System;

namespace GymProject.Domain.SharedKernel.Exceptions
{
    public class ValueObjectInvariantViolationException : Exception
    {


        public ValueObjectInvariantViolationException() : base() { }


        public ValueObjectInvariantViolationException(string msg) : base(msg) { }

    }
}