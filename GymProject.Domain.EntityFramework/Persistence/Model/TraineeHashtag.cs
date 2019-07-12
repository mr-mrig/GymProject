using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class TraineeHashtag
    {
        public TraineeHashtag()
        {
            TraineeHasHashtag = new HashSet<TraineeHasHashtag>();
        }

        public long Id { get; set; }
        public string Body { get; set; }
        public long EntryStatusTypeId { get; set; }
        public long? ModeratorId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual User Moderator { get; set; }
        public virtual ICollection<TraineeHasHashtag> TraineeHasHashtag { get; set; }
    }
}
