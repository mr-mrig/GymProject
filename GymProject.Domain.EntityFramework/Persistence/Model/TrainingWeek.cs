using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class TrainingWeek
    {
        public TrainingWeek()
        {
            WorkoutTemplate = new HashSet<WorkoutTemplate>();
        }

        public long Id { get; set; }
        public long ProgressiveNumber { get; set; }
        public long? TrainingWeekTypeId { get; set; }
        public long TrainingPlanId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }
        public virtual TrainingWeekType TrainingWeekType { get; set; }
        public virtual ICollection<WorkoutTemplate> WorkoutTemplate { get; set; }
    }
}
