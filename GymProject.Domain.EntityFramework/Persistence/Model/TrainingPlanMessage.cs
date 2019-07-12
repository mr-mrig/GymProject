using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingPlanMessage
    {
        public TrainingPlanMessage()
        {
            TrainingPlanRelation = new HashSet<TrainingPlanRelation>();
        }

        public long Id { get; set; }
        public string Body { get; set; }

        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelation { get; set; }
    }
}
