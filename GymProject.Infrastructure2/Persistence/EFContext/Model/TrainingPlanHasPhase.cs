using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingPlanHasPhase
    {
        public long PlanId { get; set; }
        public long PhaseId { get; set; }

        public virtual Phase Phase { get; set; }
        public virtual TrainingPlan Plan { get; set; }
    }
}
