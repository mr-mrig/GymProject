using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.Persistence.EFContext.Model
{
    public partial class TrainingHashtag
    {
        public TrainingHashtag()
        {
            TrainingPlanHasHashtag = new HashSet<TrainingPlanHasHashtag>();
        }

        public long Id { get; set; }
        public string Body { get; set; }
        public long EntryStatusTypeId { get; set; }
        public long? ModeratorId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual User Moderator { get; set; }
        public virtual ICollection<TrainingPlanHasHashtag> TrainingPlanHasHashtag { get; set; }
    }
}
