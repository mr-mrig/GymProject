using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class TrainingSchedule
    {
        public TrainingSchedule()
        {
            TrainingScheduleFeedback = new HashSet<TrainingScheduleFeedback>();
        }

        public long Id { get; set; }
        public long StartDate { get; set; }
        public long? EndDate { get; set; }
        public long? PlannedEndDate { get; set; }
        public long TrainingPlanId { get; set; }
        public long? PhaseId { get; set; }
        public long? TrainingProficiencyId { get; set; }

        public virtual Post IdNavigation { get; set; }
        public virtual Phase Phase { get; set; }
        public virtual TrainingPlan TrainingPlan { get; set; }
        public virtual TrainingProficiency TrainingProficiency { get; set; }
        public virtual ICollection<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }
    }
}
