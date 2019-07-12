using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.Exceptions
{
    public class SocialNetworkGenericException : Exception
    {


        public SocialNetworkGenericException() : base() { }


        public SocialNetworkGenericException(string msg) : base(msg) { }

    }
}
