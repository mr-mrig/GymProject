using System;


namespace GymProject.Domain.PhaseDomain.Exceptions
{
    public class PhaseDomainInvariantViolationException : Exception
    {


        public PhaseDomainInvariantViolationException() : base() { }


        public PhaseDomainInvariantViolationException(string msg) : base(msg) { }
    }
}
