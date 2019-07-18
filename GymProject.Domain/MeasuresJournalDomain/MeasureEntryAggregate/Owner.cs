using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class Owner : Entity<IdType>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfileImage { get; private set; }


        private Owner(string username, string imageProfileUrl)
        {
            Username = UsernameValue.Register(username);
            ProfileImage = ProfilePictureValue.Link(UrlValue.CreateLink(imageProfileUrl));
        }


        public static Owner Register(string username, string imageprofile)
        {
            return new Owner(username, imageprofile);
        }

    }
}
