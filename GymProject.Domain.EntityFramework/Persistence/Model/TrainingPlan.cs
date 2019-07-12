using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TrainingPlan
    {
        public TrainingPlan()
        {
            TrainingMuscleFocus = new HashSet<TrainingMuscleFocus>();
            TrainingPlanHasHashtag = new HashSet<TrainingPlanHasHashtag>();
            TrainingPlanHasPhase = new HashSet<TrainingPlanHasPhase>();
            TrainingPlanRelationChildPlan = new HashSet<TrainingPlanRelation>();
            TrainingPlanRelationParentPlan = new HashSet<TrainingPlanRelation>();
            TrainingPlanTargetProficiency = new HashSet<TrainingPlanTargetProficiency>();
            TrainingSchedule = new HashSet<TrainingSchedule>();
            TrainingWeek = new HashSet<TrainingWeek>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long IsBookmarked { get; set; }
        public long IsTemplate { get; set; }
        public long CreatedOn { get; set; }
        public long OwnerId { get; set; }
        public long? TrainingPlanNoteId { get; set; }

        public virtual User Owner { get; set; }
        public virtual TrainingPlanNote TrainingPlanNote { get; set; }
        public virtual ICollection<TrainingMuscleFocus> TrainingMuscleFocus { get; set; }
        public virtual ICollection<TrainingPlanHasHashtag> TrainingPlanHasHashtag { get; set; }
        public virtual ICollection<TrainingPlanHasPhase> TrainingPlanHasPhase { get; set; }
        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelationChildPlan { get; set; }
        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelationParentPlan { get; set; }
        public virtual ICollection<TrainingPlanTargetProficiency> TrainingPlanTargetProficiency { get; set; }
        public virtual ICollection<TrainingSchedule> TrainingSchedule { get; set; }
        public virtual ICollection<TrainingWeek> TrainingWeek { get; set; }
    }
}
