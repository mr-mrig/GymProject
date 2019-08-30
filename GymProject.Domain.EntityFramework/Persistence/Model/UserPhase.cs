using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class UserPhase
    {
        public long Id { get; set; }
        public long StartDate { get; set; }
        public long? EndDate { get; set; }
        public long CreatedOn { get; set; }
        public long OwnerId { get; set; }
        public long PhaseId { get; set; }
        public long? UserPhaseNoteId { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual User Owner { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual UserPhaseNote UserPhaseNote { get; set; }
    }
}
