using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class UserPhaseNote
    {
        public UserPhaseNote()
        {
            UserPhase = new HashSet<UserPhase>();
        }

        public long Id { get; set; }
        public string Body { get; set; }

        public virtual ICollection<UserPhase> UserPhase { get; set; }
    }
}
