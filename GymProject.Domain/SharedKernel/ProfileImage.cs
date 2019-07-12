using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class ProfileImage : ValueObject
    {


        public string Url { get; private set; }



        private ProfileImage(string url)
        {
            Url = url;
        }


        public static ProfileImage Link(string url)
        {
            return new ProfileImage(url);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Url;
        }
    }
}
