using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UsernameObject : ValueObject
    {

        public string Name { get; private set; }


        private UsernameObject(string username)
        {
            Name = username;
        }


        public static UsernameObject Register(string username)
        {
            return new UsernameObject(username);
        }



        private bool Validate()
        {
            throw new NotImplementedException();
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
        }
    }
}
