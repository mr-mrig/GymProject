using System;


namespace GymProject.Domain.TrainingDomain.Exceptions
{
    public class TrainingDomainInvariantViolationException : Exception
    {


        public TrainingDomainInvariantViolationException() : base() { }


        public TrainingDomainInvariantViolationException(string msg) : base(msg) { }
    }
}
