using System;


namespace GymProject.Domain.DietDomain.Exceptions
{
    public class DietDomainIvariantViolationException : Exception
    {


        public DietDomainIvariantViolationException() : base() { }


        public DietDomainIvariantViolationException(string msg) : base(msg) { }
    }
}
