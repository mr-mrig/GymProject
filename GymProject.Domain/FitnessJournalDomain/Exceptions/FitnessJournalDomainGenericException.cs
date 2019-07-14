using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.Exceptions
{
    public class FitnessJournalDomainGenericException : Exception
    {


        public FitnessJournalDomainGenericException() : base() { }


        public FitnessJournalDomainGenericException(string msg) : base(msg) { }
    }
}
