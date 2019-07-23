using System;

namespace GymProject.Domain.SharedKernel.Exceptions
{
    public class GlobalDomainGenericException : Exception
    {


        public GlobalDomainGenericException() : base() { }


        public GlobalDomainGenericException(string msg) : base(msg) { }

    }
}
