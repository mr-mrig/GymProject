using System;
using System.Collections.Generic;

namespace GymProject.Infrastructure.EntityFramework.Persistence.Model
{
    public partial class DietHashtag
    {
        public DietHashtag()
        {
            DietHasHashtag = new HashSet<DietHasHashtag>();
        }

        public long Id { get; set; }
        public string Body { get; set; }
        public long EntryStatusTypeId { get; set; }
        public long ModeratorId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }
        public virtual User Moderator { get; set; }
        public virtual ICollection<DietHasHashtag> DietHasHashtag { get; set; }
    }
}
