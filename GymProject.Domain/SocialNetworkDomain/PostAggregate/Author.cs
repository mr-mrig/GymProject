using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Author : Entity
    {


        public Username Username { get; private set; }


        public ProfileImage ProfileImage { get; private set; }


        private Author(string username, string imageProfileUrl)
        {
            Username = Username.Register(username);
            ProfileImage = ProfileImage.Link(imageProfileUrl);
        }


        public static Author Register(string username, string imageprofile)
        {
            return new Author(username, imageprofile);
        }

    }
}
