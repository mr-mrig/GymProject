using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class WorkoutTemplate
    {
        public WorkoutTemplate()
        {
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
            WorkoutSession = new HashSet<WorkoutSession>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long IsWeekDaySpecific { get; set; }
        public long ProgressiveNumber { get; set; }
        public long TrainingWeekId { get; set; }

        public virtual TrainingWeek TrainingWeek { get; set; }
        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }
        public virtual ICollection<WorkoutSession> WorkoutSession { get; set; }
    }
}
