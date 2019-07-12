using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class Username : ValueObject
    {

        public string Name { get; private set; }


        private Username(string username)
        {
            Name = username;
        }


        public static Username Register(string username)
        {
            return new Username(username);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
        }
    }
}
