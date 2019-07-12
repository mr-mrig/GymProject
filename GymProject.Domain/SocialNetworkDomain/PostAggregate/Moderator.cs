using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Moderator : Entity
    {


        public Username Username { get; private set; }



        private Moderator(string username)
        {
            Username = Username.Register(username);
        }


        public static Moderator Register(string username)
        {
            return new Moderator(username);
        }

    }
}
