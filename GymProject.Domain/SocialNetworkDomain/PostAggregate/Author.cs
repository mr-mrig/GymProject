using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Author : Entity
    {


        public UsernameObject Username { get; private set; }


        public ProfilePictureObject ProfileImage { get; private set; }


        private Author(string username, string imageProfileUrl)
        {
            Username = UsernameObject.Register(username);
            ProfileImage = ProfilePictureObject.Link(UrlObject.CreateLink(imageProfileUrl));
        }


        public static Author Register(string username, string imageprofile)
        {
            return new Author(username, imageprofile);
        }

    }
}
