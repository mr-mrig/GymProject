using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingPlanNote
    {
        public TrainingPlanNote()
        {
            TrainingPlan = new HashSet<TrainingPlan>();
        }

        public long Id { get; set; }
        public string Body { get; set; }

        public virtual ICollection<TrainingPlan> TrainingPlan { get; set; }
    }
}
