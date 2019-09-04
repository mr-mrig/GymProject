using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingPlanHasHashtag
    {
        public long TrainingPlanId { get; set; }
        public long TrainingHashtagId { get; set; }
        public long ProgressiveNumber { get; set; }

        public virtual TrainingHashtag TrainingHashtag { get; set; }
        public virtual TrainingPlan TrainingPlan { get; set; }
    }
}
