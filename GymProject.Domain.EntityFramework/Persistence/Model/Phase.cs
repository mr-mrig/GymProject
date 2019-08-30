using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class Phase
    {
        public Phase()
        {
            TrainingPlanHasPhase = new HashSet<TrainingPlanHasPhase>();
            TrainingSchedule = new HashSet<TrainingSchedule>();
            UserPhase = new HashSet<UserPhase>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long EntryStatusTypeId { get; set; }
        public long OwnerId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<TrainingPlanHasPhase> TrainingPlanHasPhase { get; set; }
        public virtual ICollection<TrainingSchedule> TrainingSchedule { get; set; }
        public virtual ICollection<UserPhase> UserPhase { get; set; }
    }
}
