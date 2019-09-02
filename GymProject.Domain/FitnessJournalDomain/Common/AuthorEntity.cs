using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.Common
{
    public class AuthorEntity : Entity<IdTypeValue>
    {

        public UsernameValue Username { get; private set; }


        public ProfilePictureValue ProfileImage { get; private set; }


        private AuthorEntity(string username, string imageProfileUrl) : base(null)
        {
            Username = UsernameValue.Register(username);
            ProfileImage = ProfilePictureValue.Link(UrlValue.CreateLink(imageProfileUrl));
        }


        public static AuthorEntity Register(string username, string imageprofile)
        {
            return new AuthorEntity(username, imageprofile);
        }
    }
}
