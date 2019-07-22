using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.Exceptions
{
    public class FitnessJournalDomainInvariantViolationException : Exception
    {


        public FitnessJournalDomainInvariantViolationException() : base() { }


        public FitnessJournalDomainInvariantViolationException(string msg) : base(msg) { }
    }
}
