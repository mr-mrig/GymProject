using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class WorkoutSession
    {
        public WorkoutSession()
        {
            WorkUnit = new HashSet<WorkUnit>();
        }

        public long Id { get; set; }
        public long? PlannedDate { get; set; }
        public long StartTime { get; set; }
        public long? EndTime { get; set; }
        public long? Rating { get; set; }
        public long? WorkoutTemplateId { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual WorkoutTemplate WorkoutTemplate { get; set; }
        public virtual ICollection<WorkUnit> WorkUnit { get; set; }
    }
}
