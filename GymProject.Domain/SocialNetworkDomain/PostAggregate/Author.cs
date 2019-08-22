using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Author : Entity<IdTypeValue>
    {


        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfileImage { get; private set; }


        private Author(string username, string imageProfileUrl) : base(null)
        {
            Username = UsernameValue.Register(username);
            ProfileImage = ProfilePictureValue.Link(UrlValue.CreateLink(imageProfileUrl));
        }


        public static Author Register(string username, string imageprofile)
        {
            return new Author(username, imageprofile);
        }

    }
}
