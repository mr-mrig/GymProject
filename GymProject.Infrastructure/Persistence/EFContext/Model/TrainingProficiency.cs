using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingProficiency
    {
        public TrainingProficiency()
        {
            TrainingPlanTargetProficiency = new HashSet<TrainingPlanTargetProficiency>();
            TrainingSchedule = new HashSet<TrainingSchedule>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrainingPlanTargetProficiency> TrainingPlanTargetProficiency { get; set; }
        public virtual ICollection<TrainingSchedule> TrainingSchedule { get; set; }
    }
}
