using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class ProfilePictureValue : ValueObject
    {


        public UrlValue Url { get; private set; }



        private ProfilePictureValue(UrlValue url)
        {
            Url = url;
        }


        public static ProfilePictureValue Link(UrlValue url)
        {
            return new ProfilePictureValue(url);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Url;
        }
    }
}
