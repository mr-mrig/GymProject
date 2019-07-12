using System;
using System.Collections.Generic;

namespace GymProject.Domain.EntityFramework.Persistence.Model
{
    public partial class Hashtag
    {
        public Hashtag()
        {
            PostHasHashtag = new HashSet<PostHasHashtag>();
        }

        public long Id { get; set; }
        public string Body { get; set; }
        public long EntryStatusTypeId { get; set; }
        public long? ModeratorId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual User Moderator { get; set; }
        public virtual ICollection<PostHasHashtag> PostHasHashtag { get; set; }
    }
}
