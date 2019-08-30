using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class TrainingPlanRelation
    {
        public long ParentPlanId { get; set; }
        public long ChildPlanId { get; set; }
        public long RelationTypeId { get; set; }
        public long? TrainingPlanMessageId { get; set; }

        public virtual TrainingPlan ChildPlan { get; set; }
        public virtual TrainingPlan ParentPlan { get; set; }
        public virtual TrainingPlanRelationType RelationType { get; set; }
        public virtual TrainingPlanMessage TrainingPlanMessage { get; set; }
    }
}
