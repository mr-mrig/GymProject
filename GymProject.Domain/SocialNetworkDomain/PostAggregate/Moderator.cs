using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SocialNetworkDomain.PostAggregate
{
    public class Moderator : Entity
    {


        public UsernameObject Username { get; private set; }



        private Moderator(string username)
        {
            Username = UsernameObject.Register(username);
        }


        public static Moderator Register(string username)
        {
            return new Moderator(username);
        }

    }
}
