using System;

namespace GymProject.Domain.BodyDomain.Exceptions
{
    public class BodyDomainInvariantViolationException : Exception
    {


        public BodyDomainInvariantViolationException() : base() { }


        public BodyDomainInvariantViolationException(string msg) : base(msg) { }
    }
}
