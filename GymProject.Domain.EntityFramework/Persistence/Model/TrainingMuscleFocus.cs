using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingMuscleFocus
    {
        public long TrainingPlanId { get; set; }
        public long MuscleId { get; set; }

        public virtual Muscle Muscle { get; set; }
        public virtual TrainingPlan TrainingPlan { get; set; }
    }
}
