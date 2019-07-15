using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UrlValue : ValueObject
    {


        public string Address { get; }


        private UrlValue(string url)
        {
            Address = url;
        }


        public static UrlValue CreateLink(string url)
        {
            return new UrlValue(url);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Address;
        }

    }
}
