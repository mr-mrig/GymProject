using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class UsernameValue : ValueObject
    {

        public string Name { get; private set; }


        private UsernameValue(string username)
        {
            Name = username;
        }


        public static UsernameValue Register(string username)
        {
            return new UsernameValue(username);
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
