using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class ProfilePictureObject : ValueObject
    {


        public UrlObject Url { get; private set; }



        private ProfilePictureObject(UrlObject url)
        {
            Url = url;
        }


        public static ProfilePictureObject Link(UrlObject url)
        {
            return new ProfilePictureObject(url);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Url;
        }
    }
}
