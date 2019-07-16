using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class GlobalDomainGenericException : Exception
    {


        public GlobalDomainGenericException() : base() { }


        public GlobalDomainGenericException(string msg) : base(msg) { }

    }
}
