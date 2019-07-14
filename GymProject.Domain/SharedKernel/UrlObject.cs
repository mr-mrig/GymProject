using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UrlObject : ValueObject
    {


        public string Address { get; }


        private UrlObject(string url)
        {
            Address = url;
        }


        public static UrlObject CreateLink(string url)
        {
            return new UrlObject(url);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Address;
        }

    }
}
