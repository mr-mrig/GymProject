using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class WorkUnit
    {
        public WorkUnit()
        {
            WorkingSet = new HashSet<WorkingSet>();
        }

        public long Id { get; set; }
        public long? QuickRating { get; set; }
        public long ProgressiveNumber { get; set; }
        public long WorkoutSessionId { get; set; }
        public long ExcerciseId { get; set; }

        public virtual Excercise Excercise { get; set; }
        public virtual WorkoutSession WorkoutSession { get; set; }
        public virtual ICollection<WorkingSet> WorkingSet { get; set; }
    }
}
