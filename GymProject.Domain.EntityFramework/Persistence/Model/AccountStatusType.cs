using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class AccountStatusType
    {
        public AccountStatusType()
        {
            User = new HashSet<User>();
        }

        public long Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
