using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingPlanRelationType
    {
        public TrainingPlanRelationType()
        {
            TrainingPlanRelation = new HashSet<TrainingPlanRelation>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelation { get; set; }
    }
}
