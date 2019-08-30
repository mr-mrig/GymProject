using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class UserHasProficiency
    {
        public long UserId { get; set; }
        public long ProficiencyId { get; set; }
        public long OwnerId { get; set; }
        public long StartDate { get; set; }
        public long? EndDate { get; set; }

        public virtual User Owner { get; set; }
        public virtual User Proficiency { get; set; }
        public virtual User User { get; set; }
    }
}
